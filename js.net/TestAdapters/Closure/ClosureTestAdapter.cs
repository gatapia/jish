using System.IO;
using js.net.FrameworkAdapters.Closure;

namespace js.net.TestAdapters.Closure
{
  public class ClosureTestAdapter : AbstractTestAdapter
  {
    private ClosureTestsConsoleScrapper scrapper;

    public bool Silent { get; set; }    

    public ClosureTestAdapter(ClosureAdapter js) : base(js) {}            

    protected override void PrepareFrameworkAndRunTest(string sourceFile)
    {
      string fileName = new FileInfo(sourceFile).Name;
      scrapper = new ClosureTestsConsoleScrapper(fileName, Silent);        
      js.SetGlobal("console", scrapper); // Intercept console.log calls               
      js.Run("goog.require('goog.testing.jsunit');");
      js.Run(GetTestingJSFromFile(sourceFile)); // Load the file        
      js.Run(
@"
var test = new goog.testing.TestCase('" + fileName + @"');
test.autoDiscoverTests();
G_testRunner.initialize(test);
G_testRunner.execute();
"); 
    }

    protected override TestResults GetResults(string fileName)
    {
      return scrapper.GetResults();
    }
  }
}
