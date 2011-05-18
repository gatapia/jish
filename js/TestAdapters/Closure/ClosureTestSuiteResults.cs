using System;
using System.Collections.Generic;
using System.Linq;

namespace js.net.TestAdapters.Closure
{
  public class ClosureTestSuiteResults : ClosureTestResults
  {
    public ClosureTestSuiteResults(IEnumerable<ClosureTestResults> individualResults) : base("Test Suite")
    {
      Array.ForEach(individualResults.ToArray(), ExtractResultsFromTest);
    }

    private void ExtractResultsFromTest(ClosureTestResults result)
    {
      Array.ForEach(result.Passed.ToArray(), name => AddPassedTest(result.TestName + " - " + name));
      Array.ForEach(result.Failed.ToArray(), name => AddFailedTest(result.TestName + " - " + name));
    }
  }
}