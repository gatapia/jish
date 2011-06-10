using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace js.net.TestAdapters.Coverage
{
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