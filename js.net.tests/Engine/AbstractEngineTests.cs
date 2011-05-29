using js.net.Engine;
using NUnit.Framework;

namespace js.net.tests.Engine
{
  public abstract class AbstractEngineTests
  {
    protected IEngine engine;    

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
      Assert.AreEqual(2, engine.Run("1 + 1", "AbstractEngineTests.SimpleAddition"));
    }

    [Test] public void AccrossMultipleSessions()
    {      
      engine.Run("var x = 1", "AbstractEngineTests.AccrossMultipleSessions");
      Assert.AreEqual(2, engine.Run("x + 1", "AbstractEngineTests.AccrossMultipleSessions"));
    }

    [Test] public void ArrayReduce()
    {
      Assert.AreEqual(10, engine.Run(
@"
[0,1,2,3,4].reduce(function(previousValue, currentValue, index, array){
  return previousValue + currentValue;
});
", "AbstractEngineTests.ArrayReduce"));
    }

    [Test] public void TestSetModifyAndGetGlobal()
    {
      engine.SetGlobal("x", 10);
      engine.Run("x *= x;", "AbstractEngineTests.TestSetModifyAndGetGlobal");
      Assert.AreEqual(100, engine.GetGlobal("x"));
    }
  }
}
