using NUnit.Framework;

namespace js.net.tests.jish
{
  [TestFixture] public class MultiLineTests : AbstractJishTest
  {
    
    [Test] public void TestProbelmaticCommand()
    {
      cli.ExecuteCommand("{a : 1}");      
    }

    [Test] public void TestSingleLineMultiExpressions()
    {
      cli.ExecuteCommand("for (var i = 0; i < 10; i++) console.log(i);");
      Assert.AreEqual("9", console.GetLastMessage());
    }

    [Test] public void TestMultiLineExpression()
    {
      cli.ExecuteCommand("for (var i = 0; i < 10; i++)");
      cli.ExecuteCommand(" console.log(i);");
      Assert.AreEqual("9", console.GetLastMessage());
    }

    [Test] public void TestBreakMultiLineExpression()
    {
      cli.ExecuteCommand("for (var i = 0; i < 10; i++)");
      cli.ExecuteCommand(".break");
      cli.ExecuteCommand("console.log(i);");
      Assert.AreEqual("i is not defined", console.GetLastMessage());
    }
  }
}
