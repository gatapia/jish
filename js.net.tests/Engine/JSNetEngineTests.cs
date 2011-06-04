using System.Dynamic;
using System.Linq;
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

    [Test, Ignore("Not supported")] public void TestDynamics()
    {
      dynamic d = new ExpandoObject();
      d.Test = "Test String";
      engine.SetGlobal("d", d);
      Assert.AreEqual("Test String", engine.Run("d.Test", ""));
    }

    [Test, Ignore("Not supported")] public void TestCallingParams()
    {
      engine.SetGlobal("test", this);
      Assert.AreEqual(0, engine.Run("test.TestParams();", ""));
    }

    [Test, Ignore("Not supported")] public void TestCallingOptionals()
    {
      engine.SetGlobal("test", this);
      Assert.AreEqual(1, engine.Run("test.TestOptional();", ""));
    }

    public int TestAdd(int i1, int i2)
    {
      return i1 + i2;
    }

    public int TestParams(params int[] args)
    {
      return args == null ? 0 : args.Sum();
    }

    public int TestOptional(int opt = 1)
    {
      return opt;
    }
  }
}
