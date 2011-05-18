using js.net.Engine;
using NUnit.Framework;

namespace js.net.engine.tests
{
  public abstract class AbstractEngineTests
  {
    private IEngine engine;    

    protected abstract IEngine CreateEngine();

    [SetUp] public void SetUp()
    {
      engine = CreateEngine();
    }

    [TearDown] public void TearDown()
    {
      engine.Dispose();
    }

    [Test] public void SimpleAddition()
    {      
      Assert.AreEqual(2, engine.Run("1 + 1"));
    }

    [Test] public void TypeOfSetTimeout()
    {      
      Assert.AreEqual("function", engine.Run("typeof (setTimeout)"));
    }

    [Test] public void AccrossMultipleSessions()
    {      
      engine.Run("var x = 1");
      Assert.AreEqual(2, engine.Run("x + 1"));
    }

    [Test] public void ArrayReduce()
    {
      Assert.AreEqual(10, engine.Run(
@"
[0,1,2,3,4].reduce(function(previousValue, currentValue, index, array){
  return previousValue + currentValue;
});
"));
    }

    [Test] public void ResultsWithATimeout()
    {
      Assert.AreEqual(2, engine.Run(
@"
var x = 1;
setTimeout(function() {
  x = 2;
}, 10);
"));
    }

    [Test] public void TestSetModifyAndGetGlobal()
    {
      engine.SetGlobal("x", 10);
      engine.Run("x *= x;");
      Assert.AreEqual(100, engine.GetGlobal("x"));
    }
  }
}
