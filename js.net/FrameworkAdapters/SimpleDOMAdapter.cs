using System;
using System.Diagnostics;
using js.net.Engine;
using js.net.Util;

namespace js.net.FrameworkAdapters
{
  public class SimpleDOMAdapter : IEngine
  {
    protected readonly PathLoader pathLoader = new PathLoader();
    protected readonly CWDFileLoader fileLoader;
    protected IEngine engine;

    public SimpleDOMAdapter(IEngine engine)
    {
      Trace.Assert(engine != null);

      this.engine = engine;
      fileLoader = new CWDFileLoader(engine);
    }

    public virtual void Initialise()
    {
      EmbeddedResourcesUtils embedded = new EmbeddedResourcesUtils();
      Run(embedded.ReadEmbeddedResourceTextContents("js.net.resources.env.therubyracer.js"));
      Run(embedded.ReadEmbeddedResourceTextContents("js.net.resources.window.js"));

      new JSGlobal(engine, fileLoader).BindToGlobalScope();
      new JSConsole(engine);
    }

    public object LoadJSFile(string file)
    {
      object returnValue = engine.Run(fileLoader.LoadJSFile(file));
      fileLoader.ScriptFinnished();
      return returnValue;
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