using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using js.net.Engine;
using js.net.jish.Util;
using js.net.Util;
using Ninject;
using Noesis.Javascript;

namespace js.net.jish
{
  public static class DefaultJishInterpreterFactory
  {
    static DefaultJishInterpreterFactory()
    {
      AppDomain.CurrentDomain.AssemblyResolve += EmbeddedAssemblyLoader.OnAssemblyResolve;
    } 

    public static IJishInterpreter CreateInterpreter()
    {
      IKernel kernel = new StandardKernel(new NinjectSettings { UseReflectionBasedInjection = true });
      IEngine engine = new JSNetEngine();
      JSConsole console = new JSConsole();
      engine.SetGlobal("console", console);
      
      kernel.Bind<IEngine>().ToConstant(engine);
      kernel.Bind<JSConsole>().ToConstant(console);
      kernel.Bind<IJishInterpreter>().To<JishInterpreter>().InSingletonScope();
      kernel.Bind<ICurrentContextAssemblies>().To<CurrentContextAssemblies>().InSingletonScope();
      kernel.Bind<LoadedAssembliesBucket>().ToSelf().InSingletonScope();

      return kernel.Get<IJishInterpreter>();
    }
  }

  public class JishInterpreter : IJishInterpreter, IInitializable
  {    
    private readonly JSConsole console;
    private readonly IEngine engine;
    private readonly LoadedAssembliesBucket loadedAssemblies;
    private readonly EmbeddedResourcesUtils embeddedResourceLoader;
    private readonly AssemblyCommandLoader assemblyCommandsLoader;
    private readonly ICurrentContextAssemblies currentContextAssemblies;

    private string bufferedCommand = String.Empty;

    public JishInterpreter(IEngine engine, JSConsole console, LoadedAssembliesBucket loadedAssemblies, EmbeddedResourcesUtils embeddedResourceLoader, AssemblyCommandLoader assemblyCommandsLoader, ICurrentContextAssemblies currentContextAssemblies)
    {
      Trace.Assert(engine != null);
      Trace.Assert(console != null);
      Trace.Assert(loadedAssemblies != null);
      Trace.Assert(embeddedResourceLoader != null);
      Trace.Assert(assemblyCommandsLoader != null);
      Trace.Assert(currentContextAssemblies != null);

      this.engine = engine;
      this.currentContextAssemblies = currentContextAssemblies;
      this.assemblyCommandsLoader = assemblyCommandsLoader;
      this.embeddedResourceLoader = embeddedResourceLoader;
      this.loadedAssemblies = loadedAssemblies;
      this.console = console;           
    }

    public void Initialize()
    {
      LoadJishJS();
      LoadCommandsFromIncludedAssemblies();
      LoadJavaScriptModules();
      ConfigureCommandLineInputConsole();
    }

    private void LoadJishJS()
    {
      engine.Run(embeddedResourceLoader.ReadEmbeddedResourceTextContents("js.net.jish.resources.jish.js", GetType().Assembly), "jish.js");                  
    }    

    private void LoadCommandsFromIncludedAssemblies()
    {
      Assembly[] assemblies = currentContextAssemblies.GetAllAssemblies().ToArray();
      Array.ForEach(assemblies, assemblyCommandsLoader.LoadCommandsFromAssemblyAndInjectIntoGlobalScope);      
    }

    private void ConfigureCommandLineInputConsole()
    {
      try
      {
        Console.TreatControlCAsInput = false;
        Console.CancelKeyPress += (s, e) => Environment.Exit(0);
      } catch {} // Ignore, as this throws when running in a Process (tests)
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
      Trace.Assert(input != null);
      
      if (String.IsNullOrWhiteSpace(input)) return;

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
            throw ex2.InnerException ?? ex2;
          }
          return false;
        }
      }
      return true;
    }

    public void RunFile(string file, string[] args = null)
    {
      Trace.Assert(file != null);
      Trace.Assert(File.Exists(file));

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
      Trace.Assert(file != null);
      Trace.Assert(lines != null);

      // Do not loose line numbers
      IEnumerable<string> nonCommands = lines.Select(l => l.StartsWith(".") ? "" : l); 
      string nonCommandsInline = String.Join("\n", nonCommands);
      engine.Run(nonCommandsInline, new FileInfo(file).Name);
    }

    public void ClearBufferedCommand()
    {
      bufferedCommand = String.Empty;      
    }

    public IEngine GetEngine() { return engine; }

    public void SetGlobal(string name, object value)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(name));

      engine.SetGlobal(name, value);
    }
    
    public bool ThrowErrors { get; set; }

    private void PrintExceptionMessage(Exception e)
    {
      Trace.Assert(e != null);

      string msg = e.Message;
      if (msg.IndexOf(": ") > 0) msg = msg.Substring(msg.IndexOf(": ") + 2);
      if (msg.IndexOf('(') > 0) msg = msg.Substring(0, msg.IndexOf('('));

      console.log(msg);
    }    
  }
}