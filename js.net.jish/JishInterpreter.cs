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
using js.net.jish.Util;
using js.net.Util;
using Ninject;
using Noesis.Javascript;

namespace js.net.jish
{
  public class JishInterpreter : IJishInterpreter
  {
    private readonly IDictionary<string, Type> commands = new Dictionary<string, Type>();
    // private readonly IList<ICommand> loadedCommands = new List<ICommand>();    

    private readonly JSConsole console;
    private readonly IEngine engine;
    private readonly LoadedAssembliesBucket loadedAssemblies;
    private readonly EmbeddedResourcesUtils embeddedResourceLoader;
    private readonly IKernel kernel;
    private readonly HelpMgr helpManager;

    private string bufferedCommand = String.Empty;

    public JishInterpreter(IEngine engine, JSConsole console, LoadedAssembliesBucket loadedAssemblies, EmbeddedResourcesUtils embeddedResourceLoader, IKernel kernel, HelpMgr helpManager)
    {
      Trace.Assert(engine != null);
      Trace.Assert(console != null);

      this.engine = engine;
      this.helpManager = helpManager;
      this.kernel = kernel;
      this.embeddedResourceLoader = embeddedResourceLoader;
      this.loadedAssemblies = loadedAssemblies;
      this.console = console;           
    }

    public void InitialiseDependencies()
    {
      engine.Run(embeddedResourceLoader.ReadEmbeddedResourceTextContents("js.net.jish.resources.jish.js", GetType().Assembly), "jish.js");

      Assembly[] assemblies = LoadAllAssemblies().Distinct(new AssemblyNameComparer()).ToArray();
      Array.ForEach(assemblies, LoadAllCommandsFromAssembly);      
      Array.ForEach(assemblies, LoadAllInlineCommandsFromAssembly);      
      Array.ForEach(assemblies, loadedAssemblies.AddAssembly);
      
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
        ICommand command = CreateCommand(kernel, t);
        commands.Add(command.GetName(), t);
        if (command.GetType().Assembly.FullName.IndexOf("js.net.test.module") < 0)
        {
          helpManager.AddHelpForSpecialCommand(command);
        }
      }
    }

    private ICommand CreateCommand(IKernel kernel, Type t)
    {
      return (ICommand) kernel.Get(t);
    }

    private void LoadAllInlineCommandsFromAssembly(Assembly assembly)
    {
      IDictionary<string, IList<IInlineCommand>> icommands = new Dictionary<string, IList<IInlineCommand>>();
      foreach (Type t in GetAllTypesThatImplement(assembly, typeof(IInlineCommand)))
      {
        IInlineCommand icommand = (IInlineCommand) kernel.Get(t);        
        string ns = icommand.GetNameSpace();        
        if (String.IsNullOrWhiteSpace(ns)) { throw new ApplicationException("Could not load inline command from type[" + t.FullName + "].  No namespace specified.");}
        if (ns.IndexOf('.') > 0) { throw new ApplicationException("Nested namespaces (namespaces with '.' in them) are not supported."); }
        if (!icommands.ContainsKey(ns)) icommands.Add(ns, new List<IInlineCommand>());
        if (icommand.GetType().Assembly.FullName.IndexOf("js.net.test.module") < 0)
        {
          helpManager.AddHelpForInlineCommand(icommand);
        }      
        icommands[ns].Add(icommand);
      }

      foreach (KeyValuePair<string, IList<IInlineCommand>> nsCommands in icommands)
      {
        InjectCommands(nsCommands.Key, nsCommands.Value);
      }
    }

    private void InjectCommands(string ns, IList<IInlineCommand> nsCommands)
    {
      const string jsBinder = 
@"
global['{0}']['{1}'] = function() {{
  return global['{2}']['{1}'].apply(global, arguments)
}};
";
      StringBuilder js = new StringBuilder(String.Format("\nif (!global['{0}']) global['{0}'] = {{}};\n", ns));
      foreach (IInlineCommand command in nsCommands)
      {
        string tmpClassName = "__" + Guid.NewGuid();
        engine.SetGlobal(tmpClassName, command);

        foreach (string method in command.GetType().GetMethods().Select(mi => mi.Name).Distinct().Where(m => Char.IsLower(m[0])))
        {
          js.Append(String.Format(jsBinder, ns, method, tmpClassName));
        }
      }
      engine.Run(js.ToString(), "JishInterpreter.InjectCommands");
    }

    private IEnumerable<Type> GetAllTypesThatImplement(Assembly assembly, Type iface)
    {
      try
      {
        Type[] types = assembly.GetTypes();
        return types.Where(t => !t.IsAbstract && iface.IsAssignableFrom(t));
      } catch (ReflectionTypeLoadException ex) {
        foreach(Exception inner in ex.LoaderExceptions) {
            Console.WriteLine(inner);
        }
        throw;
      }
    }


    private void LoadJavaScriptModules()
    {
      if (!Directory.Exists("modules")) return;
      string[] modules = Directory.GetFiles("modules", "*.js", SearchOption.AllDirectories);
      Array.ForEach(modules, LoadJavaScriptModule);
    }

    private void LoadJavaScriptModule(string file)
    {
      RunFile(file);
      console.log("Successfully Imported JavaScript Module: " + file);
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
        if (ThrowErrors) throw;
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
        returnValue = engine.Run("(" + bufferedCommand + ")", "JishInterpreter.AttemptToRunCommand_1");
        bufferedCommand = String.Empty;
      }  catch (JavascriptException)
      {
        try
        {
          returnValue = engine.Run(bufferedCommand, "JishInterpreter.AttemptToRunCommand_2");
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

    public void SetGlobal(string name, object valud)
    {
      engine.SetGlobal(name, valud);
    }

    public void InitialiseInputConsole()
    {
      try
      {
        Console.TreatControlCAsInput = false;
        Console.CancelKeyPress += (s, e) => Environment.Exit(0);
      } catch {} // Ignore, as this throws when running in a Process (tests)
    }

    public bool ThrowErrors { get; set; }

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
      if (commandName.Equals("help"))
      {
        console.log(helpManager.GetHelpString());
        return;
      }
      if (!commands.ContainsKey(commandName))
      {
        console.log("Could not find command: " + input);
        return;
      }
      CreateCommand(kernel, commands[commandName]).Execute(ParseSpecialCommandInputs(input));
    }

    private string[] ParseSpecialCommandInputs(string input)
    {
      if (input.IndexOf('(') < 0) return null;
      input = input.Substring(input.IndexOf('(') + 1); 
      input = input.Substring(0, input.LastIndexOf(')')); 
      return Regex.Split(input, ",(?=(?:[^']*'[^']*')*[^']*$)").Select(s => s.Trim().Replace("\"", "").Replace("'", "")).ToArray();
    }
  }
}