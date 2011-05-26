using System;
using System.Diagnostics;
using System.IO;
using js.net.Engine;
using js.net.FrameworkAdapters;
using Noesis.Javascript;

namespace js.net.TestAdapters
{
  public abstract class AbstractTestAdapter : ITestAdapter
  {
    protected readonly SimpleDOMAdapter js;    

    protected AbstractTestAdapter(SimpleDOMAdapter js)
    {
      Trace.Assert(js != null);

      this.js = js;
    }    

    protected string GetTestingJSFromFile(string testFile)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(testFile));
      Trace.Assert(File.Exists(testFile));

      return testFile.EndsWith(".js")
        ? File.ReadAllText(testFile)
        : new HtmlFileScriptExtractor(testFile).GetScriptContents();
    }

    public void LoadSourceFile(string sourceFile)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(sourceFile));
      Trace.Assert(File.Exists(sourceFile));

      js.LoadJSFile(sourceFile, false);
    }

    public ITestResults RunTest(string testFile)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(testFile));
      Trace.Assert(File.Exists(testFile));

      string fileName = new FileInfo(testFile).Name;
      try
      {
        PrepareFrameworkAndRunTest(testFile);
      } catch (JavascriptException ex)
      {
        TestResults tr = new TestResults(fileName);
        tr.AddFailedTest(fileName + " - " + GetNiceExceptionMessage(ex));
        Console.WriteLine("Exception: " + ex);
        return tr;
      }
      return GetResults(fileName);
    }

    private string GetNiceExceptionMessage(Exception exception)
    {
      return exception.Message.Replace("Exception in managed code invocation ", "");
    }

    public IEngine GetInternalEngine()
    {
      return js;
    }

    protected abstract void PrepareFrameworkAndRunTest(string file);

    protected abstract TestResults GetResults(string testFixtureName);

    public void Dispose()
    {
      if (js != null) js.Dispose();
    }
  }
}