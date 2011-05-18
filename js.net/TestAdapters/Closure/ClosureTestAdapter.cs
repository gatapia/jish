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
      scrapper = new ClosureTestsConsoleScrapper(new FileInfo(sourceFile).Name, Silent);        
      js.SetGlobal("console", scrapper); // Intercept console.log calls               
      js.Run(GetTestingJSFromFile(sourceFile)); // Load the file        
      js.Run("window.onload();"); 
    }

    protected override TestResults GetResults(string fileName)
    {
      return scrapper.GetResults();
    }
  }
}
