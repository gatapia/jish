using NUnit.Framework;

namespace js.net.tests.jish.Command
{
  [TestFixture] public class AssemblyCommandTests : AbstractJishTest
  {
    [Test] public void TestAssemblyCommandReturnsNameSpacesLoaded()
    {
      jish.ExecuteCommand("var x = jish.loadAssemblyImpl('js.net.tests.dll');");
      Assert.AreEqual("Assembly 'js.net.tests' loaded.", console.GetLastMessage());

      jish.ExecuteCommand("console.log(typeof(x));");
      Assert.AreEqual("object", console.GetLastMessage());
    }

    [Test] public void TestAssemblyRedirectionToLoadAssemblyImpl()
    {
      jish.ExecuteCommand("var x = jish.assembly('js.net.tests.dll');");
      Assert.AreEqual("Assembly 'js.net.tests' loaded.", console.GetLastMessage());

      jish.ExecuteCommand("console.log(typeof(x));");
      Assert.AreEqual("undefined", console.GetLastMessage());
    }
  }
}