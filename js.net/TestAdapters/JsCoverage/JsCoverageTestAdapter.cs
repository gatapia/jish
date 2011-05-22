using System.Collections.Generic;
using System.Diagnostics;
using js.net.Engine;
using js.net.Util;

namespace js.net.TestAdapters.JSCoverage
{
  public class JSCoverageTestAdapter : ICoverageAdapter
  {
    private readonly ITestAdapter testAdapter;

    public JSCoverageTestAdapter(ITestAdapter testAdapter)
    {
      Trace.Assert(testAdapter != null);

      this.testAdapter = testAdapter;
    }

    public void LoadSourceFile(string sourceFile)
    {
      testAdapter.LoadSourceFile(sourceFile);
    }

    public ICoverageResults RunCoverage(string testFile)
    {
      ITestResults results = testAdapter.RunTest(testFile);      
      testAdapter.GetInternalEngine().Run(new EmbeddedResourcesUtils().ReadEmbeddedResourceTextContents(@"js.net.resources.jscoverage.parser.js"));
      IDictionary<string, object> rawCoverageResults = (IDictionary<string, object>) testAdapter.GetInternalEngine().GetGlobal("coverageResults");
      TotalCoverageResults totalCoverageResults = new TotalCoverageResults(results, rawCoverageResults);
      totalCoverageResults.ParseCoverageResults();
      return totalCoverageResults;
    }

    public void Dispose()
    {
      testAdapter.Dispose();
    }
  }
}