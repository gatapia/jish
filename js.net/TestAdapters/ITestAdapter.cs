using System;

namespace js.net.TestAdapters
{
  public interface ITestAdapter : IDisposable
  {
    void LoadSourceFile(string sourceFile);
    TestResults RunTest(string testFile);    
  }
}