using System.Collections.Generic;
using System.Diagnostics;
using js.net.Util;

namespace js.net.TestAdapters.Coverage.JSCoverage
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
      Trace.Assert(testAdapter != null);
      Trace.Assert(sourceFile != null);

      testAdapter.LoadSourceFile(sourceFile);
    }

    public ICoverageResults RunCoverage(string testFile)
    {
      Trace.Assert(testAdapter != null);
      Trace.Assert(testFile != null);

      ITestResults results = testAdapter.RunTest(testFile);      
      testAdapter.GetFrameworkAdapter().Run(new EmbeddedResourcesUtils().ReadEmbeddedResourceTextContents(@"js.net.resources.jscoverage.parser.js"), "js.net.resources.jscoverage.parser.js");
      IDictionary<string, object> rawCoverageResults = (IDictionary<string, object>) testAdapter.GetFrameworkAdapter().GetGlobal("coverageResults");
      TotalCoverageResults totalCoverageResults = new TotalCoverageResults(results, rawCoverageResults);
      totalCoverageResults.ParseCoverageResults();
      return totalCoverageResults;
    }

    public void Dispose()
    {
      Trace.Assert(testAdapter != null);

      testAdapter.Dispose();
    }
  }
}