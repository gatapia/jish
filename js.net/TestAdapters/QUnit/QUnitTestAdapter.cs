using System.Collections.Generic;
using js.net.FrameworkAdapters;

namespace js.net.TestAdapters.QUnit
{
  public class QUnitTestAdapter : AbstractTestAdapter
  {
    private readonly string qUnitJs;

    public bool Silent { get; set; }

    public QUnitTestAdapter(SimpleDOMAdapter js, string qUnitJs) : base(js)
    {
      this.qUnitJs = qUnitJs;
    }

    protected override void PrepareFrameworkAndRunTest(string sourceFile)
    {      
      // Initialise Framework
      js.Initialise(); 
      // Pre - qunit.js load
      js.Run(@"location.protocol = 'file:';");
      // Load qunit.js
      js.LoadJSFile(qUnitJs);
      // Load <testfile>.js
      js.Run(GetTestingJSFromFile(sourceFile)); 
      // window.onload
      js.Run(
@"
var globalResults = {};
QUnit.testDone = function(testResults) {
  globalResults[testResults.name] = testResults.failed;
};
var event = document.createEvent('HTMLEvent');
event.initEvent('load');
window.dispatchEvent(event);
"
);
    }

    protected override TestResults GetResults(string fileName)
    {
      IDictionary<string, object> results = (IDictionary<string, object>) js.GetGlobal("globalResults");
      TestResults res = new TestResults(fileName);
      foreach (KeyValuePair<string, object> testResult in results)
      {
        if ((int) testResult.Value == 0)
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