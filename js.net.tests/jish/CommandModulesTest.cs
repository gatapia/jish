using System.IO;
using NUnit.Framework;

namespace js.net.tests.jish
{
  [TestFixture] public class CommandModulesTest : AbstractJishTest
  {
    private const string sourceCommandDll = @"..\..\..\js.net.test.module\bin\Debug\js.net.test.module.dll";
    private const string targetCommandDll = @"modules\js.net.test.module.dll";

    public override void SetUp()
    {
      if (!Directory.Exists("modules")) { Directory.CreateDirectory("modules"); }
      File.Copy(sourceCommandDll, targetCommandDll);
      base.SetUp();
    }

    public override void TearDown()
    {
      if (File.Exists(targetCommandDll)) File.Delete(targetCommandDll);
      base.TearDown();
    }

    [Test] public void TestModuleLoaded()
    {
      cli.ExecuteCommand(".testcommand");
      Assert.AreEqual("test command executed", console.GetLastMessage());
    }
  }
}
