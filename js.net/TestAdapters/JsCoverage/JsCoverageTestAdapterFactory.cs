using System.Diagnostics;

namespace js.net.TestAdapters.JSCoverage
{
  public class JSCoverageTestAdapterFactory : ICoverageAdapterFactory
  {
    private readonly ITestAdapterFactory testAdapterFactory;

    public JSCoverageTestAdapterFactory(ITestAdapterFactory testAdapterFactory)
    {
      Trace.Assert(testAdapterFactory != null);

      this.testAdapterFactory = testAdapterFactory;
    }

    public ICoverageAdapter CreateAdapter()
    {
      return new JSCoverageTestAdapter(testAdapterFactory.CreateAdapter());
    }
  }
}
