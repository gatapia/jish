using System;
using System.IO;
using System.Linq;
using js.net.TestAdapters;
using NUnit.Framework;

namespace js.net.tests.TestAdapters
{
  [TestFixture] public class ClosureTests
  {
    const string basejsfile = @"C:\dev\Projects\Misc\closure-library\closure\goog\base.js";    

    [Test] public void RunJSFileTest()
    {
      using (ITestAdapter adapter = JSNet.ClosureLibrary(basejsfile))
      {
        TestResults results = adapter.RunTest(@"TestAdapters\stand_alone_base_tests.js");         

        // 7 tests known to fail
        Assert.AreEqual(7, results.Failed.Count(), results.ToString());
        Assert.Greater(results.Passed.Count(), 0, results.ToString());
      }            
    }    

    [Test] public void RunHtmlFileTest()
    {
      using (ITestAdapter adapter = JSNet.ClosureLibrary(basejsfile))
      {
        TestResults results = adapter.RunTest(@"C:\dev\Projects\Misc\closure-library\closure\goog\array\array_test.html");
        Assert.IsNotNull(results);
        Console.WriteLine(results);
      }
    }    
        
    [Test, Ignore("Runs out of memory")] public void RunEntireClosureTestSuite()
    {
      string[] files = GetTestSuiteFiles();
      TestSuiteResults results = JSNet.ClosureLibraryTestSuiteRunner(basejsfile).TestFiles(files);

      Assert.AreEqual(0, results.Failed.Count(), results.ToString());
      Assert.Greater(results.Passed.Count(), 0, results.ToString());
    }

    private string[] GetTestSuiteFiles()
    {
      string dir = @"C:\dev\Projects\Misc\closure-library\closure\goog\";      
      string[] allFiles = Directory.GetFiles(dir, "*_test.html", SearchOption.AllDirectories);
      string[] ignore = new [] {"fontsizemonitor_test.html", "icontent_test.html", "abstractdialogplugin_test.html", 
        "linkdialogplugin_test.html", "crossdomainrpc_test.html", "positioning_test.html", "serializer_test.html",
        "descriptor_test.html", "message_test.html", "proto_test.html", "viewportclientposition_test.html",
        "spellcheck_test.html", "pubsub_test.html", "collectablestorage_test.html"};
      return allFiles.Where(f => !ignore.Any(i => f.IndexOf(i) >= 0)).ToArray();
    }
  }
}
