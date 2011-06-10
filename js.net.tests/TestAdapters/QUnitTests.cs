using System.Collections.Generic;
using System.Linq;
using js.net.TestAdapters;
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
      TestSuiteResults results = JSNet.QUnitTestSuiteRunner(qUnitJS).TestFiles(GetTestSuiteFiles());
      
      Assert.AreEqual(2, results.Failed.Count());
      Assert.AreEqual(38, results.Passed.Count());
    }

    private IEnumerable<string> GetTestSuiteFiles()
    {
      return new[] { @"C:\dev\libs\qunit\test\same.js", @"C:\dev\libs\qunit\test\test.js" };
    }
  }
}
