using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using js.net.FrameworkAdapters;

namespace js.net.TestAdapters.QUnit
{
  public class QUnitTestAdapter : AbstractTestAdapter
  {
    private readonly string qUnitJs;

    public QUnitTestAdapter(JSDomAdapter js, string qUnitJs) : base(js)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(qUnitJs));
      Trace.Assert(File.Exists(qUnitJs));
      Trace.Assert(js != null);

      this.qUnitJs = qUnitJs;
    }

    protected override void PrepareFrameworkAndRunTest(string sourceFile)
    {      
      Trace.Assert(!String.IsNullOrWhiteSpace(sourceFile));
      Trace.Assert(File.Exists(sourceFile));

      // Initialise Framework
      js.Initialise(); 
      // Pre - qunit.js load
      js.Run(@"
location.protocol = 'file:';
// Required as QUnit will not extend the window object if this is not set 
// to undefined.
exports = undefined; 
", "QUniteTestAdapter.PreLoadFile");
      // Load qunit.js
      js.LoadJSFile(qUnitJs, false);
      // Override testDone
      js.Run(
@"
var globalResults = {};
QUnit.testDone = function(testResults) {
  globalResults[testResults.name] = testResults.failed;
};
"
, "QUniteTestAdapter.PostLoadFile");
      // Load <testfile>.js
      js.Run(GetTestingJSFromFile(sourceFile), new FileInfo(sourceFile).Name); 
    }

    protected override TestResults GetResults(string testFixtureName)
    {      
      Trace.Assert(!String.IsNullOrWhiteSpace(testFixtureName));      

      IDictionary<string, object> results = (IDictionary<string, object>) js.GetGlobal("globalResults");
      TestResults res = new TestResults(testFixtureName);
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