using System;
using System.Collections.Generic;
using System.IO;
using js.net.FrameworkAdapters;

namespace js.net.TestAdapters.Jasmine
{
  public class JasmineTestAdapter : AbstractTestAdapter
  {
    private readonly string jasminJsFile;

    public bool Silent { get; set; }

    public JasmineTestAdapter(SimpleDOMAdapter js, string jasminJsFile) : base(js)
    {
      this.jasminJsFile = jasminJsFile;
    }
    
    protected override void PrepareFrameworkAndRunTest(string testFile) {
      js.Initialise();
      js.LoadJSFile(jasminJsFile);
      js.Run(GetTestingJSFromFile(testFile) + "; null;"); // Load tests into memory
      js.LoadJSFile(@"TestAdapters\Jasmine\js\reporter.js");      
    }

    protected override TestResults GetResults(string fileName)
    {
      TestResults res = new TestResults(fileName);
      foreach (KeyValuePair<string, object> testResult in (IDictionary<string, object>) js.GetGlobal("results"))
      {
        if (testResult.Value == null)
        {
          res.AddPassedTest(testResult.Key);
        } else
        {
          res.AddFailedTest(testResult.Key);
        }
      }
      return res;
    }
  }
}