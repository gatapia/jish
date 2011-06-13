using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace js.net.tests.jish.TypeILWrapper
{
  /**
   * Steps to support delegates:   
   * + Rename method to <methodname>_internal
   * + <methodname>_internal should replace the delegate param with a string 
   *   (id of real method in jish.internal scope)
   * - Generate JS method with original name/namespace
   *     - Generate <newid>
   *     - Save the callback in the jish.internal['<newid>']   
   *     - Call and <methodname>_internal, with a string arg (<newid>)
   * - Create mock Action / Func
   * - Execute original method (C#) with mock action
   * - When mock Action / Func gets called call original callback in JS
   */
  [TestFixture] public class DelegateSupportTests : AbstractTypeILWrapperTest
  {    
    public DelegateSupportTests() : base(typeof(TestDel)) {}
    

    [Test] public void Rename_method_to_methodname_internal()
    {
      var method = GetExpandedMethods("CallbackMethod", "CallbackMethod_internal").Single();
      Assert.AreEqual("CallbackMethod_internal", method.Name);
    }

    [Test] public void Methodname_internal_should_replace_the_delegate_param_with_a_string()
    {
      var method1 = GetExpandedMethods("CallbackMethod", "CallbackMethod_internal").Single();
      var method2 = GetExpandedMethods("CallbackMethodWithAdditionalArg", "CallbackMethodWithAdditionalArg_internal").Single();
     
      Assert.AreEqual(typeof(string), method1.GetParameters().Single().ParameterType);
      Assert.AreEqual(typeof(object), method2.GetParameters().ElementAt(0).ParameterType);
      Assert.AreEqual(typeof(string), method2.GetParameters().ElementAt(1).ParameterType);
    }

    [Test] public void TestCreatingCallbackMethodWorks()
    {
      GetExpandedMethods("CallbackMethodWithReturnType");
      GetExpandedMethods("CallbackMethod");
      GetExpandedMethods("CallbackMethodWithAdditionalArg");
    }

    [Test, ExpectedException(typeof(NotSupportedException))] public void TestMultiDelegateParamsNotSupported()
    {
      GetExpandedMethods("InvalidCallbackMethod"); 
    }
  }

  public static class TestDel
  {
    public static string CallbackMethodWithReturnType(Action<int> cb) { return String.Empty;  }
    public static void CallbackMethod(Action<int> cb) { }
    public static void CallbackMethodWithAdditionalArg(object arg1, Action<int> cb) { }
    public static void InvalidCallbackMethod(object arg1, Action<int> cb, Action<int> cb2) { }
  }
}