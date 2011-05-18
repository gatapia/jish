using System;
using System.Diagnostics;
using System.IO;
using js.net.Engine;

namespace js.net.FrameworkAdapters
{
  public class SimpleDOMAdapter : IEngine
  {
    protected readonly PathLoader loader = new PathLoader();
    protected IEngine engine;

    public SimpleDOMAdapter(IEngine engine)
    {
      Trace.Assert(engine != null);

      this.engine = engine;                  
    }

    public virtual void Initialise()
    {
      LoadJSFile(@"C:\dev\projects\labs\js.net\lib\dom\env.therubyracer.js");
      LoadJSFile(@"C:\dev\projects\labs\js.net\lib\dom\window.js");

      engine.SetGlobal("global", new JSGlobal());
      engine.SetGlobal("console", new JSConsole());
    }
    
    public object LoadJSFile(string file)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(file));
      Trace.Assert(File.Exists(file), "Could not find file: " + file);

      string scriptContent = loader.LoadScriptContent(file);
      if (String.IsNullOrWhiteSpace(scriptContent)) return null;
      
      try
      {
        return Run(scriptContent);
      } catch (Exception e)
      {
        Console.WriteLine("Error running file: " + file + " - " + e.Message);
        throw;
      }
    }

    public object Run(string script)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(script), "Script is empty");

      return engine.Run(script);
    }

    public void SetGlobal(string name, object value)
    {
      engine.SetGlobal(name, value);
    }

    public object GetGlobal(string name)
    {
      return engine.GetGlobal(name);
    }

    public void Dispose()
    {
      if (engine != null) engine.Dispose();
    }
  }
}