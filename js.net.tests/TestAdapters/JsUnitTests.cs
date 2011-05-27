using System;
using System.IO;
using System.Linq;
using js.net.Engine;
using js.net.TestAdapters;
using js.net.TestAdapters.JSUnit;
using NUnit.Framework;

namespace js.net.tests.TestAdapters
{
  [TestFixture] public class JSUnitTests
  {
    private const string jsUnitCoreFile = @"C:\dev\libs\jsunit\app\jsUnitCore.js";

    [Test] public void RunFailingTests()
    {      
      using (ITestAdapter adapter = JSNet.JSUnit(jsUnitCoreFile))
      {
        ITestResults results = adapter.RunTest(@"C:\dev\libs\jsunit\tests\failingTest.html"); 

        Assert.AreEqual(2, results.Failed.Count());
        Assert.AreEqual(0, results.Passed.Count());

        Assert.IsNotNull(results);
        Console.WriteLine(results);
      }            
    }

    [Test] public void SucceedingTests()
    {      
      using (ITestAdapter adapter = JSNet.JSUnit(jsUnitCoreFile))
      {
        ITestResults results = adapter.RunTest(@"C:\dev\libs\jsunit\tests\jsUnitAssertionTests.html"); 

        Assert.AreEqual(0, results.Failed.Count());
        Assert.AreEqual(31, results.Passed.Count());

        Assert.IsNotNull(results);
        Console.WriteLine(results);
      }            
    }

    [Test] public void RunEntireJSUnitTestSuite()
    {
      JSUnitTestAdapterFactory fact = new JSUnitTestAdapterFactory(jsUnitCoreFile, new DefaultEngineFactory());
      string[] files = GetTestSuiteFiles();
      TestSuiteResults results = new TestSuiteRunner(fact).TestFiles(files);
            
      Assert.AreEqual(51, results.Passed.Count());
      Assert.AreEqual(22, results.Failed.Count());
    }

    private string[] GetTestSuiteFiles()
    {
      return Directory.GetFiles(@"C:\dev\libs\jsunit\tests", "*Tests.html");
    }
  }
}
