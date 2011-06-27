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

    [Test] public void TestAssemblyLoadingUsingAssemblyLoadCommandLoadsNewILCode()
    {
      jish.ExecuteCommand("var x = jish.assembly('../../../js.net.test.module/bin/js.net.test.module.dll');");
      Assert.AreEqual("Assembly 'js.net.test.module' loaded.", console.GetLastMessage());

      jish.ExecuteCommand("console.log(complex_inline_command.paramsTest());");
      Assert.AreEqual("intparams: undefined", console.GetLastMessage());

      jish.ExecuteCommand("console.log(complex_inline_command.paramsTest(1, 2, 3));");
      Assert.AreEqual("intparams: undefined", console.GetLastMessage());

      jish.ExecuteCommand("console.log(complex_inline_command.defaultValueTest());");
      Assert.AreEqual("defParam: 10", console.GetLastMessage());

      jish.ExecuteCommand("console.log(complex_inline_command.defaultValueTest(20));");
      Assert.AreEqual("defParam: 20", console.GetLastMessage());

      jish.ExecuteCommand("console.log(complex_inline_command.genericsTest('string', 10));");
      Assert.AreEqual("genericParam: 10", console.GetLastMessage());

      jish.ExecuteCommand("console.log(complex_inline_command.genericsTest('Int32', 20));");
      Assert.AreEqual("genericParam: 20", console.GetLastMessage());
    }
  }
}