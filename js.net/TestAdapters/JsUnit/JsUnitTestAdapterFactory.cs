using js.net.Engine;
using js.net.FrameworkAdapters;

namespace js.net.TestAdapters.JsUnit
{
  public class JsUnitTestAdapterFactory : AbstractTestAdapterFactory
  {
    public JsUnitTestAdapterFactory(string jsUnitCoreFile, IEngineFactory engineFactory) : base(jsUnitCoreFile, engineFactory) {}

    protected override ITestAdapter CreateTestAdapter(IEngine engine, string frameworkJsFile)
    {
      SimpleDOMAdapter domAdapter = new SimpleDOMAdapter(engine);
      return new JsUnitTestAdapter(domAdapter, frameworkJsFile);
    }
  }
}
