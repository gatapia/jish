using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace js.net.tests.jish.TypeILWrapper
{
  [TestFixture] public class TypeILWrapperMethodArgumentsExpansionTests : AbstractTypeILWrapperTest
  {
    public TypeILWrapperMethodArgumentsExpansionTests() : base(typeof(TestExpansion)) {}

    [Test] public void TestNoArg()
    {
      IEnumerable<MethodInfo> expanded = GetExpandedMethods("NoArg");
      Assert.AreEqual(1, expanded.Count());
      Assert.AreEqual(0, expanded.Single().GetParameters().Length);
    }

    [Test] public void TestOneArg()
    {
      IEnumerable<MethodInfo> expanded = GetExpandedMethods("OneArg");
      Assert.AreEqual(1, expanded.Count());
      Assert.AreEqual(1, expanded.Single().GetParameters().Length);
      Assert.AreEqual(typeof(int), expanded.Single().GetParameters()[0].ParameterType);
    }

    [Test] public void TestTwoArg()
    {
      IEnumerable<MethodInfo> expanded = GetExpandedMethods("TwoArg");
      Assert.AreEqual(1, expanded.Count());
      Assert.AreEqual(2, expanded.Single().GetParameters().Length);
      Assert.AreEqual(typeof(string), expanded.Single().GetParameters()[0].ParameterType);
      Assert.AreEqual(typeof(object), expanded.Single().GetParameters()[1].ParameterType);
    }

    [Test] public void TestSingleParamsArg()
    {
      IEnumerable<MethodInfo> expanded = GetExpandedMethods("SingleParamsArg");
      Assert.AreEqual(17, expanded.Count()); // No args + 1, 2, 3, ... 16 args
      for (int i = 0; i < 17; i++)
      {
        MethodInfo mi = expanded.ElementAt(i);
        Assert.AreEqual(i, mi.GetParameters().Length);
        Assert.IsTrue(mi.GetParameters().All(pi => pi.ParameterType == typeof(int)));
      }
    }    

    [Test] public void TestOneStringThenParamsArg()
    {      
      IEnumerable<MethodInfo> expanded = GetExpandedMethods("OneStringThenParamsArg");
      Assert.AreEqual(17, expanded.Count()); // str + str + 1, + 2, + 3, ... + 16 args
      for (int i = 0; i < 17; i++)
      {
        MethodInfo mi = expanded.ElementAt(i);
        Assert.AreEqual(i + 1, mi.GetParameters().Length);
        Assert.AreEqual(typeof(string), mi.GetParameters()[0].ParameterType);
        Assert.IsTrue(mi.GetParameters().Skip(1).All(pi => pi.ParameterType == typeof(int)));
      }
    } 

    [Test] public void TestSingleDefValue()
    {      
      IEnumerable<MethodInfo> expanded = GetExpandedMethods("SingleDefValue");
      Assert.AreEqual(2, expanded.Count()); 
      Assert.AreEqual(0, expanded.ElementAt(0).GetParameters().Length);
      Assert.AreEqual(1, expanded.ElementAt(1).GetParameters().Length);
      Assert.AreEqual(typeof(int), expanded.ElementAt(1).GetParameters()[0].ParameterType);
    } 

    [Test] public void OneStringThenSingleDefValue()
    {      
      IEnumerable<MethodInfo> expanded = GetExpandedMethods("OneStringThenSingleDefValue");
      Assert.AreEqual(2, expanded.Count()); 
      
      Assert.AreEqual(1, expanded.ElementAt(0).GetParameters().Length);
      Assert.AreEqual(typeof(string), expanded.ElementAt(0).GetParameters()[0].ParameterType);

      Assert.AreEqual(2, expanded.ElementAt(1).GetParameters().Length);
      Assert.AreEqual(typeof(string), expanded.ElementAt(1).GetParameters()[0].ParameterType);
      Assert.AreEqual(typeof(int), expanded.ElementAt(1).GetParameters()[1].ParameterType);
    } 

    [Test] public void TwoStringsThenTwoDefValue()
    {      
      IEnumerable<MethodInfo> expanded = GetExpandedMethods("TwoStringsThenTwoDefValue");
      Assert.AreEqual(3, expanded.Count()); 
      
      Assert.AreEqual(2, expanded.ElementAt(0).GetParameters().Length);
      Assert.AreEqual(typeof(string), expanded.ElementAt(0).GetParameters()[0].ParameterType);
      Assert.AreEqual(typeof(string), expanded.ElementAt(0).GetParameters()[1].ParameterType);

      Assert.AreEqual(3, expanded.ElementAt(1).GetParameters().Length);
      Assert.AreEqual(typeof(string), expanded.ElementAt(1).GetParameters()[0].ParameterType);
      Assert.AreEqual(typeof(string), expanded.ElementAt(1).GetParameters()[1].ParameterType);
      Assert.AreEqual(typeof(int), expanded.ElementAt(1).GetParameters()[2].ParameterType);

      Assert.AreEqual(4, expanded.ElementAt(2).GetParameters().Length);
      Assert.AreEqual(typeof(string), expanded.ElementAt(2).GetParameters()[0].ParameterType);
      Assert.AreEqual(typeof(string), expanded.ElementAt(2).GetParameters()[1].ParameterType);
      Assert.AreEqual(typeof(int), expanded.ElementAt(2).GetParameters()[2].ParameterType);
      Assert.AreEqual(typeof(int), expanded.ElementAt(2).GetParameters()[3].ParameterType);
    } 

    [Test] public void OneDefAndOneParams()
    {
      IEnumerable<MethodInfo> expanded = GetExpandedMethods("OneDefAndOneParams");
      Assert.AreEqual(18, expanded.Count());
      
      ParameterInfo[] ps = expanded.ElementAt(0).GetParameters();
      Assert.AreEqual(0, ps.Length);

      ps = expanded.ElementAt(1).GetParameters();
      Assert.AreEqual(1, ps.Length);
      Assert.AreEqual(typeof(int), ps[0].ParameterType);

      for (int i = 2; i < expanded.Count(); i++)
      {
        ps = expanded.ElementAt(i).GetParameters();
        Assert.AreEqual(i, ps.Length);
        Assert.IsTrue(ps.All(p => p.ParameterType == typeof(int))); 
      }      
    }

    [Test] public void TwoDefAndOneParams()
    {
      IEnumerable<MethodInfo> expanded = GetExpandedMethods("TwoDefAndOneParams");
      Assert.AreEqual(19, expanded.Count());
      
      ParameterInfo[] ps = expanded.ElementAt(0).GetParameters();
      Assert.AreEqual(0, ps.Length);

      ps = expanded.ElementAt(1).GetParameters();
      Assert.AreEqual(1, ps.Length);
      Assert.AreEqual(typeof(int), ps[0].ParameterType);

      ps = expanded.ElementAt(2).GetParameters();
      Assert.AreEqual(2, ps.Length);
      Assert.AreEqual(typeof(int), ps[0].ParameterType);
      Assert.AreEqual(typeof(object), ps[1].ParameterType);

      for (int i = 3; i < expanded.Count(); i++)
      {
        ps = expanded.ElementAt(i).GetParameters();
        Assert.AreEqual(i, ps.Length);
        Assert.IsTrue(ps.Skip(2).All(p => p.ParameterType == typeof (int)));
      }
    }

    [Test] public void OneStringTwoDefAndOneParams()
    {
      IEnumerable<MethodInfo> expanded = GetExpandedMethods("OneStringTwoDefAndOneParams");
      Assert.AreEqual(19, expanded.Count());
      
      ParameterInfo[] ps = expanded.ElementAt(0).GetParameters();
      Assert.AreEqual(1, ps.Length);
      Assert.AreEqual(typeof(string), ps[0].ParameterType);

      ps = expanded.ElementAt(1).GetParameters();
      Assert.AreEqual(2, ps.Length);
      Assert.AreEqual(typeof(string), ps[0].ParameterType);
      Assert.AreEqual(typeof(int), ps[1].ParameterType);

      ps = expanded.ElementAt(2).GetParameters();
      Assert.AreEqual(3, ps.Length);
      Assert.AreEqual(typeof(string), ps[0].ParameterType);
      Assert.AreEqual(typeof(int), ps[1].ParameterType);
      Assert.AreEqual(typeof(object), ps[2].ParameterType);

      for (int i = 3; i < expanded.Count(); i++)
      {
        ps = expanded.ElementAt(i).GetParameters();
        Assert.AreEqual(i + 1, ps.Length);
        Assert.IsTrue(ps.Skip(3).All(p => p.ParameterType == typeof (int)));
      }
    }

    [Test] public void SingleGen()
    {
      IEnumerable<MethodInfo> expanded = GetExpandedMethods("SingleGen");
      Assert.AreEqual(1, expanded.Count());
      Assert.AreEqual(1, expanded.ElementAt(0).GetParameters().Length);
      Assert.AreEqual(typeof(object), expanded.ElementAt(0).GetParameters().First().ParameterType);
    }

    [Test] public void DoubleGen()
    {
      IEnumerable<MethodInfo> expanded = GetExpandedMethods("DoubleGen");
      Assert.AreEqual(1, expanded.Count());
      Assert.AreEqual(2, expanded.ElementAt(0).GetParameters().Length);
      Assert.AreEqual(typeof(object), expanded.ElementAt(0).GetParameters().First().ParameterType);
      Assert.AreEqual(typeof(object), expanded.ElementAt(0).GetParameters().ElementAt(1).ParameterType);
    }

    [Test] public void IntAndSingleGen()
    {
      IEnumerable<MethodInfo> expanded = GetExpandedMethods("IntAndSingleGen");
      Assert.AreEqual(1, expanded.Count());
      Assert.AreEqual(2, expanded.ElementAt(0).GetParameters().Length);
      Assert.AreEqual(typeof(int), expanded.ElementAt(0).GetParameters().First().ParameterType);
      Assert.AreEqual(typeof(object), expanded.ElementAt(0).GetParameters().ElementAt(1).ParameterType);
    }

    [Test] public void IntAndDoubleGen()
    {
      IEnumerable<MethodInfo> expanded = GetExpandedMethods("IntAndDoubleGen");
      Assert.AreEqual(1, expanded.Count());
      Assert.AreEqual(3, expanded.ElementAt(0).GetParameters().Length);
      Assert.AreEqual(typeof(int), expanded.ElementAt(0).GetParameters().First().ParameterType);
      Assert.AreEqual(typeof(object), expanded.ElementAt(0).GetParameters().ElementAt(1).ParameterType);
      Assert.AreEqual(typeof(object), expanded.ElementAt(0).GetParameters().ElementAt(2).ParameterType);
    }
  
    [Test, ExpectedException(typeof(TargetParameterCountException))] public void TestCallingOptinalWithoutValue() { typeof (TestExpansion).GetMethod("SingleDefValue").Invoke(null, new object[0]); }
    [Test, ExpectedException(typeof(TargetParameterCountException))] public void TestCallingParamsWithoutArg() { typeof (TestExpansion).GetMethod("SingleParamsArg").Invoke(null, new object[0]); }
    [Test, ExpectedException(typeof(TargetParameterCountException))] public void TestCallingParamsWithoutArrayArg() { typeof (TestExpansion).GetMethod("SingleParamsArg").Invoke(null, new object[] {1, 2, 3}); }
  }

  public static class TestExpansion
  {
    public static void NoArg() {}
    public static void OneArg(int a1) {}
    public static void TwoArg(string a1, object a2) {}

    public static void SingleParamsArg(params int[] args) {}
    public static void OneStringThenParamsArg(string strArg, params int[] args) {}

    public static void SingleDefValue(int a1 = 1) {}
    public static void OneStringThenSingleDefValue(string strArg, int a1 = 1) {}
    public static void TwoStringsThenTwoDefValue(string strArg1, string strArg2, int a1 = 1, int a2 = 2) {}

    public static void OneDefAndOneParams(int a1 = 1, params int[] args) { }
    public static void TwoDefAndOneParams(int a1 = 1, object a2 = null, params int[] args) { }
    public static void OneStringTwoDefAndOneParams(string strArg1, int a1 = 1, object a2 = null, params int[] args) { }

    public static void SingleGen<T>(T arg1) { }
    public static void DoubleGen<T, U>(T arg1, U arg2) { }
    public static void IntAndSingleGen<T>(int intarg1, T arg1) { }
    public static void IntAndDoubleGen<T, U>(int intarg1, T arg1, U arg2) { }    
  }
}
