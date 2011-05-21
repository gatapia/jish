using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using js.net.TestAdapters;
using NUnit.Framework;

namespace js.net.tests.TestAdapters
{
  [TestFixture] public class JsCoverageTests
  {
    const string basejsfile = @"C:\dev\Projects\Misc\closure-library\closure\goog\base.js";    

    [Test] public void AssertThatTheClosureTestWorksWithoutAnyCoverageStuff()
    {
      using (ITestAdapter adapter = JSNet.ClosureLibrary(basejsfile))
      {
        adapter.LoadSourceFile(@"resources\jscoverage\src\jscoverage_source.js"); 
        ITestResults results = adapter.RunTest(@"resources\jscoverage\jscoverage_test.js");         

        Assert.AreEqual(0, results.Failed.Count(), results.ToString());
        Assert.AreEqual(4, results.Passed.Count(), results.ToString());
      }            
    }
    
    [Test] public void TestInstrument()
    {
      const string instrumented = @"resources\jscoverage\instrumented\";
      const string source = @"resources\jscoverage\src\";      

      if (Directory.Exists(instrumented)) Directory.Delete(instrumented, true);
      Directory.CreateDirectory(instrumented);

      const string cmd = @"resources\jscoverage\jscoverage.exe";
      const string args = source + " " + instrumented + " --encoding=UTF-8";
      Console.WriteLine(cmd + " " + args);
      Process p = Process.Start(cmd, args);
      p.WaitForExit();
      Assert.AreEqual(0, p.ExitCode);

      Assert.IsTrue(File.Exists(instrumented + "jscoverage_source.js"));
    }

    [Test] public void TestRunCoverageWithRawTestAdapter()
    {
      TestInstrument();
      using (ITestAdapter adapter = JSNet.ClosureLibrary(basejsfile))
      {        
        adapter.LoadSourceFile(@"resources\jscoverage\instrumented\jscoverage_source.js"); 
        ITestResults results = adapter.RunTest(@"resources\jscoverage\jscoverage_test.js");         

        Assert.AreEqual(0, results.Failed.Count(), results.ToString());
        Assert.AreEqual(4, results.Passed.Count(), results.ToString());
        
        adapter.LoadSourceFile(@"resources\jscoverage.parser.js");         
      }  
    }

    [Test] public void TestRunCoverageWithProperAdapter()
    {
      TestInstrument();
      using (ICoverageAdapter adapter = JSNet.JsCoverage(JSNet.ClosureLibrary(basejsfile)))
      {        
        adapter.LoadSourceFile(@"resources\jscoverage\instrumented\jscoverage_source.js"); 
        ICoverageResults results = adapter.RunCoverage(@"resources\jscoverage\jscoverage_test.js");         

        Assert.AreEqual(0, results.Failed.Count(), results.ToString());
        Assert.AreEqual(4, results.Passed.Count(), results.ToString());

        Assert.AreEqual(1, results.FilesCount);
        Assert.AreEqual(5, results.Statements);
        Assert.AreEqual(5, results.Executed);
        Assert.AreEqual(100.0m, results.CoveragePercentage);

        IFileCoverageResults sourceCoverage = results.FileResults.First();
        Assert.AreEqual("jscoverage_source.js", sourceCoverage.FileName);
        Assert.AreEqual(5, sourceCoverage.Statements);
        Assert.AreEqual(5, sourceCoverage.Executed);
        Assert.AreEqual(100.0m, sourceCoverage.CoveragePercentage);        

        Console.WriteLine(results);
      }  
    }
  }
}
