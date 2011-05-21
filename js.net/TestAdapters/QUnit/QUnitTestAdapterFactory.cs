using js.net.Engine;
using js.net.FrameworkAdapters;

namespace js.net.TestAdapters.QUnit
{
  public class QUnitTestAdapterFactory : AbstractTestAdapterFactory
  {

    public QUnitTestAdapterFactory(string qUnitJs, IEngineFactory engineFactory) : base(qUnitJs, engineFactory) { }

    protected override ITestAdapter CreateTestAdapter(IEngine engine, string frameworkJsFile)
    {
      SimpleDOMAdapter domAdapter = new SimpleDOMAdapter(engine);
      return new QUnitTestAdapter(domAdapter, frameworkJsFile);
    }
  }
}
