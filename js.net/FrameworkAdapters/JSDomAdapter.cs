using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using js.net.Engine;
using js.net.Util;

namespace js.net.FrameworkAdapters
{
  public class JSDomAdapter : SimpleDOMAdapter
  {
    private readonly string jsDomSrcFile;

    public JSDomAdapter(IEngine engine, string jsDomSrcFile) : base(engine)
    {
      Trace.Assert(engine != null);

      this.jsDomSrcFile = jsDomSrcFile;
    }

    public override void Initialise()
    {
      new JSConsole(engine);
      new JSGlobal(engine, fileLoader).BindToGlobalScope();
      LoadJSFile(jsDomSrcFile);
    }    
  }
}