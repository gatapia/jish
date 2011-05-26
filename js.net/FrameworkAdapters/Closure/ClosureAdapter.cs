using System;
using System.Diagnostics;
using System.IO;
using js.net.Engine;

namespace js.net.FrameworkAdapters.Closure
{
  public class ClosureAdapter : JSDomAdapter
  {    
    private readonly string baseJsFile;

    public ClosureAdapter(string baseJsFile, string jsDomSourceFile, IEngine engine) : base(engine, jsDomSourceFile)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(baseJsFile));
      Trace.Assert(File.Exists(baseJsFile));

      this.baseJsFile = baseJsFile;      
    }

    public override void Initialise()
    {
      base.Initialise();

      new ClosureDependencies(this, baseJsFile, pathLoader).Initialise();
    }
  }
}