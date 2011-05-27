using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using js.net.FrameworkAdapters;
using js.net.Util;

namespace js.net.TestAdapters.JSUnit
{
  public class JSUnitTestAdapter : AbstractTestAdapter
  {
    private readonly string jsUnitCoreFile;

    public JSUnitTestAdapter(JSDomAdapter js, string jsUnitCoreFile) : base(js)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(jsUnitCoreFile));
      Trace.Assert(File.Exists(jsUnitCoreFile));

      this.jsUnitCoreFile = jsUnitCoreFile;
    }

    protected override void PrepareFrameworkAndRunTest(string testFile) {
      Trace.Assert(!String.IsNullOrWhiteSpace(testFile));
      Trace.Assert(File.Exists(testFile));

      js.Initialise();
      js.LoadJSFile(jsUnitCoreFile, false);
      js.Run(GetTestingJSFromFile(testFile), new FileInfo(testFile).Name); // Load tests into memory
      string testManager = new EmbeddedResourcesUtils().ReadEmbeddedResourceTextContents("js.net.resources.jsunit.testmanager.js");
      js.Run(testManager, "js.net.resources.jsunit.testmanager.js");
    }

    protected override TestResults GetResults(string testFixtureName)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(testFixtureName));

      IDictionary<string, object> results = (IDictionary<string, object>) js.GetGlobal("results");
      TestResults res = new TestResults(testFixtureName);
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