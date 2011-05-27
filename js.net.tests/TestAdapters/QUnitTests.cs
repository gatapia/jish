using System;
using System.Linq;
using js.net.Engine;
using js.net.TestAdapters;
using js.net.TestAdapters.QUnit;
using NUnit.Framework;

namespace js.net.tests.TestAdapters
{
  [TestFixture] public class QUnitTests
  {
    private const string qUnitJS = @"C:\dev\libs\qunit\qunit\qunit.js";

    [Test] public void RunSingleTestFile()
    {      
      using (ITestAdapter adapter = JSNet.QUnit(qUnitJS))
      {
        ITestResults results = adapter.RunTest(@"C:\dev\libs\qunit\test\test.js"); 
        
        Assert.AreEqual(2, results.Failed.Count());
        Assert.AreEqual(23, results.Passed.Count());
      }            
    }

    [Test] public void RunEntireQUnitTestSuite()
    {
      QUnitTestAdapterFactory fact = new QUnitTestAdapterFactory(qUnitJS, new DefaultEngineFactory());
      string[] files = GetTestSuiteFiles();
      TestSuiteResults results = new TestSuiteRunner(fact).TestFiles(files);
      
      Assert.AreEqual(2, results.Failed.Count());
      Assert.AreEqual(38, results.Passed.Count());
    }

    private string[] GetTestSuiteFiles()
    {
      return new[] { @"C:\dev\libs\qunit\test\same.js", @"C:\dev\libs\qunit\test\test.js" };
    }
  }
}
