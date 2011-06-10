using System;
using System.Diagnostics;
using System.IO;
using js.net.FrameworkAdapters;

namespace js.net.TestAdapters.Closure
{
  public class ClosureTestAdapter : AbstractTestAdapter
  {
    private ClosureTestsConsoleScrapper scrapper;

    public ClosureTestAdapter(IFrameworkAdapter js) : base(js) {}

    protected override void PrepareFrameworkAndRunTest(string sourceFile)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(sourceFile));
      Trace.Assert(File.Exists(sourceFile));

      string niceFileName = new FileInfo(sourceFile).Name;      

      InterceptConsoleLogging(niceFileName);
      PrepareEnvironmentForTests();            
      LoadTestFile(sourceFile, niceFileName);
      ExecuteTests(niceFileName); 
    }

    private void InterceptConsoleLogging(string niceFileName)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(niceFileName));

      scrapper = new ClosureTestsConsoleScrapper(niceFileName);            
      js.SetGlobal("console", scrapper);
    }

    private void PrepareEnvironmentForTests()
    {
      Trace.Assert(js != null);

      js.Run(@"
window.console = console;
goog.require('goog.testing.jsunit');
", "ClosureTestAdapter.PreLoadFile");
    }

    private void LoadTestFile(string sourceFile, string niceFileName)
    {
      Trace.Assert(js != null);
      Trace.Assert(File.Exists(sourceFile));
      Trace.Assert(!String.IsNullOrWhiteSpace(niceFileName));


      js.Run(GetTestingJSFromFile(sourceFile), niceFileName);
    }

    private void ExecuteTests(string niceFileName)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(niceFileName));

      js.Run(
        @"
var test = new goog.testing.TestCase('" + niceFileName + @"');
test.autoDiscoverTests();
G_testRunner.initialize(test);
G_testRunner.execute();
", "ClosureTestAdapter.PostLoadFile");
    }

    protected override TestResults GetResults(string testFixtureName)
    {
      Trace.Assert(scrapper != null);
      Trace.Assert(!String.IsNullOrWhiteSpace(testFixtureName));

      return scrapper.GetResults();
    }
  }
}
