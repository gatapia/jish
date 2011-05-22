using js.net.Engine;
using js.net.FrameworkAdapters;

namespace js.net.TestAdapters.JSUnit
{
  public class JSUnitTestAdapterFactory : AbstractTestAdapterFactory
  {
    public JSUnitTestAdapterFactory(string jsUnitCoreFile, IEngineFactory engineFactory) : base(jsUnitCoreFile, engineFactory) {}

    protected override ITestAdapter CreateTestAdapter(IEngine engine, string frameworkJsFile)
    {
      SimpleDOMAdapter domAdapter = new SimpleDOMAdapter(engine);
      return new JSUnitTestAdapter(domAdapter, frameworkJsFile);
    }
  }
}
