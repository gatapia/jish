using System.Collections.Generic;
using js.net.FrameworkAdapters;

namespace js.net.TestAdapters.JsUnit
{
  public class JsUnitTestAdapter : AbstractTestAdapter
  {
    private readonly string jsUnitCoreFile;

    public bool Silent { get; set; }

    public JsUnitTestAdapter(SimpleDOMAdapter js, string jsUnitCoreFile) : base(js)
    {
      this.jsUnitCoreFile = jsUnitCoreFile;
    }

    protected override void PrepareFrameworkAndRunTest(string testFile) {
      js.Initialise();
      js.LoadJSFile(jsUnitCoreFile);
      js.Run(GetTestingJSFromFile(testFile)); // Load tests into memory
      js.LoadJSFile(@"TestAdapters\JsUnit\js\testmanager.js");
    }

    protected override TestResults GetResults(string fileName)
    {
      IDictionary<string, object> results = (IDictionary<string, object>) js.GetGlobal("results");
      TestResults res = new TestResults(fileName);
      foreach (KeyValuePair<string, object> testResult in results)
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