using System.Collections.Generic;
using System.Linq;
using js.net.jish.Command;
using js.net.jish.Command.InlineCommand;
using NUnit.Framework;

namespace js.net.tests.jish.Command
{
  [TestFixture] public class AssemblyCommandTests : AbstractJishTest
  {    
    [Test] public void TestAssemblyCommandReturnsNameSpacesLoaded()
    {
      jish.ExecuteCommand("var x = jish.loadAssemblyImpl('../../../js.net.test.module/bin/js.net.test.module.dll');");
      Assert.AreEqual("Assembly 'js.net.test.module' loaded.", console.GetLastMessage());

      jish.ExecuteCommand("console.log(typeof(x));");
      Assert.AreEqual("object", console.GetLastMessage());
    }

    [Test] public void TestAssemblyRedirectionToLoadAssemblyImpl()
    {
      jish.ExecuteCommand("var x = jish.assembly('../../../js.net.test.module/bin/js.net.test.module.dll');");
      Assert.AreEqual("Assembly 'js.net.test.module' loaded.", console.GetLastMessage());

      jish.ExecuteCommand("console.log(typeof(x));");
      Assert.AreEqual("undefined", console.GetLastMessage());
    }    

    [Test] public void TestAlreadyLoadedAssemblyCommandsAreILedProperly()
    {
      jish.ExecuteCommand("console.log(tst2.paramsAddTest());");
      Assert.AreEqual("0", console.GetLastMessage());

      jish.ExecuteCommand("console.log(tst2.paramsAddTest(1, 2, 3));");
      Assert.AreEqual("6", console.GetLastMessage());
    }

    [Test] public void TestLoadedAssemblyInlineCommandsGetILedProperly()
    {
      jish.ExecuteCommand("jish.assembly('../../../js.net.test.module/bin/js.net.test.module.dll');");
      Assert.AreEqual("Assembly 'js.net.test.module' loaded.", console.GetLastMessage());

      jish.ExecuteCommand("console.log(tst.paramsTest());");
      Assert.AreEqual("intparams: ", console.GetLastMessage());

      jish.ExecuteCommand("console.log(tst.paramsTest(1, 2, 3));");
      Assert.AreEqual("intparams: 1,2,3", console.GetLastMessage());

      jish.ExecuteCommand("console.log(tst.defaultValueTest());");
      Assert.AreEqual("defParam: 10", console.GetLastMessage());

      jish.ExecuteCommand("console.log(tst.defaultValueTest(20));");
      Assert.AreEqual("defParam: 20", console.GetLastMessage());

      jish.ExecuteCommand("console.log(tst.genericsTest(10));");
      Assert.AreEqual("genericParam: 10", console.GetLastMessage());

      jish.ExecuteCommand("console.log(tst.genericsTest('superduper'));");
      Assert.AreEqual("genericParam: superduper", console.GetLastMessage());
    }
  }

  public class TestInlineCommand : IInlineCommand
  {
    public string GetName() { return "name"; }
    public string GetHelpDescription() { return "help"; }
    public IEnumerable<CommandParam> GetParameters() { return null; }
    public string GetNameSpace() { return "tst2"; }

    public string paramsAddTest(params int[] args) { return (args == null ? 0 : args.Sum()).ToString(); }
  }
}