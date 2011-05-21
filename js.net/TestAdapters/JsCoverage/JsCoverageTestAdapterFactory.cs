using System.Diagnostics;

namespace js.net.TestAdapters.JsCoverage
{
  public class JsCoverageTestAdapterFactory : ICoverageAdapterFactory
  {
    private readonly ITestAdapterFactory testAdapterFactory;

    public JsCoverageTestAdapterFactory(ITestAdapterFactory testAdapterFactory)
    {
      Trace.Assert(testAdapterFactory != null);

      this.testAdapterFactory = testAdapterFactory;
    }

    public bool Silent { get; set; }

    public ICoverageAdapter CreateAdapter()
    {
      return new JsCoverageTestAdapter(testAdapterFactory.CreateAdapter());
    }
  }
}
