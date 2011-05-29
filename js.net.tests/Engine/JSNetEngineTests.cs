using js.net.Engine;
using NUnit.Framework;

namespace js.net.tests.Engine
{
  [TestFixture] public class JSNetEngineTests : AbstractEngineTests
  {
    protected override IEngine CreateEngine()
    {
      return new JSNetEngine();
    }

    [Test] public void TestSetGlobal()
    {
      engine.SetGlobal("newglobal", this);
      int add = (int) engine.Run("newglobal.TestAdd(1, 2);", "test");
      Assert.AreEqual(3, add);
    }

    [Test, Ignore("Not supported")] public void TestSetGlobalEmbedded()
    {
      engine.Run("var ns = {}; ns.class = {};", "TestSetGlobalEmbedded");
      engine.SetGlobal("ns.class", this);
      int add = (int) engine.Run("ns.class.TestAdd(1, 2);", "test");
      Assert.AreEqual(3, add);
    }

    public int TestAdd(int i1, int i2)
    {
      return i1 + i2;
    }
  }
}
