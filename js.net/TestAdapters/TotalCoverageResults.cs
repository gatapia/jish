using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace js.net.TestAdapters
{
  public class TotalCoverageResults : CoverageResultsBase, ICoverageResults
  {
    private readonly ITestResults testResults;
    private readonly IDictionary<string, object> coverageResults;

    public TotalCoverageResults(ITestResults testResults, IDictionary<string, object> coverageResults) 
    {
      this.testResults = testResults;
      this.coverageResults = coverageResults;
    }

    public void ParseCoverageResults()
    {
      Statements = (int) coverageResults["totalStatements"];
      Executed = (int) coverageResults["totalExecuted"];

      object[] files = (object[]) coverageResults["files"];
      IList<IFileCoverageResults> fileResults = new List<IFileCoverageResults>();
      foreach(IDictionary<string, object> file in files)
      {
        fileResults.Add(new FileCoverageResults
                          {
                            FileName = (string) file["fileName"], 
                            Statements = (int) file["statements"], 
                            Executed = (int) file["executed"]
                          });
      }
      FileResults = fileResults;
    }

    public string FixtureName
    {
      get { return testResults.FixtureName; }
    }

    public IEnumerable<string> Passed
    {
      get { return testResults.Passed; }
    }

    public IEnumerable<string> Failed
    {
      get { return testResults.Failed; }
    }

    public int FilesCount { get { return FileResults.Count(); } }

    public IEnumerable<IFileCoverageResults> FileResults { get; private set; }

    public override string ToString()
    {
      return new TestCoverageFormatter(this).FormatCoverageResults();
    }
  }

  public class TestCoverageFormatter
  {
    private readonly TotalCoverageResults results;

    public TestCoverageFormatter(TotalCoverageResults results)
    {
      Trace.Assert(results != null);

      this.results = results;
    }

    public string FormatCoverageResults()
    {
      Trace.Assert(results != null);

      StringBuilder str = new StringBuilder("\nCoverage Results [");
      str.Append(results.FixtureName).Append(" (").
        Append(results.Passed.Count()).Append(" / ").Append(results.Passed.Count() + results.Failed.Count()).
      Append(")]");
      str.Append("\n" + new String('=', str.Length) + "\n");
      str.Append("Total Files: ").Append(results.FilesCount).Append("\n");
      str.Append("Total Statements: ").Append(results.Statements).Append("\n");
      str.Append("Total Executed: ").Append(results.Executed).Append("\n");
      str.Append("Total Coverage (%): ").Append(results.CoveragePercentage).Append("\n");
      foreach (IFileCoverageResults fileResults in results.FileResults)
      {
        str.Append("\n\tFile: ").Append(fileResults.FileName);
        str.Append("\n\t\tStatements[").Append(fileResults.Statements).
          Append("] Executed[").Append(fileResults.Executed).Append("] Coverage (%)[").Append(fileResults.CoveragePercentage).Append("]");
      }
      return str.ToString();
    }
  }
}