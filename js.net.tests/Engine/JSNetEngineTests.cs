using System;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using js.net.Engine;
using Noesis.Javascript;
using NUnit.Framework;

namespace js.net.tests.Engine
{
  [TestFixture] public class JSNetEngineTests : AbstractEngineTests
  {
    protected override IEngine CreateEngine()
    {
      return new JSNetEngine();
    }
   
    [Test, Ignore("Known Memory Leak in js.net")] public void MemoryUsageTests()
    {      
      MemoryUsageLoadInstance();
      long mem = Process.GetCurrentProcess().PrivateMemorySize64;

      for (int i = 0; i < 20; i++)
      {        
        MemoryUsageLoadInstance();
      }
      GC.Collect();
      decimal diffMBytes = Math.Round((Process.GetCurrentProcess().PrivateMemorySize64 - mem) / 1048576m, 2);
      Assert.Less(diffMBytes, 1); // Allow 1 MB
    }

    private void MemoryUsageLoadInstance() {
      using (JavascriptContext ctx = new JavascriptContext())
      {
        ctx.Run(
@"
buffer = [];
for (var i = 0; i < 100000; i++) {
  buffer[i] = 'new string';
}
");
      }
    }

    [Test, Ignore("Not supported")] public void TestCallbackIntoCSharp()
    {
      engine.SetGlobal("callbackHandler", this);
      engine.Run("var retval; callbackHandler.methodWithCallback(function(val) { retval = val; });", "TestCallbackIntoCSharp");
      Assert.AreEqual(10, engine.GetGlobal("retval"));
    }

    public void methodWithCallback(Action<int> action)
    {
      action(10);
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
