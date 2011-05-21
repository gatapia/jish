using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace js.net.TestAdapters
{
  public class TestSuiteResults : TestResults
  {
    public TestSuiteResults(IEnumerable<ITestResults> individualResults) : base("Test Suite")
    {
      Trace.Assert(individualResults != null);

      Array.ForEach(individualResults.ToArray(), ExtractResultsFromTest);
    }

    private void ExtractResultsFromTest(ITestResults result)
    {
      Trace.Assert(result != null);
      Trace.Assert(!String.IsNullOrWhiteSpace(result.FixtureName));

      Array.ForEach(result.Passed.ToArray(), name => AddPassedTest(result.FixtureName + " - " + name));
      Array.ForEach(result.Failed.ToArray(), name => AddFailedTest(result.FixtureName + " - " + name));
    }
  }
}