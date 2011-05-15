using System;
using System.Diagnostics;
using System.IO;
using js.Engine;
using PicNet2.Diag;

namespace js.Closure
{
  public class ClosureContext : IEngine
  {
    static ClosureContext()
    {
      ExceptionTraceListener.OverwriteDefaultTraceListener();
    }

    private readonly string baseJsFile;
    private readonly IEngine engine;
    private readonly PathLoader loader = new PathLoader();

    public ClosureContext(string baseJsFile, IEngine engine)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(baseJsFile));
      Trace.Assert(File.Exists(baseJsFile));
      Trace.Assert(engine != null);

      this.baseJsFile = baseJsFile;
      this.engine = engine;
      // TODO: This should go somewhere else.
      InitialiseJSContext();
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

    public void Dispose()
    {
      if (engine != null) engine.Dispose();
    }
       

    private void InitialiseJSContext()
    {
      new ClosureRootDependencies(this, baseJsFile, loader).Initialise();
      LoadJSFile(baseJsFile);
    }   
  }
}