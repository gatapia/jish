using System.Collections.Generic;

namespace js.net.TestAdapters
{
  public interface ITestResults
  {
    string FixtureName { get; }
    IEnumerable<string> Passed { get; }
    IEnumerable<string> Failed { get; }
  }
}