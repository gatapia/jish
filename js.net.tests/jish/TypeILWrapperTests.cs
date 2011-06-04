using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using js.net.jish.IL;
using NUnit.Framework;

namespace js.net.tests.jish
{
  [TestFixture] public class TypeILWrapperTests
  {  
    private readonly TypeILWrapper wrapper = new TypeILWrapper();
    private readonly MethodInfo miStatic = typeof (StaticWithSimpleArgs).GetMethod("GetStringStatic", new [] {typeof(int)});
    private readonly MethodInfo miInstance = typeof (NonStaticWithSimpleArgs).GetMethod("GetStringNonStatic");

    [Test] public void TestStaticTypeWithSimpleArgs()
    {
      object wrapped = wrapper.CreateWrapper(typeof(StaticWithSimpleArgs));
      Assert.AreEqual("STATIC[123]", miStatic.Invoke(wrapped, new object[] {123}));
    }

    [Test] public void TestNonStaticWithSimpleArgs()
    {
      object instance = new NonStaticWithSimpleArgs(222);
      object wrapped = wrapper.CreateWrapper(instance);
      Assert.AreEqual("INSTANCE[222][0][123]", wrapped.GetType().GetMethod("GetStringNonStatic").Invoke(wrapped, new object[] {123}));
    }

    [Test] public void TestNonStaticWithMultipleArgs()
    {
      object instance = new NonStaticWithSimpleArgs(222, 333);
      object wrapped = wrapper.CreateWrapper(instance);
      Assert.AreEqual("INSTANCE[222][333][123]", wrapped.GetType().GetMethod("GetStringNonStatic").Invoke(wrapped, new object[] {123}));
    }

    [Test] public void TestMergeMethodsInSingleType()
    {
      object instance = new NonStaticWithSimpleArgs(222);
      object wrapped = wrapper.CreateWrapper(typeof(StaticWithSimpleArgs), new [] {new ProxyMethod(miStatic, null), new ProxyMethod(miInstance, instance)});
      Assert.AreEqual("INSTANCE[222][0][1]", wrapped.GetType().GetMethod("GetStringNonStatic").Invoke(wrapped, new object[] {1}));
      Assert.AreEqual("STATIC[2]", wrapped.GetType().GetMethod("GetStringStatic").Invoke(wrapped, new object[] {2}));
    }

    [Test] public void TestMergeMethodsInSingleTypeWithMultipleInstances()
    {
      object instance = new NonStaticWithSimpleArgs(222);
      object instance2 = new NonStaticWithSimpleArgs(2, 3);
      object wrapped = wrapper.CreateWrapper(typeof(StaticWithSimpleArgs), new [] {new ProxyMethod(miStatic, null), new ProxyMethod(miInstance, instance), new ProxyMethod(miInstance, instance2) { OverrideMethodName = "NewMethodName"}});
      Assert.AreEqual("INSTANCE[222][0][1]", wrapped.GetType().GetMethod("GetStringNonStatic").Invoke(wrapped, new object[] {1}));
      Assert.AreEqual("INSTANCE[2][3][1]", wrapped.GetType().GetMethod("NewMethodName").Invoke(wrapped, new object[] {1}));
      Assert.AreEqual("STATIC[2]", wrapped.GetType().GetMethod("GetStringStatic").Invoke(wrapped, new object[] {2}));
    }

    [Test] public void TeatStaticOverridenMethod()
    {
      object wrapped = wrapper.CreateWrapper(typeof (StaticWithSimpleArgs));      
      Assert.AreEqual("STATIC[2]", wrapped.GetType().GetMethod("GetStringStatic", new [] {typeof(int)}).Invoke(wrapped, new object[] {2}));
      Assert.AreEqual("STATIC[6]", wrapped.GetType().GetMethod("GetStringStatic", new [] {typeof(int), typeof(int)}).Invoke(wrapped, new object[] {2, 4}));
    }

    [Test] public void TestParamsMethodCalledWithNoArgs()
    {
      NonStaticWithSimpleArgs instance = new NonStaticWithSimpleArgs(1);
      object wrapped = wrapper.CreateWrapper(typeof (NonStaticWithSimpleArgs), new [] {new ProxyMethod(typeof(NonStaticWithSimpleArgs).GetMethod("ParamsMethod"), instance)});
      MethodInfo target = wrapped.GetType().GetMethod("ParamsMethod", new Type[0]);
      string ret = (string) target.Invoke(wrapped, null);
      Assert.AreEqual("ParamsMethod: ", ret);
    }

    [Test] public void TestParamsMethodCalledWithOneArgs()
    {
      NonStaticWithSimpleArgs instance = new NonStaticWithSimpleArgs(1);
      object wrapped = wrapper.CreateWrapper(typeof (NonStaticWithSimpleArgs), new [] {new ProxyMethod(typeof(NonStaticWithSimpleArgs).GetMethod("ParamsMethod"), instance)});
      MethodInfo target = wrapped.GetType().GetMethod("ParamsMethod", new[] {typeof (int)});
      string ret = (string) target.Invoke(wrapped, new object[] {1});
      Assert.AreEqual("ParamsMethod: 1", ret);
    }  
  }

  public static class StaticWithSimpleArgs
  {
    public static string GetStringStatic(int arg1)
    {
      return "STATIC[" + arg1 +"]";
    }

    public static string GetStringStatic(int arg1, int arg2)
    {
      return "STATIC[" + (arg1 + arg2) + "]";
    }
  }

  public class NonStaticWithSimpleArgs
  {
    private readonly int param1;
    private readonly int param2;

    public NonStaticWithSimpleArgs(int param1)
    {
      this.param1 = param1;
    }

    public NonStaticWithSimpleArgs(int param1, int param2)
    {
      this.param1 = param1;
      this.param2 = param2;
    }

    public string ParamsMethod(params int[] args)
    {
      return "ParamsMethod: " + (args == null ? "NULL" : String.Join(",", args));
    }

    public string GetStringNonStatic(int arg1)
    {
      return "INSTANCE[" + param1 +"][" + param2 + "][" + arg1 +"]";
    }
  }
}
