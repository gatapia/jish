using js.net.Engine;
using js.net.FrameworkAdapters.Closure;
using NUnit.Framework;

namespace js.net.tests.FrameworkAdapters
{
  [TestFixture] public class ClosureAdapterTests
  {
    private ClosureAdapter ctx;
    [SetUp] public void SetUp()
    {
      const string basejsfile = @"C:\dev\Projects\Misc\closure-library\closure\goog\base.js";
      const string jsdomJsFile = @"C:\dev\libs\jsdom\lib\jsdom.js";

      IEngine engine = new JSNetEngine();      
      ctx = new ClosureAdapter(basejsfile, jsdomJsFile, engine);
      ctx.Initialise();
    }

    [TearDown] public void TearDown()
    {
      ctx.Dispose();
    }

    [Test] public void NonClosureCode()
    {      
      Assert.AreEqual(2, ctx.Run("1 + 1", "ClosureAdapterTests.NonClosureCode"));
    }

    [Test] public void SimpleBaseJSDependantScript()
    {
      Assert.AreEqual("function", ctx.Run("typeof (goog.bind)", "ClosureAdapterTests.SimpleBaseJSDependantScript"));
    }

    [Test] public void AccrossMultipleSessions()
    {
      ctx.Run("var x = 1", "ClosureAdapterTests.TestSetModifyAndGetGlobal");
      Assert.AreEqual(2, ctx.Run("x + 1", "ClosureAdapterTests.AccrossMultipleSessions"));
    }

    [Test] public void CallingArrayFunction()
    {
      Assert.AreEqual(10, ctx.Run(
@"
[0,1,2,3,4].reduce(function(previousValue, currentValue, index, array){
  return previousValue + currentValue;
});
", "ClosureAdapterTests.CallingArrayFunction"));
    }

    [Test] public void UsingAComplexNonClosureFunction()
    {      
      Assert.AreEqual(10, ctx.Run(
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
      ctx.Run("goog.require('goog.string');", "ClosureAdapterTests.GoogRequireString");
    }    
  }
}
