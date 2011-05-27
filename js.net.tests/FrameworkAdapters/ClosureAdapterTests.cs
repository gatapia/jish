using js.net.TestAdapters;
using NUnit.Framework;

namespace js.net.tests.FrameworkAdapters
{
  [TestFixture] public class ClosureAdapterTests
  {
    private ITestAdapter ctx;
    [SetUp] public void SetUp()
    {
      const string basejsfile = @"C:\dev\Projects\Misc\closure-library\closure\goog\base.js";

      ctx = JSNet.ClosureLibrary(basejsfile);
    }

    [TearDown] public void TearDown()
    {
      ctx.Dispose();
    }

    [Test] public void NonClosureCode()
    {      
      Assert.AreEqual(2, ctx.GetInternalEngine().Run("1 + 1", "ClosureAdapterTests.NonClosureCode"));
    }

    [Test] public void SimpleBaseJSDependantScript()
    {
      Assert.AreEqual("function", ctx.GetInternalEngine().Run("typeof (goog.bind)", "ClosureAdapterTests.SimpleBaseJSDependantScript"));
    }

    [Test] public void AccrossMultipleSessions()
    {
      ctx.GetInternalEngine().Run("var x = 1", "ClosureAdapterTests.TestSetModifyAndGetGlobal");
      Assert.AreEqual(2, ctx.GetInternalEngine().Run("x + 1", "ClosureAdapterTests.AccrossMultipleSessions"));
    }

    [Test] public void CallingArrayFunction()
    {
      Assert.AreEqual(10, ctx.GetInternalEngine().Run(
@"
[0,1,2,3,4].reduce(function(previousValue, currentValue, index, array){
  return previousValue + currentValue;
});
", "ClosureAdapterTests.CallingArrayFunction"));
    }

    [Test] public void UsingAComplexNonClosureFunction()
    {      
      Assert.AreEqual(10, ctx.GetInternalEngine().Run(
@"
goog.require('goog.array');
var arr = [0,1,2,3,4];
goog.array.reduce(arr, function(previousValue, currentValue, index, array){
  return previousValue + currentValue;
}, 0);
", "ClosureAdapterTests.UsingAComplexNonClosureFunction"));
    }

    [Test] public void GoogRequireString()
    {
      ctx.GetInternalEngine().Run("goog.require('goog.string');", "ClosureAdapterTests.GoogRequireString");
    }    
  }
}
