using NUnit.Framework;

namespace js.net.tests.jish
{
  [TestFixture] public class MultiLineTests : AbstractJishTest
  {
    
    [Test] public void TestProbelmaticCommand()
    {
      jish.ExecuteCommand("{a : 1}");      
    }

    [Test] public void TestSingleLineMultiExpressions()
    {
      jish.ExecuteCommand("for (var i = 0; i < 10; i++) console.log(i);");
      Assert.AreEqual("9", console.GetLastMessage());
    }

    [Test] public void TestMultiLineExpression()
    {
      jish.ExecuteCommand("for (var i = 0; i < 10; i++)");
      jish.ExecuteCommand(" console.log(i);");
      Assert.AreEqual("9", console.GetLastMessage());
    }

    [Test] public void TestBreakMultiLineExpression()
    {
      jish.ExecuteCommand("for (var i = 0; i < 10; i++)");
      jish.ExecuteCommand(".break");
      jish.ExecuteCommand("console.log(i);");
      Assert.AreEqual("i is not defined", console.GetLastMessage());
    }
  }
}
