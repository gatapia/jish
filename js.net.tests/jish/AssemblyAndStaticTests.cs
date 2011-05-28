using NUnit.Framework;

namespace js.net.tests.jish
{
  [TestFixture] public class AssemblyAndStaticTests : AbstractJishTest
  {    
    [Test] public void TestUsingRecognisedRegardlessOfParenthesisOrQuotes() 
    {             
      cli.ExecuteCommand(".static(System.IO.File)");
      Assert.IsTrue(console.GetLastMessage().StartsWith("System.IO.File imported"));
      console.Reset();

      cli.ExecuteCommand(".static('System.IO.File')");
      Assert.IsTrue(console.GetLastMessage().StartsWith("System.IO.File imported"));
      console.Reset();

      cli.ExecuteCommand(".static (\"System.IO.File\")");
      Assert.IsTrue(console.GetLastMessage().StartsWith("System.IO.File imported"));
      console.Reset();
    }

    [Test] public void TestUsingFullyQualifiedName()
    {
      cli.ExecuteCommand(".static(js.net.tests.jish.AssemblyAndStaticTests)");
      Assert.AreEqual("Could not find type: js.net.tests.jish.AssemblyAndStaticTests", console.GetLastMessage());      
      cli.ExecuteCommand(".static(js.net.tests.jish.AssemblyAndStaticTests, js.net.tests)");
      Assert.IsTrue(console.GetLastMessage().StartsWith("js.net.tests.jish.AssemblyAndStaticTests imported"));
    }

    
    [Test] public void TestAssemblyLoad()
    {
      cli.ExecuteCommand(".static(PicNet2.CryptoUtils, PicNet2)");
      Assert.AreEqual("Could not find type: PicNet2.CryptoUtils, PicNet2", console.GetLastMessage());
      cli.ExecuteCommand(@".assembly(..\..\..\lib\PicNet2.dll)");
      Assert.AreEqual("Assembly 'PicNet2' loaded.", console.GetLastMessage());
      cli.ExecuteCommand(".static(PicNet2.CryptoUtils, PicNet2)");
      Assert.IsTrue(console.GetLastMessage().StartsWith("PicNet2.CryptoUtils imported"));
    }

    [Test] public void TestUsingStaticsWithGenericsWhichAreIgnored()
    {
      TestAssemblyLoad();

      cli.ExecuteCommand(".static(PicNet2.CollectionUtils, PicNet2)");
      Assert.IsTrue(console.GetLastMessage().StartsWith("PicNet2.CollectionUtils imported"));
    }    
  }
}
