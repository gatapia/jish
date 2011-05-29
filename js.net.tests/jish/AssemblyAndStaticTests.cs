using NUnit.Framework;

namespace js.net.tests.jish
{
  [TestFixture] public class AssemblyAndStaticTests : AbstractJishTest
  {    
    [Test] public void TestUsingRecognisedRegardlessOfParenthesisOrQuotes() 
    {             
      jish.ExecuteCommand(".static(System.IO.File)");
      Assert.IsTrue(console.GetLastMessage().StartsWith("System.IO.File imported"));
      console.Reset();

      jish.ExecuteCommand(".static('System.IO.File')");
      Assert.IsTrue(console.GetLastMessage().StartsWith("System.IO.File imported"));
      console.Reset();

      jish.ExecuteCommand(".static (\"System.IO.File\")");
      Assert.IsTrue(console.GetLastMessage().StartsWith("System.IO.File imported"));
      console.Reset();
    }

    [Test] public void TestUsingFullyQualifiedName()
    {
      jish.ExecuteCommand(".static(js.net.tests.jish.AssemblyAndStaticTestsMock)");
      Assert.AreEqual("Could not find type: js.net.tests.jish.AssemblyAndStaticTestsMock", console.GetLastMessage());      
      jish.ExecuteCommand(".static(js.net.tests.jish.AssemblyAndStaticTestsMock, js.net.tests)");
      Assert.IsTrue(console.GetLastMessage().StartsWith("js.net.tests.jish.AssemblyAndStaticTestsMock imported"));
    }

    
    [Test] public void TestAssemblyLoad()
    {
      jish.ExecuteCommand(".static(PicNet2.CryptoUtils, PicNet2)");
      Assert.AreEqual("Could not find type: PicNet2.CryptoUtils, PicNet2", console.GetLastMessage());
      jish.ExecuteCommand(@".assembly(..\..\..\lib\PicNet2.dll)");
      Assert.AreEqual("Assembly 'PicNet2' loaded.", console.GetLastMessage());
      jish.ExecuteCommand(".static(PicNet2.CryptoUtils, PicNet2)");
      Assert.IsTrue(console.GetLastMessage().StartsWith("PicNet2.CryptoUtils imported"));
    }

    [Test] public void TestOverloadedStaticMethods()
    {
      jish.ExecuteCommand(".static(js.net.tests.jish.AssemblyAndStaticTestsMock, js.net.tests)");
      Assert.IsTrue(console.GetLastMessage().StartsWith("js.net.tests.jish.AssemblyAndStaticTestsMock imported"));
      jish.ExecuteCommand("console.log(AssemblyAndStaticTestsMock.OverridenMember([1, 2]));");
      Assert.AreEqual("Member 1 [12]", console.GetLastMessage());
      jish.ExecuteCommand("console.log(AssemblyAndStaticTestsMock.OverridenMember([1, 2, 3]));");
      Assert.AreEqual("Member 2 [123]", console.GetLastMessage());
    }        

    [Test] public void TestCallingWithParamsArgs()
    {
      jish.ExecuteCommand(".static(js.net.tests.jish.AssemblyAndStaticTestsMock, js.net.tests)");
      Assert.IsTrue(console.GetLastMessage().StartsWith("js.net.tests.jish.AssemblyAndStaticTestsMock imported"));
      jish.ExecuteCommand("console.log(AssemblyAndStaticTestsMock.ObjectArgs(['test', true]));");
    }
  }

  public class AssemblyAndStaticTestsMock
  {
    public static string OverridenMember(int arg1, int arg2)
    {
      return "Member 1 [" + arg1 + arg2 + "]";
    }

    public static string OverridenMember(int arg1, int arg2, int arg3)
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
