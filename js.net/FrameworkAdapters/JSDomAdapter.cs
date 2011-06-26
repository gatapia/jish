using System;
using System.Diagnostics;
using System.IO;
using js.net.Engine;
using js.net.Util;
using Ninject;

namespace js.net.FrameworkAdapters
{
  public class JSDomAdapter : IFrameworkAdapter, IInitializable
  {
    protected readonly CWDFileLoader fileLoader;
    protected readonly IEngine engine;
    private readonly JSGlobal jsGlobal;

    public JSDomAdapter(IEngine engine, CWDFileLoader fileLoader, JSGlobal jsGlobal)
    {
      Trace.Assert(engine != null);
      Trace.Assert(fileLoader != null);

      this.engine = engine;
      this.jsGlobal = jsGlobal;
      this.fileLoader = fileLoader;            
    }

    public virtual void Initialize()
    {
      Trace.Assert(engine != null);
      Trace.Assert(fileLoader != null);

      fileLoader.ResetCwd();
      jsGlobal.BindToGlobalScope(engine);
      LoadJSFile("js.net.resources.dom.jsdom.lib.jsdom.js", true);
      fileLoader.ResetCwd();
    }

    public object LoadJSFile(string file, bool setCwd)
    {      
      Trace.Assert(!String.IsNullOrWhiteSpace(file));
      Trace.Assert(engine != null);
      Trace.Assert(fileLoader != null);

      object returnValue = engine.Run(fileLoader.GetFileContentFromCwdIfRequired(file, setCwd), new FileInfo(file).Name);
      if (setCwd) fileLoader.ScriptFinnished();
      return returnValue;
    }

    public object Run(string script, string fileName)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(script), "Script is empty");
      Trace.Assert(fileName != null);
      Trace.Assert(engine != null);

      return engine.Run(script, fileName);
    }

    public void SetGlobal(string name, object value)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(name));
      Trace.Assert(engine != null);

      engine.SetGlobal(name, value);
    }

    public object GetGlobal(string name)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(name));
      Trace.Assert(engine != null);

      return engine.GetGlobal(name);
    }

    public void Dispose()
    {
      Trace.Assert(engine != null);

      engine.Dispose();
    }
  }
}