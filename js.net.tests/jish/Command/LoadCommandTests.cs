using System;
using System.IO;
using NUnit.Framework;

namespace js.net.tests.jish.Command
{
  [TestFixture] public class LoadCommandTests : AbstractJishTest
  {
    [Test] public void TestLoadNonExistentFile()
    {      
      try
      {
        jish.ExecuteCommand("jish.load('unknownfile.exe')");
        Assert.Fail("Should throw an assertion failure");
      } catch (Exception) {}
    }

    [Test] public void TestRunSimpleScript()
    {
      const string jsFileContent = "var x = 123;";
      const string jsFile = "test.js";
      File.WriteAllText(jsFile, jsFileContent);
      try
      {
        jish.ExecuteCommand("jish.load('" + jsFile + "');console.log('result: ' + x);");
        Assert.AreEqual("result: 123", console.GetLastMessage());
      } finally
      {
        File.Delete(jsFile);
      }
    }
  }
}