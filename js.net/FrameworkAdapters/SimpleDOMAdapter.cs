using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using js.net.Engine;
using js.net.Util;

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
      EmbeddedResourcesUtils embedded = new EmbeddedResourcesUtils();
      Run(embedded.ReadEmbeddedResourceTextContents("js.net.resources.env.therubyracer.js"));
      Run(embedded.ReadEmbeddedResourceTextContents("js.net.resources.window.js"));

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
      Trace.Assert(!String.IsNullOrWhiteSpace(name));

      engine.SetGlobal(name, value);
    }

    public object GetGlobal(string name)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(name));

      return engine.GetGlobal(name);
    }

    public void Dispose()
    {
      if (engine != null) engine.Dispose();
    }
  }
}