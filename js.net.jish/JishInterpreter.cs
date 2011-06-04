using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using js.net.Engine;
using js.net.jish.InlineCommand;
using js.net.jish.Util;
using js.net.Util;
using Noesis.Javascript;

namespace js.net.jish
{
  public class JishInterpreter : IJishInterpreter
  {    

    private readonly JSConsole console;
    private readonly IEngine engine;
    private readonly LoadedAssembliesBucket loadedAssemblies;
    private readonly EmbeddedResourcesUtils embeddedResourceLoader;

    private string bufferedCommand = String.Empty;

    public JishInterpreter(IEngine engine, JSConsole console, LoadedAssembliesBucket loadedAssemblies, EmbeddedResourcesUtils embeddedResourceLoader)
    {
      Trace.Assert(engine != null);
      Trace.Assert(console != null);

      this.engine = engine;
      this.embeddedResourceLoader = embeddedResourceLoader;
      this.loadedAssemblies = loadedAssemblies;
      this.console = console;           
    }

    public void InitialiseDependencies()
    {
      engine.Run(embeddedResourceLoader.ReadEmbeddedResourceTextContents("js.net.jish.resources.jish.js", GetType().Assembly), "jish.js");

      Assembly[] assemblies = LoadAllAssemblies().Distinct(new AssemblyNameComparer()).ToArray();
      Array.ForEach(assemblies, LoadCommandsFromAssembly);      
      
      LoadJavaScriptModules();
    }

    private void LoadCommandsFromAssembly(Assembly assembly)
    {
      IEnumerable<IInlineCommand> commands = loadedAssemblies.AddAssembly(assembly);
      if (commands.Any()) InjectCommands(commands);
    }

    private void InjectCommands(IEnumerable<IInlineCommand> commands)
    {
      var nsCommands = commands.GroupBy(c => c.GetNameSpace());
      foreach (var commandsInNamespace in nsCommands)
      {
        if (commandsInNamespace.Count() == 1)
        {
          Console.WriteLine("SINGLE: engine.SetGlobal [" + commandsInNamespace.Key +"] [1]");
          engine.SetGlobal(commandsInNamespace.Key, commandsInNamespace.First());
        } else
        {
          var methods = new List<ProxyMethod>();
          foreach (var command in commandsInNamespace)
          {
            var thisMethods = command.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(m => Char.IsLower(m.Name[0]));
            foreach (MethodInfo mi in thisMethods)
            {
              methods.Add(new ProxyMethod(mi, command));
            }
          }
          var nsWrapper = new TypeILWrapper().CreateWrapper(commandsInNamespace.First().GetType(), methods.ToArray());
          Console.WriteLine("MULTIPLE: engine.SetGlobal [" + commandsInNamespace.Key +"] [" + String.Join(", ",  methods.Select(m => m.MethodInfo.Name)) + "]");
          engine.SetGlobal(commandsInNamespace.Key, nsWrapper);
        }
      }
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
        loadedAssemblies.InterceptSpecialCommands(input);
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
      RunFileImpl(file, lines);
      Directory.SetCurrentDirectory(cwd);
    }    

    private void RunFileImpl(string file, IEnumerable<string> lines)
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
  }
}