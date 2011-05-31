using NUnit.Framework;

namespace js.net.tests.jish
{
  [TestFixture] public class CreateAndLoadCommandTests : AbstractJishTest
  {    
    [Test] public void TestUsingRecognisedRegardlessOfParenthesisOrQuotes() 
    {             
      jish.ExecuteCommand(".create('System.IO.File', 'file')");
      Assert.AreEqual("Created instance of System.IO.File. Saved in global 'file'", console.GetLastMessage());
      console.Reset();

      jish.ExecuteCommand(".create (\"System.IO.File\", \"file\")");
      Assert.AreEqual("Created instance of System.IO.File. Saved in global 'file'", console.GetLastMessage());
      console.Reset();
    }

    [Test] public void TestUsingShortName()
    {
      jish.ExecuteCommand(".create('js.net.tests.jish.InstanceMock', 'im')");      
      Assert.AreEqual("Created instance of js.net.tests.jish.InstanceMock. Saved in global 'im'", console.GetLastMessage());
    }

    
    [Test] public void TestAssemblyLoad()
    {
      jish.ExecuteCommand(".create('PicNet2.CryptoUtils, PicNet2')");
      Assert.AreEqual("Could not find a matching type: PicNet2.CryptoUtils, PicNet2", console.GetLastMessage());
      jish.ExecuteCommand(@".assembly(..\..\..\lib\PicNet2.dll)");
      Assert.AreEqual("Assembly 'PicNet2' loaded.", console.GetLastMessage());
      jish.ExecuteCommand(".create('PicNet2.CryptoUtils, PicNet2', 'cu')");
      Assert.AreEqual("Created instance of PicNet2.CryptoUtils, PicNet2. Saved in global 'cu'", console.GetLastMessage());
    }

    [Test] public void TestOverloadedStaticMethods()
    {
      jish.ExecuteCommand(".create('js.net.tests.jish.InstanceMock, js.net.tests', 'im')");
      Assert.AreEqual("Created instance of js.net.tests.jish.InstanceMock, js.net.tests. Saved in global 'im'", console.GetLastMessage());
      jish.ExecuteCommand("console.log(im.OverridenStaticMember(1, 2));");
      Assert.AreEqual("Member 1 [12]", console.GetLastMessage());
      jish.ExecuteCommand("console.log(im.OverridenStaticMember(1, 2, 3));");
      Assert.AreEqual("Member 2 [123]", console.GetLastMessage());
    }        

    [Test] public void TestCallingWithParamsArgs()
    {
      jish.ExecuteCommand(".create('js.net.tests.jish.InstanceMock, js.net.tests', 'im')");
      Assert.AreEqual("Created instance of js.net.tests.jish.InstanceMock, js.net.tests. Saved in global 'im'", console.GetLastMessage());
      jish.ExecuteCommand("console.log(im.ObjectArgs(['test', true]));");
    }

    [Test] public void TestOverloadedNonStaticMethods()
    {
      jish.ExecuteCommand(".create('js.net.tests.jish.InstanceMock, js.net.tests', 'ns')");
      jish.ExecuteCommand("console.log(ns.OverridenNSMember(1, 2));");
      Assert.AreEqual("Non Satatic Member 1 [12]", console.GetLastMessage());
      jish.ExecuteCommand("console.log(ns.OverridenNSMember(1, 2, 3));");
      Assert.AreEqual("Non Satatic Member 2 [123]", console.GetLastMessage());
    }

    [Test] public void TestOverloadedStaticMethodsWithInstance()
    {
      jish.ExecuteCommand(".create('js.net.tests.jish.InstanceMock, js.net.tests', 'ns')");
      jish.ExecuteCommand("console.log(ns.OverridenStaticMember(1, 2));");
      Assert.AreEqual("Member 1 [12]", console.GetLastMessage());
      jish.ExecuteCommand("console.log(ns.OverridenStaticMember(1, 2, 3));");
      Assert.AreEqual("Member 2 [123]", console.GetLastMessage());
    }

    [Test] public void TestStaticClassOverloadedStaticMethodsWithInstance()
    {
      jish.ExecuteCommand(".create('js.net.tests.jish.StaticMock, js.net.tests', 'ns')");
      jish.ExecuteCommand("console.log(ns.OverridenStaticMember(1, 2));");
      Assert.AreEqual("Member 1 [12]", console.GetLastMessage());
      jish.ExecuteCommand("console.log(ns.OverridenStaticMember(1, 2, 3));");
      Assert.AreEqual("Member 2 [123]", console.GetLastMessage());
    }

    [Test, Ignore("Generic Methods not supported")] public void TestNonStaticOverloadsWithGenericArgs()
    {
      jish.ExecuteCommand(".create('js.net.tests.jish.InstanceMock, js.net.tests', 'ns')");
      jish.ExecuteCommand("console.log(ns.OverridenNSMemberWithGenericArgs(1, 2));");
      Assert.AreEqual("Non Satatic Member 1 [12]", console.GetLastMessage());
      jish.ExecuteCommand("console.log(ns.OverridenNSMemberWithGenericArgs(1, 2, 3));");
      Assert.AreEqual("Non Satatic Member 2 [123]", console.GetLastMessage());
    }

    [Test, Ignore("Generic Methods not supported")] public void TestStaticOverloadsWithGenericArgs()
    {
      jish.ExecuteCommand(".create('js.net.tests.jish.StaticMock, js.net.tests', 'ns')");
      jish.ExecuteCommand("console.log(ns.OverridenStaticMemberWithGenerics(1, 2));");
      Assert.AreEqual("Member 1 [12]", console.GetLastMessage());
      jish.ExecuteCommand("console.log(ns.OverridenStaticMemberWithGenerics(1, 2, 3));");
      Assert.AreEqual("Member 2 [123]", console.GetLastMessage());
    }
  }

