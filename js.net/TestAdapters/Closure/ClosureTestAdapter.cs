using System;
using System.Diagnostics;
using System.IO;
using js.net.FrameworkAdapters.Closure;

namespace js.net.TestAdapters.Closure
{
  public class ClosureTestAdapter : AbstractTestAdapter
  {
    private ClosureTestsConsoleScrapper scrapper;

    public ClosureTestAdapter(ClosureAdapter js) : base(js) {}

    protected override void PrepareFrameworkAndRunTest(string sourceFile)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(sourceFile));
      Trace.Assert(File.Exists(sourceFile));

      string fileName = new FileInfo(sourceFile).Name;      
      scrapper = new ClosureTestsConsoleScrapper(fileName, GetInternalEngine());      
      js.Run(@"
var window = global.exports.html().createWindow(); 
var top = window;
var document = window.document;
window.location = {
  search: '',
  href: '" + fileName + @"'
};
window.console = console;

goog.require('goog.testing.jsunit');
", "ClosureTestAdapter.PreLoadFile");            
      js.Run(GetTestingJSFromFile(sourceFile), fileName); // Load the file        
      js.Run(
@"
var test = new goog.testing.TestCase('" + fileName + @"');
test.autoDiscoverTests();
G_testRunner.initialize(test);
G_testRunner.execute();
", "ClosureTestAdapter.PostLoadFile"); 
    }

    protected override TestResults GetResults(string testFixtureName)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(testFixtureName));

      return scrapper.GetResults();
    }
  }
}
