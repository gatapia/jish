using System;
using System.Collections.Generic;
using System.Linq;

namespace js.net.TestAdapters
{
  public class TestSuiteResults : TestResults
  {
    public TestSuiteResults(IEnumerable<TestResults> individualResults) : base("Test Suite")
    {
      Array.ForEach(individualResults.ToArray(), ExtractResultsFromTest);
    }

    private void ExtractResultsFromTest(TestResults result)
    {
      Array.ForEach(result.Passed.ToArray(), name => AddPassedTest(result.TestName + " - " + name));
      Array.ForEach(result.Failed.ToArray(), name => AddFailedTest(result.TestName + " - " + name));
    }
  }
}