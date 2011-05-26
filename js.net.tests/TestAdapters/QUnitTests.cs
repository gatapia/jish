using System;
using System.Diagnostics;
using System.Linq;
using js.net.Engine;
using js.net.TestAdapters;
using js.net.TestAdapters.QUnit;
using NUnit.Framework;

namespace js.net.tests.TestAdapters
{
  [TestFixture] public class QUnitTests
  {
    static QUnitTests()
    {
      DefaultTraceListener def = (DefaultTraceListener) Trace.Listeners[0];
      def.AssertUiEnabled = false; // No silly dialogs
      Trace.Listeners.Clear();
      Trace.Listeners.Add(def);
    }

    private const string qUnitJS = @"C:\dev\libs\qunit\qunit\qunit.js";

    [Test] public void RunSingleTestFile()
    {      
      using (ITestAdapter adapter = JSNet.QUnit(qUnitJS))
      {
        ITestResults results = adapter.RunTest(@"C:\dev\libs\qunit\test\test.js"); 
        
        //Tests 'sync', 'setup' and 'basics' fail
        Assert.AreEqual(3, results.Failed.Count());
        Assert.AreEqual(22, results.Passed.Count());

        Assert.IsNotNull(results);
        Console.WriteLine(results);
      }            
    }

    [Test] public void RunEntireQUnitTestSuite()
    {
      QUnitTestAdapterFactory fact = new QUnitTestAdapterFactory(qUnitJS, new DefaultEngineFactory());
      string[] files = GetTestSuiteFiles();
      TestSuiteResults results = new TestSuiteRunner(fact).TestFiles(files);
      Assert.IsNotNull(results);
      Console.WriteLine(results); 
    }

    private string[] GetTestSuiteFiles()
    {
      return new[] { @"C:\dev\libs\qunit\test\same.js", @"C:\dev\libs\qunit\test\test.js" };
    }
  }
}
