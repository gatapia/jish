using NUnit.Framework;

namespace js.net.tests.jish
{
  [TestFixture] public class JishJsTests : AbstractJishTest
  {
    [Test] public void TestCreateFunctionProxy()
    {
      jish.ExecuteCommand("var proxy = global.jish.internal.createFunctionProxy(function(arg1) {});");      
    }
  }
}