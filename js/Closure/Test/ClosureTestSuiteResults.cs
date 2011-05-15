using System.Collections.Generic;
using PicNet2;

namespace js.Closure.Test
{
  public class ClosureTestSuiteResults : ClosureTestResults
  {
    public ClosureTestSuiteResults(IEnumerable<ClosureTestResults> individualResults) : base("Test Suite")
    {
      CollectionUtils.ForEach(individualResults, ExtractResultsFromTest);
    }

    private void ExtractResultsFromTest(ClosureTestResults result)
    {
      CollectionUtils.ForEach(result.Passed, name => AddPassedTest(result.TestName + " - " + name));
      CollectionUtils.ForEach(result.Failed, name => AddFailedTest(result.TestName + " - " + name));
    }
  }
}