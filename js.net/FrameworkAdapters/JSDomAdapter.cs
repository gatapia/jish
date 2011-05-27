using System;
using System.Diagnostics;
using System.IO;
using js.net.Engine;

namespace js.net.FrameworkAdapters
{
  public class JSDomAdapter : IEngine
  {
    protected readonly PathLoader pathLoader = new PathLoader();
    protected readonly CWDFileLoader fileLoader;
    protected IEngine engine;

    public JSDomAdapter(IEngine engine)
    {
      Trace.Assert(engine != null);

      this.engine = engine;
      fileLoader = new CWDFileLoader();
    }

    public virtual void Initialise()
    {
      new JSGlobal(engine, fileLoader, new JSConsole()).BindToGlobalScope();
      LoadJSFile("js.net.resources.dom.jsdom.lib.jsdom.js", true);
    }

    public object LoadJSFile(string file, bool setCwd)
    {      
      object returnValue = engine.Run(fileLoader.GetFileContentFromCwdIfRequired(file, setCwd), new FileInfo(file).Name);
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