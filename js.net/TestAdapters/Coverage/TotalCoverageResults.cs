using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace js.net.TestAdapters.Coverage
{
  public class TotalCoverageResults : CoverageResultsBase, ICoverageResults
  {
    private readonly ITestResults testResults;
    private readonly IDictionary<string, object> coverageResults;

    public TotalCoverageResults(ITestResults testResults, IDictionary<string, object> coverageResults) 
    {
      Trace.Assert(testResults != null);
      Trace.Assert(coverageResults != null);

      this.testResults = testResults;
      this.coverageResults = coverageResults;
    }

    public void ParseCoverageResults()
    {      
      Trace.Assert(coverageResults != null);

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
      get
      {
        Trace.Assert(testResults != null);

        return testResults.FixtureName;
      }
    }

    public IEnumerable<string> Passed
    {
      get
      {
        Trace.Assert(testResults != null);

        return testResults.Passed;
      }
    }

    public IEnumerable<string> Failed
    {
      get
      {
        Trace.Assert(testResults != null);

        return testResults.Failed;
      }
    }

    public int FilesCount { get
    {
      Trace.Assert(FileResults != null);
      return FileResults.Count();
    } }

    public IEnumerable<IFileCoverageResults> FileResults { get; private set; }

    public override string ToString()
    {
      return new TestCoverageFormatter(this).FormatCoverageResults();
    }
  }
}