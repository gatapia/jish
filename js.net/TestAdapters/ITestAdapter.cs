using System;
using js.net.Engine;

namespace js.net.TestAdapters
{
  public interface ITestAdapter : IDisposable
  {
    void LoadSourceFile(string sourceFile);
    ITestResults RunTest(string testFile);
    IEngine GetInternalEngine();
  }
}