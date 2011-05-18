using js.net.Engine;
using js.net.FrameworkAdapters;

namespace js.net.TestAdapters.QUnit
{
  public class QUnitTestAdapterFactory : ITestAdapterFactory
  {
    private readonly string qUnitJs;
    private readonly IEngineFactory engineFactory;

    public QUnitTestAdapterFactory(string qUnitJs, IEngineFactory engineFactory)
    {
      this.qUnitJs = qUnitJs;
      this.engineFactory = engineFactory;
    }

    public bool Silent { get; set; }

    public ITestAdapter CreateAdapter()
    {
      SimpleDOMAdapter domAdapter = new SimpleDOMAdapter(engineFactory.CreateEngine());
      QUnitTestAdapter qUnit = new QUnitTestAdapter(domAdapter, qUnitJs) {Silent = Silent};
      return qUnit;
    }
  }
}
