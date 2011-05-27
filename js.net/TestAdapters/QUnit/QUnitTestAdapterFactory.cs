using js.net.Engine;
using js.net.FrameworkAdapters;

namespace js.net.TestAdapters.QUnit
{
  public class QUnitTestAdapterFactory : AbstractTestAdapterFactory
  {

    public QUnitTestAdapterFactory(string qUnitJs, IEngineFactory engineFactory) : base(qUnitJs, engineFactory) { }

    protected override ITestAdapter CreateTestAdapter(IEngine engine, string frameworkJsFile)
    {
      JSDomAdapter domAdapter = new JSDomAdapter(engine);
      domAdapter.Initialise();
      return new QUnitTestAdapter(domAdapter, frameworkJsFile);
    }
  }
}
