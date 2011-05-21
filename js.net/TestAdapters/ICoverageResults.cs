using System.Collections.Generic;

namespace js.net.TestAdapters
{
  public interface ICoverageResults : ITestResults
  {
    IEnumerable<IFileCoverageResults> FileResults { get; }    

    int FilesCount { get; }    

    int Statements { get; }
    int Executed { get; }
    decimal CoveragePercentage { get; }    
  }
}