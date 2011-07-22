using System.Collections.Generic;
using System.IO;
using System.Linq;
using js.net.TestAdapters;
using NUnit.Framework;

namespace js.net.tests.TestAdapters
{
  [TestFixture] public class ClosureTests
  {
    const string basejsfile = @"J:\dev\Projects\Misc\closure-library\closure\goog\base.js";    

    [Test] public void RunJSFileTest()
    {
      using (ITestAdapter adapter = JSNet.ClosureLibrary(basejsfile))
      {
        ITestResults results = adapter.RunTest(@"resources\simple_closure_tests.js");         

        Assert.AreEqual(0, results.Failed.Count(), results.ToString());
        Assert.AreEqual(66, results.Passed.Count(), results.ToString());
      }            
    }    

    [Test] public void RunHtmlFileTest()
    {
      using (ITestAdapter adapter = JSNet.ClosureLibrary(basejsfile))
      {        
        ITestResults results = adapter.RunTest(@"J:\dev\Projects\Misc\closure-library\closure\goog\array\array_test.html");
        
        Assert.AreEqual(0, results.Failed.Count(), results.ToString());
        Assert.AreEqual(71, results.Passed.Count(), results.ToString());
      }
    }    
        
    [Test, Ignore("Runs out of memory")] public void RunEntireClosureTestSuite()
    {
      IEnumerable<string> files = GetTestSuiteFiles();
      TestSuiteRunner runner = JSNet.ClosureLibraryTestSuiteRunner(basejsfile);      
      TestSuiteResults results = runner.TestFiles(files);

      Assert.AreEqual(0, results.Failed.Count(), results.ToString());
      Assert.AreEqual(500, results.Passed.Count(), results.ToString());
    }

    private IEnumerable<string> GetTestSuiteFiles()
    {
      const string dir = @"J:\dev\Projects\Misc\closure-library\closure\goog\";      
      string[] allFiles = Directory.GetFiles(dir, "*_test.html", SearchOption.AllDirectories);
      string[] ignore = new [] {"fontsizemonitor_test.html", "icontent_test.html", "abstractdialogplugin_test.html", 
        "linkdialogplugin_test.html", "crossdomainrpc_test.html", "positioning_test.html", "serializer_test.html",
        "descriptor_test.html", "message_test.html", "proto_test.html", "viewportclientposition_test.html",
        "spellcheck_test.html", "pubsub_test.html", "collectablestorage_test.html"};
      return allFiles.Where(f => !ignore.Any(i => f.IndexOf(i) >= 0)).ToArray();
    }
  }
}
