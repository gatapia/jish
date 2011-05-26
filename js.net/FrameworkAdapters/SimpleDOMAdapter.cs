using System;
using System.Diagnostics;
using System.IO;
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
      fileLoader = new CWDFileLoader();
    }

    public virtual void Initialise()
    {
      EmbeddedResourcesUtils embedded = new EmbeddedResourcesUtils();
      Run(embedded.ReadEmbeddedResourceTextContents("js.net.resources.env.therubyracer.js"), "js.net.resources.env.therubyracer.js");
      Run(embedded.ReadEmbeddedResourceTextContents("js.net.resources.window.js"), "js.net.resources.window.js");

      new JSGlobal(engine, fileLoader, new JSConsole(engine)).BindToGlobalScope();      
    }

    public object LoadJSFile(string file, bool setCwd)
    {      
      object returnValue = engine.Run(fileLoader.GetFilePathFromCwdIfRequired(file, setCwd), new FileInfo(file).Name);
      if (setCwd) fileLoader.ScriptFinnished();
      return returnValue;
    }

    public object Run(string script, string fileName)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(script), "Script is empty");

      return engine.Run(script, fileName);
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