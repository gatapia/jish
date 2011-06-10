using System;

namespace js.net.TestAdapters.Coverage
{
  public interface ICoverageAdapter : IDisposable
  {
    void LoadSourceFile(string sourceFile);
    ICoverageResults RunCoverage(string testFile);
  }
}