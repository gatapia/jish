using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using js.net.Engine;
using js.net.jish.Command;
using js.net.jish.InlineCommand;
using js.net.Util;
using Noesis.Javascript;

namespace js.net.jish
{
  public class CommandLineInterpreter : ICommandLineInterpreter
  {
    private readonly IList<ICommand> commands = new List<ICommand>();
    protected IDictionary<string, Assembly> loadedAssemblies = new Dictionary<string, Assembly>();

    private readonly JSConsole console;
    private readonly IEngine engine;
    private string bufferedCommand = String.Empty;

    public CommandLineInterpreter(IEngine engine, JSConsole console)
    {
      Trace.Assert(engine != null);
      Trace.Assert(console != null);

      this.engine = engine;
      this.console = console;
      
      Initialise();      
    }

    private void Initialise()
    {
      Assembly[] assemblies = LoadAllAssemblies().Distinct(new AssemblyNameComparer()).ToArray();
      Array.ForEach(assemblies, LoadAllCommandsFromAssembly);      
      Array.ForEach(assemblies, LoadAllInlineCommandsFromAssembly);      
      Array.ForEach(assemblies, a => loadedAssemblies.Add(a.GetName().Name, a));
      // TODO: There is nothing in jish.js is it really needed? Is there any real immediate need for it?
      EmbeddedResourcesUtils embedded = new EmbeddedResourcesUtils();      
      engine.Run(embedded.ReadEmbeddedResourceTextContents("js.net.jish.resources.jish.js", GetType().Assembly), "jish.js");
      LoadJavaScriptModules();
    }
    

    private IEnumerable<Assembly> LoadAllAssemblies()
    {
      Assembly[] defaultAssemlies = AppDomain.CurrentDomain.GetAssemblies();
      if (!Directory.Exists("modules")) return defaultAssemlies;
      string[] assemblyFiles = Directory.GetFiles("modules", "*.dll", SearchOption.AllDirectories);
      if (assemblyFiles.Length == 0) return defaultAssemlies;
      IEnumerable<Assembly> moduleAssemblies = assemblyFiles.Select(Assembly.LoadFrom);
      return defaultAssemlies.Concat(moduleAssemblies);
    }

    private void LoadAllCommandsFromAssembly(Assembly assembly)
    {
      foreach (Type t in GetAllTypesThatImplement(assembly, typeof(ICommand)))
      {
        ICommand command = (ICommand) Activator.CreateInstance(t);
        command.JavaScriptConsole = console;
        command.JishEngine = this;
        commands.Add(command);
      }      
    }
    
    private void LoadAllInlineCommandsFromAssembly(Assembly assembly)
    {
      foreach (Type t in GetAllTypesThatImplement(assembly, typeof(IInlineCommand)))
      {
        IInlineCommand icommand = (IInlineCommand) Activator.CreateInstance(t);
        string ns = icommand.GetNameSpace();
        if (String.IsNullOrWhiteSpace(ns)) { throw new ApplicationException("Could not load inline command from type[" + t.FullName + "].  No namespace specified.");}
        if (ns.IndexOf('.') > 0)
        {
          throw new ApplicationException("Nested namespaces (namespaces with '.' in them) are not supported.");
        }
        engine.SetGlobal(ns, icommand);
      }
    }

    private IEnumerable<Type> GetAllTypesThatImplement(Assembly assembly, Type iface)
    {
      return assembly.GetTypes().Where(t => !t.IsAbstract && iface.IsAssignableFrom(t));
    }


    private void LoadJavaScriptModules()
    {
      if (!Directory.Exists("modules")) return;
      string[] modules = Directory.GetFiles("modules", "*.js", SearchOption.AllDirectories);
      Array.ForEach(modules, LoadJavaScriptModule);
    }

    private void LoadJavaScriptModule(string file)
    {
      console.log("Loading JavaScript Module: " + file);
      RunFile(file);
      console.log("Successfully Imported JavaScript Module.");
    }

    public virtual string ReadCommand()
    {
      console.log("> ", false);
      string input = Console.ReadLine().Trim();
      if (String.IsNullOrWhiteSpace(input))
      {
        return null;
      }
      return input;      
    }
    
