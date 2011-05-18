using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace js.net.TestAdapters
{
  public class TestResultsStringFormatter
  {
    private readonly string testName;
    private readonly IList<KeyValuePair<string, bool>> allTests;

    public TestResultsStringFormatter(string testName, IList<KeyValuePair<string, bool>> allTests)
    {
      this.testName = testName;
      this.allTests = allTests;
    }

    public string FormatTestResults()
    {
      StringBuilder sb = new StringBuilder("\n");
      sb.Append(testName).Append(" Results (").Append(allTests.Where(t => t.Value).Count()).Append("/").Append(allTests.Count).Append(")\n");
      sb.Append(new String('=', 52)).Append('\n');
      sb.Append(String.Format("|{0,-30}|{1,-9}|{2,-9}|\n", FormatString("Test Name", 30), FormatString("Passed", 9), FormatString("Failed", 9)));
      sb.Append(new String('-', 52)).Append('\n');
      foreach (var t in allTests)
      {
        sb.Append(CreateRow(t.Key, t.Value ? "X" : "", t.Value ? "" : "X"));
      }      
      sb.Append(new String('-', 52));
      return sb.ToString();
    }

    private string CreateRow(string testName, string passed, string failed)
    {
      return String.Format("|{0,30}|{1,-9}|{2,-9}|\n", FormatString(testName, 30), FormatString(passed, 9), FormatString(failed, 9));
    }

    private string FormatString(string str, int len)
    {
      if (str.Length >= len) return str.Substring(0, len);
      int halfDiff = (len - str.Length) / 2;
      str = (new String(' ', halfDiff) + str);
      return str;
    }
  }
}
