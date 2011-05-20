using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace js.net.TestAdapters
{
  public class TestResultsStringFormatter
  {
    private readonly string fixtureName;
    private readonly IList<KeyValuePair<string, bool>> allTests;

    public TestResultsStringFormatter(string fixtureName, IList<KeyValuePair<string, bool>> allTests)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(fixtureName));    
      Trace.Assert(allTests != null);    

      this.fixtureName = fixtureName;
      this.allTests = allTests;
    }

    public string FormatTestResults()
    {
      StringBuilder sb = new StringBuilder("\n");
      sb.Append(fixtureName).Append(" Results (").Append(allTests.Where(t => t.Value).Count()).Append("/").Append(allTests.Count).Append(")\n");
      sb.Append(new String('=', 102)).Append('\n');
      sb.Append(String.Format("|{0,-80}|{1,-9}|{2,-9}|\n", FormatString("Test Name", 80), FormatString("Passed", 9), FormatString("Failed", 9)));
      sb.Append(new String('-', 102)).Append('\n');
      foreach (var t in allTests)
      {
        sb.Append(CreateRow(t.Key, t.Value ? "X" : "", t.Value ? "" : "X"));
      }      
      sb.Append(new String('-', 102));
      return sb.ToString();
    }

    private string CreateRow(string testName, string passed, string failed)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(testName));    
      Trace.Assert(passed != null);          
      Trace.Assert(failed != null);          

      return String.Format("|{0,80}|{1,-9}|{2,-9}|\n", FormatString(testName, 80), FormatString(passed, 9), FormatString(failed, 9));
    }

    private string FormatString(string str, int len)
    {
      Trace.Assert(str != null);          
      Trace.Assert(len > 0);          

      if (str.Length >= len) return str.Substring(0, len);
      int halfDiff = (len - str.Length) / 2;
      str = (new String(' ', halfDiff) + str);
      return str;
    }
  }
}
