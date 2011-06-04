﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using js.net.jish.IL;
using NUnit.Framework;

namespace js.net.tests.jish
{
  [TestFixture] public class TypeILWrapperMethodArgumentsExpansionTests
  {      
    private readonly TypeILWrapper wrapper = new TypeILWrapper();
    
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

    [Test, ExpectedException(typeof(TargetParameterCountException))] public void TestCallingOptinalWithoutValue()
    {
      typeof (TestExpansion).GetMethod("SingleDefValue").Invoke(null, new object[0]);
    }

    [Test, ExpectedException(typeof(TargetParameterCountException))] public void TestCallingParamsWithoutArg()
    {
      typeof (TestExpansion).GetMethod("SingleParamsArg").Invoke(null, new object[0]);
    }

    [Test, ExpectedException(typeof(TargetParameterCountException))] public void TestCallingParamsWithoutArrayArg()
    {
      typeof (TestExpansion).GetMethod("SingleParamsArg").Invoke(null, new object[] {1, 2, 3});
    }

    private IEnumerable<MethodInfo> GetExpandedMethods(string methodName)
    {
      object wrapped = wrapper.CreateWrapper(typeof (TestExpansion), 
                                             new [] {new ProxyMethod(typeof(TestExpansion).GetMethod(methodName), null)});
      return wrapped.GetType().GetMethods().Where(mi => mi.Name.Equals(methodName)).OrderBy(mi => mi.GetParameters().Length);
    }
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

    // TODO!!
    public static void Weird(int a2 = 0, params string[] a1) 
    {
    }

  }
}