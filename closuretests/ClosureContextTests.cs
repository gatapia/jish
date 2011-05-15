using js.Closure;
using js.Engine;
using NUnit.Framework;

namespace closuretests
{
  [TestFixture] public class ClosureContextTests
  {
    private ClosureContext ctx;
    [SetUp] public void SetUp()
    {
      const string basejsfile = @"C:\dev\lib\closure-library\closure\goog\base.js";
      IEngine engine = new JSNetEngineAdapter();      
      ctx = new ClosureContext(basejsfile, engine);
    }

    [TearDown] public void TearDown()
    {
      ctx.Dispose();
    }

    [Test] public void NonClosureCode()
    {      
      Assert.AreEqual(2, ctx.Run("1 + 1"));
    }

    [Test] public void SimpleBaseJSDependantScript()
    {
      Assert.AreEqual("function", ctx.Run("typeof (goog.bind)"));
    }

    [Test] public void AccrossMultipleSessions()
    {
      ctx.Run("var x = 1");
      Assert.AreEqual(2, ctx.Run("x + 1"));
    }

    [Test] public void CallingArrayFunction()
    {
      Assert.AreEqual(10, ctx.Run(
@"
[0,1,2,3,4].reduce(function(previousValue, currentValue, index, array){
  return previousValue + currentValue;
});
"));
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
"));
    }

    [Test] public void GoogRequireString()
    {
      ctx.Run("goog.require('goog.string');");
    }

    [Test] public void ResultsWithATimeout()
    {
      Assert.AreEqual(2, ctx.Run(
@"
var x = 1;
global.setTimeout(function() {
  x = 2;
}, 10);
"));
    }
  }
}
