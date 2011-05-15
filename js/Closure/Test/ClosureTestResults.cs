using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PicNet2;

namespace js.Closure.Test
{
  public class ClosureTestResults
  {
    public string TestName { get; private set; }
    public IList<string> Passed { get; private set; }
    public IList<string> Failed { get; private set; }

    public ClosureTestResults(string testName)
    {
      TestName = testName;
      Passed = new List<string>();
      Failed = new List<string>();
    }

    public void AddPassedTest(string testName)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(testName));

      Passed.Add(testName);
    }

    public void AddFailedTest(string testName)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(testName));

      Failed.Add(testName);
    }

    public override string ToString()
    {
      string passedstr = String.Empty;
      string failedstr = String.Empty;
      if (Passed.Any()) passedstr = "\nPASSED:\n\t" + CollectionUtils.ToString(Passed, "\n\t");
      if (Failed.Any()) failedstr = "\nFAILED:\n\t" + CollectionUtils.ToString(Failed, "\n\t");
      return "RESULTS\n=======\n" + passedstr + failedstr;
    }
  }
}