using js.net.Engine;
using js.net.FrameworkAdapters;

namespace js.net.TestAdapters.Jasmine
{
  public class JasmineTestAdapterFactory : AbstractTestAdapterFactory
  {
    public JasmineTestAdapterFactory(string jasmineJsFile, IEngineFactory engineFactory) : base(jasmineJsFile, engineFactory) { }

    protected override ITestAdapter CreateTestAdapter(IEngine engine, string frameworkJsFile)
    {
      JSDomAdapter domAdapter = new JSDomAdapter(engine);
      domAdapter.Initialise();
      return new JasmineTestAdapter(domAdapter, frameworkJsFile);
    }
  }
}