  public static class StaticMock
  {
    public static string OverridenStaticMember(int arg1, int arg2)
    {
      return "Member 1 [" + arg1 + arg2 + "]";
    }

    public static string OverridenStaticMember(int arg1, int arg2, int arg3)
    {
      return "Member 2 [" + arg1 + arg2 + arg3 + "]";
    }

    public static string OverridenStaticMemberWithGenerics<T>(int arg1, int arg2)
    {
      return "Member 1 [" + arg1 + arg2 + "] T[" + typeof(T).Name + "]";
    }

    public static string OverridenStaticMemberWithGenerics<T>(int arg1, int arg2, int arg3)
    {
      return "Member 2 [" + arg1 + arg2 + arg3 + "] T[" + typeof(T).Name + "]";
    }
  }

  public class InstanceMock
  {
    public string OverridenNSMember(int arg1, int arg2)
    {
      return "Non Satatic Member 1 [" + arg1 + arg2 + "]";
    }

    public string OverridenNSMember(int arg1, int arg2, int arg3)
    {
      return "Non Satatic Member 2 [" + arg1 + arg2 + arg3 + "]";
    }

    public string OverridenNSMemberWithGenericArgs<T>(int arg1, int arg2)
    {
      return "Non Satatic Member 1 [" + arg1 + arg2 + "] T[" + typeof(T).Name + "]";
    }

    public string OverridenNSMemberWithGenericArgs<T>(int arg1, int arg2, int arg3)
    {
      return "Non Satatic Member 2 [" + arg1 + arg2 + arg3 + "] T[" + typeof(T).Name + "]";
    }

    public static string OverridenStaticMember(int arg1, int arg2)
    {
      return "Member 1 [" + arg1 + arg2 + "]";
    }

    public static string OverridenStaticMember(int arg1, int arg2, int arg3)
    {
      return "Member 2 [" + arg1 + arg2 + arg3 + "]";
    }

    public static string ObjectMember(object o1)
    {
      return "Member 3 [" + o1 + "]";
    }

    public static object ObjectArgs(params object[] args)
    {
      return "Member 4 [" + args + "]";
    }
  }
}
