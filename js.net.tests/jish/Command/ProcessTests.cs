using System;
using NUnit.Framework;

namespace js.net.tests.jish.Command
{
  [TestFixture] public class ProcessTests : AbstractJishTest
  {
    [Test] public void TestProcessNotFound()
    {
      try
      {
        jish.ExecuteCommand("jish.process('unknowncommand.exe')");
        Assert.Fail("Should throw a 'The system cannot find the file specified' exception");
      } catch (Exception e)
      {
        Assert.AreEqual("The system cannot find the file specified", e.InnerException.Message);        
      }
    }

    [Test] public void TestProcessWithNoArg()
    {
      jish.ExecuteCommand("jish.process('net.exe')");
      Assert.AreEqual("1", console.GetLastMessage());
    }

    [Test] public void TestProcessWithOneArg()
    {
      jish.ExecuteCommand("jish.process('net.exe', 'TIME')");
      Assert.AreEqual("0", console.GetLastMessage());
    }

    [Test] public void TestProcessWithTwoArgs()
    {
      jish.ExecuteCommand(@"jish.process('net.exe', 'STATISTICS SERVER')");
      Assert.AreEqual("0", console.GetLastMessage());
    }
  }
}