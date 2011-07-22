using System;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using Noesis.Javascript;
using js.net.Engine;
using NUnit.Framework;
using js.net.Util;

namespace js.net.tests.Engine
{
  public abstract class AbstractEngineTests
  {
    static AbstractEngineTests() {
      Trace.Listeners.Clear();
      Trace.Listeners.Add(new ExceptionTraceListener());
    }

    protected IEngine engine;    

    protected abstract IEngine CreateEngine();

    [SetUp] public virtual void SetUp()
    {
      engine = CreateEngine();
    }

    [TearDown] public virtual void TearDown()
    {
      engine.Dispose();
    }

    [Test] public virtual void SimpleAddition()
    {      
      Assert.AreEqual(2, engine.Run("1 + 1", "AbstractEngineTests.SimpleAddition"));
    }

    [Test] public virtual void AccrossMultipleSessions()
    {      
      engine.Run("var x = 1", "AbstractEngineTests.AccrossMultipleSessions");
      Assert.AreEqual(2, engine.Run("x + 1", "AbstractEngineTests.AccrossMultipleSessions"));
    }

    [Test] public virtual void ArrayReduce()
    {
      Assert.AreEqual(10, engine.Run(
@"
[0,1,2,3,4].reduce(function(previousValue, currentValue, index, array){
  return previousValue + currentValue;
});
", "AbstractEngineTests.ArrayReduce"));
    }

       
    [Test] public virtual void TestSetGlobal()
    {
      engine.SetGlobal("newglobal", this);
      int add = (int) engine.Run("newglobal.TestAdd(1, 2);", "test");
      Assert.AreEqual(3, add);
    }

    [Test] public virtual void TestSetModifyAndGetGlobal()
    {
      engine.SetGlobal("x", 10);
      engine.Run("x *= x;", "AbstractEngineTests.TestSetModifyAndGetGlobal");
      Assert.AreEqual(100, engine.GetGlobal("x"));
    }

    [Test] public virtual void MemoryUsageTests()
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

    [Test] public virtual void TestCallbackIntoCSharp()
    {
      engine.SetGlobal("callbackHandler", this);
      engine.Run("var retval; callbackHandler.methodWithCallback(function(val) { retval = val; });", "TestCallbackIntoCSharp");
      Assert.AreEqual(10, engine.GetGlobal("retval"));
    }

    public virtual void methodWithCallback(Action<int> action)
    {
      action(10);
    }

    [Test] public virtual void TestSetGlobalEmbedded()
    {
      engine.Run("var ns = {}; ns.class = {};", "TestSetGlobalEmbedded");
      engine.SetGlobal("ns.class", this);
      int add = (int) engine.Run("ns.class.TestAdd(1, 2);", "test");
      Assert.AreEqual(3, add);
    }

    [Test] public virtual void TestDynamics()
    {
      dynamic d = new ExpandoObject();
      d.Test = "Test String";
      engine.SetGlobal("d", d);
      Assert.AreEqual("Test String", engine.Run("d.Test", ""));
    }

    [Test] public virtual void TestCallingParams()
    {
      engine.SetGlobal("test", this);
      Assert.AreEqual(0, engine.Run("test.TestParams();", ""));
    }

    [Test] public virtual void TestCallingOptionals()
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