    public void ExecuteCommand(string input)
    {  
      if (input.StartsWith("."))
      {
        InterceptSpecialCommands(input);
        return;
      }

      try
      {
        bufferedCommand += input + '\n';
        object returnValue;
        if (!AttemptToRunCommand(out returnValue)) { return; } // Is multi-line
        if (IsLoggableReturnValue(returnValue))
        {
          console.log(returnValue);
        }
        if (returnValue != null) engine.SetGlobal("_", returnValue);
      }
      catch (Exception e)
      {
        bufferedCommand = String.Empty;
        PrintExceptionMessage(e);
      }
    }

    private bool IsLoggableReturnValue(object returnValue)
    {
      if (returnValue == null) return false;
      if (returnValue is string && (string) returnValue == String.Empty) return false;
      if (returnValue is IDictionary<string, object> && !((IDictionary<string, object>)returnValue).Any()) return false;
      return true;
    }

    private bool AttemptToRunCommand(out object returnValue)
    {
      returnValue = null;
      try
      {
        returnValue = engine.Run("(" + bufferedCommand + ")", "jish");
        bufferedCommand = String.Empty;
      }  catch (JavascriptException)
      {
        try
        {
          returnValue = engine.Run(bufferedCommand, "jish");
          bufferedCommand = String.Empty;
        } catch (JavascriptException ex2)
        {
          string msg = ex2.Message;
          if (msg.IndexOf("Unexpected token ILLEGAL") >= 0 || msg.IndexOf("SyntaxError") < 0)
          {
            throw;
          }
          return false;
        }
      }
      return true;
    }

    public void RunFile(string file, string[] args = null)
    {
      engine.SetGlobal("args", args ?? new string[] {});
      FileInfo fi = new FileInfo(file);
      string cwd = Directory.GetCurrentDirectory();
      Directory.SetCurrentDirectory(fi.Directory.FullName);
      IList<string> lines = File.ReadAllLines(fi.Name).Select(l => l.Trim()).ToList();            
      RunAllJishCommands(lines);
      RunRestOfFile(file, lines);
      Directory.SetCurrentDirectory(cwd);
    }    

    private void RunAllJishCommands(IEnumerable<string> lines)
    {
      List<string> jishCommands = lines.Where(l => l.StartsWith(".")).ToList();
      if (jishCommands.Any(c => c.StartsWith(".clear") || c.StartsWith(".help")))
      {
        throw new ApplicationException(".clear and .help commands are not allowed when running jish files.");
      }      
      jishCommands.ForEach(InterceptSpecialCommands);
    }

    private void RunRestOfFile(string file, IEnumerable<string> lines)
    {
      // Do not loose line numbers
      IEnumerable<string> nonCommands = lines.Select(l => l.StartsWith(".") ? "" : l); 
      string nonCommandsInline = String.Join("\n", nonCommands);
      engine.Run(nonCommandsInline, new FileInfo(file).Name);
    }

    public void ClearBufferedCommand()
    {
      bufferedCommand = String.Empty;      
    }

    public IEnumerable<ICommand> GetCommands()
    {
      return commands;
    }

    public IDictionary<string, Assembly> GetLoadedAssemblies()
    {
      return loadedAssemblies;
    } 

    public void SetGlobal(string name, object valud)
    {
      engine.SetGlobal(name, valud);
    }

    public void InitialiseConsole()
    {
      Console.TreatControlCAsInput = false;
      Console.CancelKeyPress += (s, e) => Environment.Exit(0);
    }    

    private void PrintExceptionMessage(Exception e)
    {
      string msg = e.Message;
      if (msg.IndexOf(": ") > 0) msg = msg.Substring(msg.IndexOf(": ") + 2);
      if (msg.IndexOf('(') > 0) msg = msg.Substring(0, msg.IndexOf('('));

      console.log(msg);
    }

    private void InterceptSpecialCommands(string input)
    {

      string commandName = new Regex(@"\.([A-z0-9])+").Match(input).Captures[0].Value.Substring(1).Trim();
      foreach (ICommand command in commands)
      {
        if (!command.GetName().Equals(commandName)) continue;
        command.Execute(input);
        return;
      }
      console.log("Could not find command: " + input);                  
    }      
  }
}