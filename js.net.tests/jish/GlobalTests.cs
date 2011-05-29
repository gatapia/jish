using System.IO;
using NUnit.Framework;

namespace js.net.tests.jish
{
  [TestFixture] public class GlobalTests : AbstractJishTest
  {
    const string command = "console.log('success');";

    private const string TEST_FILE = "test.js";

    
    [SetUp] public override void SetUp()
    {      
      base.SetUp();
      File.WriteAllText(TEST_FILE, command);
    }

    [TearDown] public override void TearDown()
    {
      base.TearDown();
      if (File.Exists(TEST_FILE)) File.Delete(TEST_FILE);
    }

    [Test] public void TestReplRunFile()
    {
      jish.RunFile(TEST_FILE);
      Assert.AreEqual("success", console.GetLastMessage());
    }

  }  
}
