using System.IO;
using NUnit.Framework;

namespace js.net.tests.jish
{
  [TestFixture] public class LoadAndUsingTests : AbstractJishTest
  {    
    [Test] public void TestUsingRecognisedRegardlessOfParenthesisOrQuotes() 
    {             
      cli.ExecuteCommand(".using(System.IO.File)");
      Assert.IsTrue(console.GetLastMessage().StartsWith("System.IO.File imported"));
      console.Reset();

      cli.ExecuteCommand(".using('System.IO.File')");
      Assert.IsTrue(console.GetLastMessage().StartsWith("System.IO.File imported"));
      console.Reset();

      cli.ExecuteCommand(".using (\"System.IO.File\")");
      Assert.IsTrue(console.GetLastMessage().StartsWith("System.IO.File imported"));
      console.Reset();
    }

    [Test] public void TestUsingFullyQualifiedName()
    {
      cli.ExecuteCommand(".using(js.net.tests.jish.LoadAndUsingTests)");
      Assert.AreEqual("Could not find type: js.net.tests.jish.LoadAndUsingTests", console.GetLastMessage());      
      cli.ExecuteCommand(".using(js.net.tests.jish.LoadAndUsingTests, js.net.tests)");
      Assert.IsTrue(console.GetLastMessage().StartsWith("js.net.tests.jish.LoadAndUsingTests imported"));
    }

    
    [Test] public void TestAssemblyLoad()
    {
      cli.ExecuteCommand(".using(PicNet2.CryptoUtils, PicNet2)");
      Assert.AreEqual("Could not find type: PicNet2.CryptoUtils, PicNet2", console.GetLastMessage());
      cli.ExecuteCommand(@".load(..\..\..\lib\PicNet2.dll)");
      Assert.AreEqual("Assembly 'PicNet2' loaded.", console.GetLastMessage());
      cli.ExecuteCommand(".using(PicNet2.CryptoUtils, PicNet2)");
      Assert.IsTrue(console.GetLastMessage().StartsWith("PicNet2.CryptoUtils imported"));
    }

    [Test] public void TestUsingStaticsWithGenericsWhichAreIgnored()
    {
      TestAssemblyLoad();

      cli.ExecuteCommand(".using(PicNet2.CollectionUtils, PicNet2)");
      Assert.IsTrue(console.GetLastMessage().StartsWith("PicNet2.CollectionUtils imported"));
    }    
  }
}
