using System;
using js.Closure.Test;
using js.Engine;
using NUnit.Framework;

namespace closuretests
{
  [TestFixture] public class ClosureTestContextTests
  {
    private ClosureTestContext ctx;
    [SetUp] public void SetUp()
    {
      const string basejsfile = @"C:\dev\lib\closure-library\closure\goog\base.js";
      IEngine engine = new JSNetEngineAdapter();      
      ctx = new ClosureTestContext(basejsfile, engine);
      ctx.Silent = true;
    }

    [TearDown] public void TearDown()
    {
      ctx.Dispose();
    }

    [Test] public void RunHtmlIndependentJSUnitTest()
    {
      ClosureTestResults results = ctx.RunTest(@"C:\dev\projects\play\picnetjs\closuretests\stand_alone_base_tests.js");
      Assert.IsNotNull(results);
      Console.WriteLine(results);
    }

    [Test] public void RunBaseHtmlTests()
    {
      ClosureTestResults results = ctx.RunTest(@"C:\dev\lib\closure-library\closure\goog\base_test.html");
      Assert.IsNotNull(results);
      Console.WriteLine(results);
    }

    [Test] public void RunHtmlDependantJSUnitTest()
    {
      ClosureTestResults results = ctx.RunTest(@"C:\dev\lib\closure-library\closure\goog\array\array_test.html");      
      Assert.IsNotNull(results);
      Console.WriteLine(results);
    }    

    [Test] public void RunEntireClosureTestSuite()
    {
      ClosureTestSuiteResults results = ctx.RunTestDirectory(@"C:\dev\lib\closure-library\closure\goog\", "*_test.html");      
      Assert.IsNotNull(results);
      Console.WriteLine(results);
    }
  }
}
