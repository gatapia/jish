using System;
using System.Diagnostics;
using System.IO;
using js.net.FrameworkAdapters;
using Noesis.Javascript;

namespace js.net.TestAdapters
{
  public abstract class AbstractTestAdapter : ITestAdapter
  {
    protected readonly IFrameworkAdapter js;    

    protected AbstractTestAdapter(IFrameworkAdapter js)
    {
      Trace.Assert(js != null);

      this.js = js;
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
        InitDomEnvironment(fileName);
        PrepareFrameworkAndRunTest(testFile);
      }
      catch (JavascriptException ex)
      {
        return BuildFailTestResultFromException(fileName, ex);
      }
      return GetResults(fileName);
    }
    
    public IFrameworkAdapter GetFrameworkAdapter()
    {
      Trace.Assert(js != null);

      return js;
    }    

    public void Dispose()
    {
      Trace.Assert(js != null);

      js.Dispose();
    }

    protected string GetTestingJSFromFile(string testFile)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(testFile));
      Trace.Assert(File.Exists(testFile));

      return testFile.EndsWith(".js")
        ? File.ReadAllText(testFile)
        : new HtmlFileScriptExtractor().GetScriptContents(testFile);
    }

    protected abstract void PrepareFrameworkAndRunTest(string file);

    protected abstract TestResults GetResults(string testFixtureName);

    private void InitDomEnvironment(string fileName)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(fileName));

      js.Run(@"
var window = global.exports.html().createWindow(); 
var top = window;
var document = window.document;
var location = window.location = {
  search: '',
  href: '" + fileName + @"'
};
window.console = console;
", "AbstractTestAdapter.RunTest");
    }

    private TestResults BuildFailTestResultFromException(string fileName, JavascriptException ex)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(fileName));
      Trace.Assert(ex != null);

      TestResults tr = new TestResults(fileName);
      tr.AddFailedTest(fileName + " - " + GetNiceExceptionMessage(ex));
      Console.WriteLine("Exception: " + ex);
      return tr;
    }

    private string GetNiceExceptionMessage(Exception exception)
    {
      Trace.Assert(exception != null);

      return exception.Message.Replace("Exception in managed code invocation ", "");
    }        
  }
}