using System.IO;
using NUnit.Framework;

namespace js.net.tests.jish
{
  [TestFixture] public class SimpleRunFileTests : AbstractJishTest
  {
    const string command = "console.log('success args[' + args + ']');";

    private const string TEST_FILE = "test.js";

    
    [SetUp] public override void SetUp()
    {      
      base.SetUp();
      File.WriteAllText(TEST_FILE, command);
    }

    [TearDown] public void TearDown()
    {
      if (File.Exists(TEST_FILE)) File.Delete(TEST_FILE);
    }

 
    [Test] public void TestRunFile()
    {
      jish.RunFile(TEST_FILE);
      Assert.AreEqual("success args[]", console.GetLastMessage());
    }

    [Test] public void TestRunFileWithArgs()
    {
      jish.RunFile(TEST_FILE, new [] {"arg1", "arg2"});      
      Assert.AreEqual("success args[arg1,arg2]", console.GetLastMessage());
      jish.ExecuteCommand("console.log('idx: ' + args.indexOf('arg2'));");
      Assert.AreEqual("idx: 1", console.GetLastMessage());
    }
  }  
}
