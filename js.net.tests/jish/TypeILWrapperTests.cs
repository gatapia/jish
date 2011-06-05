using System;
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
      object wrapped = wrapper.CreateWrapper(typeof(StaticClz), new [] {new MethodToProxify(miStatic, null), new MethodToProxify(miInstance, instance)});
      Assert.AreEqual("INSTANCE[222][0][1]", wrapped.GetType().GetMethod("GetStringNonStatic").Invoke(wrapped, new object[] {1}));
      Assert.AreEqual("STATIC[2]", wrapped.GetType().GetMethod("GetStringStatic").Invoke(wrapped, new object[] {2}));
    }

    [Test] public void TestMergeMethodsInSingleTypeWithMultipleInstances()
    {
      object instance = new InstanceClz(222);
      object instance2 = new InstanceClz(2, 3);
      object wrapped = wrapper.CreateWrapper(typeof(StaticClz), new [] {new MethodToProxify(miStatic, null), new MethodToProxify(miInstance, instance), new MethodToProxify(miInstance, instance2) { OverrideMethodName = "NewMethodName"}});
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
      object wrapped = wrapper.CreateWrapper(typeof (InstanceClz), new [] {new MethodToProxify(typeof(InstanceClz).GetMethod("ParamsMethod"), instance)});
      MethodInfo target = wrapped.GetType().GetMethod("ParamsMethod", new Type[0]);
      string ret = (string) target.Invoke(wrapped, null);
      Assert.AreEqual("ParamsMethod: ", ret);
    }

    [Test] public void TestParamsMethodCalledWithOneArgs()
    {
      InstanceClz instance = new InstanceClz(1);
      object wrapped = wrapper.CreateWrapper(typeof (InstanceClz), new [] {new MethodToProxify(typeof(InstanceClz).GetMethod("ParamsMethod"), instance)});
      MethodInfo target = wrapped.GetType().GetMethod("ParamsMethod", new[] {typeof (int)});
      string ret = (string) target.Invoke(wrapped, new object[] {1});
      Assert.AreEqual("ParamsMethod: 1", ret);
    }  

    // SingleParamsArg
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

    // OneStringThenParamsArg
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

    // SingleRefParamsArg
    [Test] public void TestSingleRefParamsArgWithNoArgs()
    {
      Assert.AreEqual("null", InvokeWrapped("SingleRefParamsArg", new object[] {}));
    }    

    [Test] public void TestSingleRefParamsArgWithOneArgs()
    {
     Assert.AreEqual("c1", InvokeWrapped("SingleRefParamsArg", new object[] {new TestO("c1")})); 
    }

    [Test] public void TestSingleRefParamsArgWithTwoArgs()
    {
     Assert.AreEqual("c1,c2", InvokeWrapped("SingleRefParamsArg", new object[] {new TestO("c1"), new TestO("c2")}));  
    }

    // OneStringThenRefParamsArg
    [Test] public void TestOneStringThenRefParamsArgWithNoArgs()
    {      
      Assert.AreEqual("strnull", InvokeWrapped("OneStringThenRefParamsArg", new object[] {"str"}));
    }    

    [Test] public void TestOneStringThenRefParamsArgWithOneArgs()
    {
      Assert.AreEqual("strc1", InvokeWrapped("OneStringThenRefParamsArg", new object[] {"str", new TestO("c1") })); 
    }

    [Test] public void TestOneStringThenRefParamsArgWithTwoArgs()
    {
     Assert.AreEqual("strc1,c2", InvokeWrapped("OneStringThenRefParamsArg", new object[] {"str", new TestO("c1"), new TestO("c2")}));  
    }

    // SingleDefValue
    [Test] public void TestSingleDefValueWithNoArgs()
    {
     Assert.AreEqual("1", InvokeWrapped("SingleDefValue", new object[] {}));  
    }

    [Test] public void TestSingleDefValueWithArg()
    {
     Assert.AreEqual("10", InvokeWrapped("SingleDefValue", new object[] {10}));  
    }

    // OneStringThenSingleDefValue
    [Test] public void TestOneStringThenSingleDefValueWithNoArg()
    {
     Assert.AreEqual("str1,1", InvokeWrapped("OneStringThenSingleDefValue", new object[] {"str1"}));  
    }

    [Test] public void TestSingleDefValueWithArgs()
    {
     Assert.AreEqual("str1,9", InvokeWrapped("OneStringThenSingleDefValue", new object[] {"str1", 9}));  
    }

    // TwoStringsThenTwoDefValue
    [Test] public void TestTwoStringsThenTwoDefValueWithNoOptionals()
    {
     Assert.AreEqual("str1,str2,1,2", InvokeWrapped("TwoStringsThenTwoDefValue", new object[] {"str1", "str2"}));  
    }

    [Test] public void TestTwoStringsThenTwoDefValueWithOneArgs()
    {
     Assert.AreEqual("str1,str2,9,2", InvokeWrapped("TwoStringsThenTwoDefValue", new object[] {"str1", "str2", 9}));  
    }

    [Test] public void TestTwoStringsThenTwoDefValueWithTwoArgs()
    {
     Assert.AreEqual("str1,str2,9,111", InvokeWrapped("TwoStringsThenTwoDefValue", new object[] {"str1", "str2", 9, 111}));  
    }

    // SingleRefDefValue
    [Test] public void TestSingleRefDefValueWithNoArgs()
    {
     Assert.AreEqual("1", InvokeWrapped("SingleRefDefValue", new object[] {}));  
    }

    [Test] public void TestSingleRefDefValueWithArg()
    {
     Assert.AreEqual("10", InvokeWrapped("SingleRefDefValue", new object[] {10}));  
    }

    // OneStringThenSingleRefDefValue
    [Test] public void TestOneStringThenSingleRefDefValueWithNoArg()
    {
     Assert.AreEqual("str1,1", InvokeWrapped("OneStringThenSingleRefDefValue", new object[] {"str1"}));  
    }

    [Test] public void TestSingleRefDefValueWithArgs()
    {
     Assert.AreEqual("str1,9", InvokeWrapped("OneStringThenSingleRefDefValue", new object[] {"str1", 9}));  
    }

    // TwoStringsThenTwoRefDefValue
    [Test] public void TestTwoStringsThenTwoRefDefValueWithNoOptionals()
    {
     Assert.AreEqual("str1,str2,null,null", InvokeWrapped("TwoStringsThenTwoRefDefValue", new object[] {"str1", "str2"}));  
    }

    [Test] public void TestTwoStringsThenTwoRefDefValueWithOneArgs()
    {
     Assert.AreEqual("str1,str2,c9,null", InvokeWrapped("TwoStringsThenTwoRefDefValue", new object[] {"str1", "str2", new TestO("c9")}));  
    }

    [Test] public void TestTwoStringsThenTwoRefDefValueWithTwoArgs()
    {
     Assert.AreEqual("str1,str2,c9,c111", InvokeWrapped("TwoStringsThenTwoRefDefValue", new object[] {"str1", "str2", new TestO("c9"), new TestO("c111")}));  
    }

    // OneDefAndOneParams
    [Test] public void TestOneDefAndOneParamsWithNoArgs()
    {
      Assert.AreEqual("1null", InvokeWrapped("OneDefAndOneParams", new object[] {}));  
    }

    [Test] public void TestOneDefAndOneParamsWithDefArg()
    {
      Assert.AreEqual("2null", InvokeWrapped("OneDefAndOneParams", new object[] {2}));  
    }

    [Test] public void TestOneDefAndOneParamsWithBothArgs()
    {
      Assert.AreEqual("31,2,3", InvokeWrapped("OneDefAndOneParams", new object[] {3, 1, 2, 3}));  
    }

    // TwoDefAndOneParams
    [Test] public void TestTwoDefAndOneParamsWithNoArgs()
    {
      Assert.AreEqual("12null", InvokeWrapped("TwoDefAndOneParams", new object[] {}));  
    }

    [Test] public void TestTwoDefAndOneParamsWithOneDefArg()
    {
      Assert.AreEqual("42null", InvokeWrapped("TwoDefAndOneParams", new object[] {4}));  
    }

    [Test] public void TestTwoDefAndOneParamsWithTwoDefArgs()
    {
      Assert.AreEqual("46null", InvokeWrapped("TwoDefAndOneParams", new object[] {4,6}));  
    }

    [Test] public void TestTwoDefAndOneParamsWithAllThreeArgs()
    {
      Assert.AreEqual("467,8,9", InvokeWrapped("TwoDefAndOneParams", new object[] {4, 6, 7, 8, 9}));  
    }

    // OneStringTwoDefAndOneParams
    [Test] public void TestOneStringTwoDefAndOneParamsWithNoArgs()
    {                      
      Assert.AreEqual("str12null", InvokeWrapped("OneStringTwoDefAndOneParams", new object[] {"str"}));  
    }                      
                           
    [Test] public void TestOneStringTwoDefAndOneParamsWithOneDefArg()
    {                      
      Assert.AreEqual("str92null", InvokeWrapped("OneStringTwoDefAndOneParams", new object[] {"str",9}));  
    }                      
                           
    [Test] public void TestOneStringTwoDefAndOneParamsWithTwoDefArgs()
    {                      
      Assert.AreEqual("str98null", InvokeWrapped("OneStringTwoDefAndOneParams", new object[] {"str",9,8}));  
    }                      
                           
    [Test] public void TestOneStringTwoDefAndOneParamsWithAllThreeArgs()
    {
      Assert.AreEqual("str981,2,3", InvokeWrapped("OneStringTwoDefAndOneParams", new object[] {"str",9,8,1,2,3}));  
    }

    private string InvokeWrapped(string methodName, object[] args)
    {
      object wrapped = wrapper.CreateWrapper(typeof (ComplexClz), new [] {new MethodToProxify(typeof(ComplexClz).GetMethod(methodName), null)});
      MethodInfo mi = wrapped.GetType().GetMethod(methodName, args.Select(a => a.GetType()).ToArray());
      return (string) mi.Invoke(wrapped, args);
    }
  }

  public class TestO
  {
    private readonly string content;

    public TestO(string content)
    {
      this.content = content;
    }

    public override string ToString()
    {
      return content;
    }
  }

  public static class ComplexClz
  {
    public static string SingleParamsArg(params int[] args) { return ToParamsString(args); }
    public static string OneStringThenParamsArg(string strArg, params int[] args) { return strArg + ToParamsString(args); }
                  
    public static string SingleRefParamsArg(params object[] args) { return ToParamsString(args); }
    public static string OneStringThenRefParamsArg(string strArg, params object[] args) { return strArg + ToParamsString(args); }    

    public static string SingleDefValue(int a1 = 1) { return ToParamsString(a1); }
    public static string OneStringThenSingleDefValue(string strArg, int a1 = 1) { return ToParamsString(strArg, (object) a1); }
    public static string TwoStringsThenTwoDefValue(string strArg1, string strArg2, int a1 = 1, int a2 = 2) { return ToParamsString(strArg1, strArg2, (object) a1, a2); }
                  
    public static string SingleRefDefValue(int a1 = 1) { return ToParamsString(a1); }    
    public static string OneStringThenSingleRefDefValue(string strArg, int a1 = 1) { return ToParamsString(strArg, (object) a1); }
    public static string TwoStringsThenTwoRefDefValue(string strArg1, string strArg2, object a1 = null, object a2 = null) { return ToParamsString(strArg1, strArg2, a1, a2); }

    public static string OneDefAndOneParams(int a1 = 1, params int[] args) { return a1 + ToParamsString(args); }
    public static string TwoDefAndOneParams(int a1 = 1, object a2 = null, params int[] args) { return a1.ToString() + a2 + ToParamsString(args); }
    public static string OneStringTwoDefAndOneParams(string strArg1, int a1 = 1, object a2 = null, params int[] args) { return strArg1 + a1 + a2 + ToParamsString(args); }

    private static string ToParamsString<T>(params T[] args)
    {
      if (args == null || !args.Any())
      {
        return "null";
      }
      return String.Join(",", args.Select(a => a == null ? "null" : a.ToString()));
    }
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
