using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using js.net.FrameworkAdapters;
using js.net.Util;

namespace js.net.TestAdapters.Jasmine
{
  public class JasmineTestAdapter : AbstractTestAdapter
  {
    private readonly string jasminJsFile;

    public bool Silent { get; set; }

    public JasmineTestAdapter(SimpleDOMAdapter js, string jasminJsFile) : base(js)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(jasminJsFile));
      Trace.Assert(File.Exists(jasminJsFile));

      this.jasminJsFile = jasminJsFile;
    }
    
    protected override void PrepareFrameworkAndRunTest(string testFile) {
      Trace.Assert(!String.IsNullOrWhiteSpace(testFile));
      Trace.Assert(File.Exists(testFile));

      js.Initialise();
      js.LoadJSFile(jasminJsFile);
      js.Run(GetTestingJSFromFile(testFile) + "; null;"); // Load tests into memory
      string jasmineReporter = new EmbeddedResourcesUtils().ReadEmbeddedResourceTextContents("js.net.resources.jasmine.reporter.js");
      js.Run(jasmineReporter);
    }

    protected override TestResults GetResults(string testFixtureName)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(testFixtureName));

      TestResults res = new TestResults(testFixtureName);
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