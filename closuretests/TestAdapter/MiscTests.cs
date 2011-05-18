using System;
using js.net.Engine;
using js.net.TestAdapters.Closure;
using NUnit.Framework;

namespace js.net.closure.tests.TestAdapter
{
  [TestFixture] public class MiscTests
  {
    private IEngine engine;
    private ClosureTestAdapter ctx;

    [SetUp] public void SetUpContextAndEngine()
    {
      const string basejsfile = @"C:\dev\Projects\Misc\closure-library\closure\goog\base.js";
      engine = new JSNetEngineAdapter();      
      ctx = new ClosureTestAdapter(basejsfile, engine) { Silent = false }; 
    }

    [TearDown] public void TearDown()
    {
      engine.Dispose();
    }

    [Test] public void RunJSFileTest()
    {
      ClosureTestResults results = ctx.RunTest(@"C:\dev\projects\labs\js.net\closuretests\stand_alone_base_tests.js");
      Assert.IsNotNull(results);
      Console.WriteLine(results);
    }

    [Test] public void RunHtmlFileTest()
    {
      ClosureTestResults results = ctx.RunTest(@"C:\dev\Projects\Misc\closure-library\closure\goog\array\array_test.html");      
      Assert.IsNotNull(results);
      Console.WriteLine(results);
    }    

    [Test] public void RunEntireClosureTestSuite()
    {
      ClosureTestSuiteResults results = ctx.RunTestDirectory(@"C:\dev\Projects\Misc\closure-library\closure\goog\", "*_test.html");      
      Assert.IsNotNull(results);
      Console.WriteLine(results);
    }
  }
}
