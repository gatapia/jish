using System;
using System.Diagnostics;
using System.IO;
using js.net.Engine;
using js.net.FrameworkAdapters;

namespace js.net.TestAdapters.Jasmine
{
  public class JasmineTestAdapterFactory : ITestAdapterFactory
  {
    private readonly string jasmineJsFile;
    private readonly IEngineFactory engineFactory;

    public JasmineTestAdapterFactory(string jasmineJsFile, IEngineFactory engineFactory)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(jasmineJsFile));
      Trace.Assert(File.Exists(jasmineJsFile));
      Trace.Assert(engineFactory != null);

      this.jasmineJsFile = jasmineJsFile;
      this.engineFactory = engineFactory;
    }

    public bool Silent { get; set; }

    public ITestAdapter CreateAdapter()
    {
      SimpleDOMAdapter domAdapter = new SimpleDOMAdapter(engineFactory.CreateEngine());
      JasmineTestAdapter jsUnit = new JasmineTestAdapter(domAdapter, jasmineJsFile) {Silent = Silent};
      return jsUnit;
    }
  }
}
