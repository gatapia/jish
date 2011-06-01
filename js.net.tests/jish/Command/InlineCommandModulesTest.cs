using System.IO;
using NUnit.Framework;

namespace js.net.tests.jish.Command
{
  [TestFixture] public class InlineCommandModulesTest : AbstractJishTest
  {
    private const string sourceCommandDll = @"..\..\..\js.net.test.module\bin\js.net.test.module.dll";
    private const string targetCommandDll = @"modules\js.net.test.module.dll";

    public override void SetUp()
    {
      if (!Directory.Exists("modules")) { Directory.CreateDirectory("modules"); }
      File.Copy(sourceCommandDll, targetCommandDll, true);
      base.SetUp();
    }

    public void TearDown()
    {
      if (File.Exists(targetCommandDll)) File.Delete(targetCommandDll);
    }

    [Test] public void TestExecutingTestInlineCommand()
    {
      jish.ExecuteCommand("console.log(inline_command.add(1, 2));");
      Assert.AreEqual("3", console.GetLastMessage());
    }
  }
}
