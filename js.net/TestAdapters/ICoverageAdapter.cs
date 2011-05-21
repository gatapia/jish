using System;

namespace js.net.TestAdapters
{
  public interface ICoverageAdapter : IDisposable
  {
    void LoadSourceFile(string sourceFile);
    ICoverageResults RunCoverage(string testFile);
  }
}