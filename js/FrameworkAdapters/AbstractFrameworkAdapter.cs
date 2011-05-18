using System;
using System.Diagnostics;
using System.IO;
using js.net.Engine;
using js.net.FrameworkAdapters.Closure;

namespace js.net.FrameworkAdapters
{
  public abstract class AbstractFrameworkAdapter : IEngine
  {
    protected IEngine engine;
    protected readonly PathLoader loader = new PathLoader();

    protected AbstractFrameworkAdapter(IEngine engine)
    {
      Trace.Assert(engine != null);

      this.engine = engine;
      
      // Bring the engine back to a 'new' state (allows us to reuse an engine)
      engine.Reset();
    }

    public object LoadJSFile(string file)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(file));
      Trace.Assert(File.Exists(file), "Could not find file: " + file);

      string scriptContent = loader.LoadScriptContent(file);
      return scriptContent == null ? null : Run(scriptContent);
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

    public void Reset()
    {
      engine.Reset();
    }

    public void Dispose()
    {
      if (engine != null) engine.Dispose();
    }
  }
}