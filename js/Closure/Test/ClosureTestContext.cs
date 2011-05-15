using System;
using System.Collections.Generic;
using System.IO;
using js.Engine;
using PicNet2;

namespace js.Closure.Test
{
  public class ClosureTestContext : IDisposable
  {
    private readonly ClosureContext ctx;
    
    public ClosureTestContext(string baseJsFile, IEngine engine) 
    {
      ctx = new ClosureContext(baseJsFile, engine);
      ctx.LoadJSFile(@"C:\dev\projects\play\picnetjs\lib\Jasmine\env.therubyracer.js");
      ctx.LoadJSFile(@"C:\dev\projects\play\picnetjs\lib\Jasmine\window.js");      
    }

    public bool Silent { get; set; }

    public ClosureTestSuiteResults RunTestDirectory(string directory, string searchPattern = "*.*")
    {
      string[] files = Directory.GetFiles(directory, searchPattern, SearchOption.AllDirectories);
      IList<ClosureTestResults> results = new List<ClosureTestResults>();
      CollectionUtils.ForEach(files, f => results.Add(RunTest(f)));
      return new ClosureTestSuiteResults(results);
    }

    public ClosureTestResults RunTest(string file)
    {
      Console.WriteLine("Running Test: " + file);
      ctx.Run(GetTestingJSFromFile(file)); // Load the file

      ClosureTestsConsoleScrapper scrapper = new ClosureTestsConsoleScrapper(new FileInfo(file).Name, Silent);
      ctx.SetGlobal("console", scrapper); // Intercept console.log calls
      ctx.Run("window.onload();"); // Run the tests
      return scrapper.GetResults(); // Scrape the results from the console.log calls
    }

    private string GetTestingJSFromFile(string testFile)
    {
      return testFile.EndsWith(".js")
               ? File.ReadAllText(testFile)
               : new HtmlFileScriptExtractor(testFile).GetScriptContents();
    }

    public void Dispose()
    {
      ctx.Dispose();
    }
  }
}
