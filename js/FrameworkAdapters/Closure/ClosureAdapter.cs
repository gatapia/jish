using System;
using System.Diagnostics;
using System.IO;
using js.net.Engine;

namespace js.net.FrameworkAdapters.Closure
{
  public class ClosureAdapter : AbstractFrameworkAdapter
  {    
    private readonly string baseJsFile;

    public ClosureAdapter(string baseJsFile, IEngine engine) : base(engine)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(baseJsFile));
      Trace.Assert(File.Exists(baseJsFile));

      this.baseJsFile = baseJsFile;
      
      InitialiseJSContext();
    }

    private void InitialiseJSContext()
    {
      new ClosureRootDependencies(this, baseJsFile, loader).Initialise();
      LoadJSFile(baseJsFile);
    }   
  }
}