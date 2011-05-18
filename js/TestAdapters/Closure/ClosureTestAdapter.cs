using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using js.net.Engine;
using js.net.FrameworkAdapters.Closure;
using Noesis.Javascript;

namespace js.net.TestAdapters.Closure
{
  public class ClosureTestAdapter
  {
    private readonly string baseJsFile;
    private readonly IEngine engine;

    public ClosureTestAdapter(string baseJsFile, IEngine engine)
    {
      this.baseJsFile = baseJsFile;
      this.engine = engine;
    }
    
    public bool Silent { get; set; }    

    public ClosureTestSuiteResults RunTestDirectory(string directory, string searchPattern = "*.*")
    {
      string[] files = Directory.GetFiles(directory, searchPattern, SearchOption.AllDirectories).Take(2).ToArray();
      IList<ClosureTestResults> results = new List<ClosureTestResults>();
      Array.ForEach(files, f => results.Add(RunTest(f)));
      return new ClosureTestSuiteResults(results);
    }

    public ClosureTestResults RunTest(string file)
    {
      string fileName = new FileInfo(file).Name;
      ClosureTestsConsoleScrapper scrapper = new ClosureTestsConsoleScrapper(fileName, Silent);        
      Console.WriteLine("Running Test: " + fileName);
      
      using (ClosureAdapter closure = CreateClosureAdapter())
      {        
        try
        {          
          closure.Run(GetTestingJSFromFile(file)); // Load the file
          closure.SetGlobal("console", scrapper); // Intercept console.log calls               
          closure.Run("window.onload();"); 
        } catch (JavascriptException ex)
        {
          return new ClosureTestResults(fileName) { Failed = {fileName + " - " + ex.Message} };
        }
        return scrapper.GetResults(); // Scrape the results from the console.log calls 
      }      
    }

    private string GetTestingJSFromFile(string testFile)
    {
      return testFile.EndsWith(".js")
               ? File.ReadAllText(testFile)
               : new HtmlFileScriptExtractor(testFile).GetScriptContents();
    }

    private ClosureAdapter CreateClosureAdapter()
    {
      ClosureAdapter closure = new ClosureAdapter(baseJsFile, engine);      

      closure.LoadJSFile(@"C:\dev\projects\labs\js.net\lib\Jasmine\env.therubyracer.js");
      closure.LoadJSFile(@"C:\dev\projects\labs\js.net\lib\Jasmine\window.js");

      return closure;
    }
  }
}
