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
    private readonly MethodInfo miStatic = typeof (StaticClz).GetMethod("GetStringStatic", new [] {typeof(int)});
    private readonly MethodInfo miInstance = typeof (InstanceClz).GetMethod("GetStringNonStatic");

    [Test] public void TestStaticTypeWithSimpleArgs()
    {
      object wrapped = wrapper.CreateWrapper(typeof(StaticClz));
      Assert.AreEqual("STATIC[123]", miStatic.Invoke(wrapped, new object[] {123}));
    }

    [Test] public void TestNonStaticWithSimpleArgs()
    {
      object instance = new InstanceClz(222);
      object wrapped = wrapper.CreateWrapper(instance);
      Assert.AreEqual("INSTANCE[222][0][123]", wrapped.GetType().GetMethod("GetStringNonStatic").Invoke(wrapped, new object[] {123}));
    }

    [Test] public void TestNonStaticWithMultipleArgs()
    {
      object instance = new InstanceClz(222, 333);
      object wrapped = wrapper.CreateWrapper(instance);
      Assert.AreEqual("INSTANCE[222][333][123]", wrapped.GetType().GetMethod("GetStringNonStatic").Invoke(wrapped, new object[] {123}));
    }

    [Test] public void TestMergeMethodsInSingleType()
    {
      object instance = new InstanceClz(222);
      object wrapped = wrapper.CreateWrapper(typeof(StaticClz), new [] {new ProxyMethod(miStatic, null), new ProxyMethod(miInstance, instance)});
      Assert.AreEqual("INSTANCE[222][0][1]", wrapped.GetType().GetMethod("GetStringNonStatic").Invoke(wrapped, new object[] {1}));
      Assert.AreEqual("STATIC[2]", wrapped.GetType().GetMethod("GetStringStatic").Invoke(wrapped, new object[] {2}));
    }

    [Test] public void TestMergeMethodsInSingleTypeWithMultipleInstances()
    {
      object instance = new InstanceClz(222);
      object instance2 = new InstanceClz(2, 3);
      object wrapped = wrapper.CreateWrapper(typeof(StaticClz), new [] {new ProxyMethod(miStatic, null), new ProxyMethod(miInstance, instance), new ProxyMethod(miInstance, instance2) { OverrideMethodName = "NewMethodName"}});
      Assert.AreEqual("INSTANCE[222][0][1]", wrapped.GetType().GetMethod("GetStringNonStatic").Invoke(wrapped, new object[] {1}));
      Assert.AreEqual("INSTANCE[2][3][1]", wrapped.GetType().GetMethod("NewMethodName").Invoke(wrapped, new object[] {1}));
      Assert.AreEqual("STATIC[2]", wrapped.GetType().GetMethod("GetStringStatic").Invoke(wrapped, new object[] {2}));
    }

    [Test] public void TeatStaticOverridenMethod()
    {
      object wrapped = wrapper.CreateWrapper(typeof (StaticClz));      
      Assert.AreEqual("STATIC[2]", wrapped.GetType().GetMethod("GetStringStatic", new [] {typeof(int)}).Invoke(wrapped, new object[] {2}));
      Assert.AreEqual("STATIC[6]", wrapped.GetType().GetMethod("GetStringStatic", new [] {typeof(int), typeof(int)}).Invoke(wrapped, new object[] {2, 4}));
    }

    [Test] public void TestParamsMethodCalledWithNoArgs()
    {
      InstanceClz instance = new InstanceClz(1);
      object wrapped = wrapper.CreateWrapper(typeof (InstanceClz), new [] {new ProxyMethod(typeof(InstanceClz).GetMethod("ParamsMethod"), instance)});
      MethodInfo target = wrapped.GetType().GetMethod("ParamsMethod", new Type[0]);
      string ret = (string) target.Invoke(wrapped, null);
      Assert.AreEqual("ParamsMethod: ", ret);
    }

    [Test] public void TestParamsMethodCalledWithOneArgs()
    {
      InstanceClz instance = new InstanceClz(1);
      object wrapped = wrapper.CreateWrapper(typeof (InstanceClz), new [] {new ProxyMethod(typeof(InstanceClz).GetMethod("ParamsMethod"), instance)});
      MethodInfo target = wrapped.GetType().GetMethod("ParamsMethod", new[] {typeof (int)});
      string ret = (string) target.Invoke(wrapped, new object[] {1});
      Assert.AreEqual("ParamsMethod: 1", ret);
    }  

    [Test] public void TestSingleParamsArgWithNoArgs()
    {
      Assert.AreEqual("null", InvokeWrapped("SingleParamsArg", new object[] {}));
    }    

    [Test] public void TestSingleParamsArgWithOneArgs()
    {
     Assert.AreEqual("1", InvokeWrapped("SingleParamsArg", new object[] {1})); 
    }

    [Test] public void TestSingleParamsArgWithTwoArgs()
    {
     Assert.AreEqual("1,2", InvokeWrapped("SingleParamsArg", new object[] {1, 2}));  
    }

    [Test] public void TestOneStringThenParamsArgWithNoArgs()
    {      
      Assert.AreEqual("strnull", InvokeWrapped("OneStringThenParamsArg", new object[] {"str"}));
    }    

    [Test] public void TestOneStringThenParamsArgWithOneArgs()
    {
     Assert.AreEqual("str1", InvokeWrapped("OneStringThenParamsArg", new object[] {"str", 1})); 
    }

    [Test] public void TestOneStringThenParamsArgWithTwoArgs()
    {
     Assert.AreEqual("str1,2", InvokeWrapped("OneStringThenParamsArg", new object[] {"str", 1, 2}));  
    }

    private string InvokeWrapped(string methodName, object[] args)
    {
      object wrapped = wrapper.CreateWrapper(typeof (ComplexClz), new [] {new ProxyMethod(typeof(ComplexClz).GetMethod(methodName), null)});
      MethodInfo mi = wrapped.GetType().GetMethod(methodName, args.Select(a => a.GetType()).ToArray());
      return (string) mi.Invoke(wrapped, args);
    }
  }

  public static class ComplexClz
  {
    public static string SingleParamsArg(params int[] args) { return ToParamsString(args); }
    public static string OneStringThenParamsArg(string strArg, params int[] args) { return strArg + ToParamsString(args); }
                  
    public static string SingleRefParamsArg(params object[] args) { return ToParamsString(args); }
    public static string OneStringThenRefParamsArg(string strArg, params object[] args) { return strArg + ToParamsString(args); }

    private static string ToParamsString<T>(IEnumerable<T> args)
    {
      if (args == null || !args.Any())
      {
        return "null";
      }
      return String.Join(",", args);
    }

    public static void SingleDefValue(int a1 = 1) {}
    public static void OneStringThenSingleDefValue(string strArg, int a1 = 1) {}
    public static void TwoStringsThenTwoDefValue(string strArg1, string strArg2, object a1 = null, object a2 = null) {}

    public static void SingleRefDefValue(int a1 = 1) {}
    public static void OneStringThenSingleRefDefValue(string strArg, int a1 = 1) {}
    public static void TwoStringsThenTwoRefDefValue(string strArg1, string strArg2, object a1 = null, object a2 = null) {}
  }

  public static class StaticClz
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

  public class InstanceClz
  {
    private readonly int param1;
    private readonly int param2;

    public InstanceClz(int param1)
    {
      this.param1 = param1;
    }

    public InstanceClz(int param1, int param2)
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
