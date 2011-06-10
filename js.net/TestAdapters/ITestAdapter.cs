using System;
using js.net.FrameworkAdapters;

namespace js.net.TestAdapters
{
  public interface ITestAdapter : IDisposable
  {
    void LoadSourceFile(string sourceFile);
    ITestResults RunTest(string testFile);
    IFrameworkAdapter GetFrameworkAdapter();
  }
}