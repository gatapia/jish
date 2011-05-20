using System;
using System.Diagnostics;
using System.IO;
using js.net.Engine;
using js.net.FrameworkAdapters;

namespace js.net.TestAdapters.JsUnit
{
  public class JsUnitTestAdapterFactory : ITestAdapterFactory
  {
    private readonly string jsUnitCoreFile;
    private readonly IEngineFactory engineFactory;

    public JsUnitTestAdapterFactory(string jsUnitCoreFile, IEngineFactory engineFactory)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(jsUnitCoreFile));
      Trace.Assert(File.Exists(jsUnitCoreFile));
      Trace.Assert(engineFactory != null);

      this.jsUnitCoreFile = jsUnitCoreFile;
      this.engineFactory = engineFactory;
    }

    public bool Silent { get; set; }

    public ITestAdapter CreateAdapter()
    {
      SimpleDOMAdapter domAdapter = new SimpleDOMAdapter(engineFactory.CreateEngine());
      JsUnitTestAdapter jsUnit = new JsUnitTestAdapter(domAdapter, jsUnitCoreFile) {Silent = Silent};
      return jsUnit;
    }
  }
}
