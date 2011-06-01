using NUnit.Framework;

namespace js.net.tests.jish.Command
{
  [TestFixture] public class CreateAndLoadCommandTests : AbstractJishTest
  {    
    [Test] public void TestUsingRecognisedRegardlessOfParenthesisOrQuotes() 
    {             
      jish.ExecuteCommand("var file = jish.create('System.IO.File');");
      Assert.AreEqual("Created instance of System.IO.File.", console.GetLastMessage());
      console.Reset();

      jish.ExecuteCommand("var file = jish.create (\"System.IO.File\");");
      Assert.AreEqual("Created instance of System.IO.File.", console.GetLastMessage());
      console.Reset();
    }

    [Test] public void TestUsingShortName()
    {
      jish.ExecuteCommand("var im = jish.create('js.net.tests.jish.Command.InstanceMock');");      
      Assert.AreEqual("Created instance of js.net.tests.jish.Command.InstanceMock.", console.GetLastMessage());
    }

    
    [Test] public void TestAssemblyLoad()
    {
      jish.ExecuteCommand("var cu = jish.create('PicNet2.CryptoUtils, PicNet2');");
      Assert.AreEqual("Could not find a matching type: PicNet2.CryptoUtils, PicNet2", console.GetLastMessage());
      jish.ExecuteCommand(@"jish.assembly('..\\..\\..\\lib\\PicNet2.dll')");
      Assert.AreEqual("Assembly 'PicNet2' loaded.", console.GetLastMessage());
      jish.ExecuteCommand("var cu = jish.create('PicNet2.CryptoUtils, PicNet2');");
      Assert.AreEqual("Created instance of PicNet2.CryptoUtils, PicNet2.", console.GetLastMessage());
    }

    [Test] public void TestOverloadedStaticMethods()
    {
      jish.ExecuteCommand("var im = jish.create('js.net.tests.jish.Command.InstanceMock, js.net.tests');");
      Assert.AreEqual("Created instance of js.net.tests.jish.Command.InstanceMock, js.net.tests.", console.GetLastMessage());
      jish.ExecuteCommand("console.log(im.OverridenStaticMember(1, 2));");
      Assert.AreEqual("Member 1 [12]", console.GetLastMessage());
      jish.ExecuteCommand("console.log(im.OverridenStaticMember(1, 2, 3));");
      Assert.AreEqual("Member 2 [123]", console.GetLastMessage());
    }        

    [Test] public void TestCallingWithParamsArgs()
    {
      jish.ExecuteCommand("var im = jish.create('js.net.tests.jish.Command.InstanceMock, js.net.tests');");
      Assert.AreEqual("Created instance of js.net.tests.jish.Command.InstanceMock, js.net.tests.", console.GetLastMessage());
      jish.ExecuteCommand("console.log(im.ObjectArgs(['test', true]));");
    }

    [Test] public void TestOverloadedNonStaticMethods()
    {
      jish.ExecuteCommand("var ns = jish.create('js.net.tests.jish.Command.InstanceMock, js.net.tests')");
      jish.ExecuteCommand("console.log(ns.OverridenNSMember(1, 2));");
      Assert.AreEqual("Non Satatic Member 1 [12]", console.GetLastMessage());
      jish.ExecuteCommand("console.log(ns.OverridenNSMember(1, 2, 3));");
      Assert.AreEqual("Non Satatic Member 2 [123]", console.GetLastMessage());
    }

    [Test] public void TestOverloadedStaticMethodsWithInstance()
    {
      jish.ExecuteCommand("var ns = jish.create('js.net.tests.jish.Command.InstanceMock, js.net.tests')");
      jish.ExecuteCommand("console.log(ns.OverridenStaticMember(1, 2));");
      Assert.AreEqual("Member 1 [12]", console.GetLastMessage());
      jish.ExecuteCommand("console.log(ns.OverridenStaticMember(1, 2, 3));");
      Assert.AreEqual("Member 2 [123]", console.GetLastMessage());
    }

    [Test] public void TestStaticClassOverloadedStaticMethodsWithInstance()
    {
      jish.ExecuteCommand("var ns = jish.create('js.net.tests.jish.Command.StaticMock, js.net.tests');");
      jish.ExecuteCommand("console.log(ns.OverridenStaticMember(1, 2));");
      Assert.AreEqual("Member 1 [12]", console.GetLastMessage());
      jish.ExecuteCommand("console.log(ns.OverridenStaticMember(1, 2, 3));");
      Assert.AreEqual("Member 2 [123]", console.GetLastMessage());
    }

    [Test] public void TestCreateWithConstructorArgs()
    {
      jish.ExecuteCommand("var nsc = jish.create('js.net.tests.jish.Command.InstanceMock, js.net.tests', 'arg1', 'arg2')");
      jish.ExecuteCommand("console.log(nsc.GetConstructorArgs());");
      Assert.AreEqual("arg1arg2", console.GetLastMessage());
    }

    [Test, Ignore("Generic Methods not supported")] public void TestNonStaticOverloadsWithGenericArgs()
    {
      jish.ExecuteCommand("var ns = jish.create('js.net.tests.jish.Command.InstanceMock, js.net.tests');");
      jish.ExecuteCommand("console.log(ns.OverridenNSMemberWithGenericArgs(1, 2));");
      Assert.AreEqual("Non Satatic Member 1 [12]", console.GetLastMessage());
      jish.ExecuteCommand("console.log(ns.OverridenNSMemberWithGenericArgs(1, 2, 3));");
      Assert.AreEqual("Non Satatic Member 2 [123]", console.GetLastMessage());
    }

    [Test, Ignore("Generic Methods not supported")] public void TestStaticOverloadsWithGenericArgs()
    {
      jish.ExecuteCommand("var ns = jish.create('js.net.tests.jish.Command.StaticMock, js.net.tests');");
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
    private readonly string a1;
    private readonly string a2;

    public InstanceMock() {}

    public InstanceMock(string a1, string a2)
    {
      this.a1 = a1;
      this.a2 = a2;
    }

    public string GetConstructorArgs()
    {
      return a1 + a2;
    }

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
