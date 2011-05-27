using js.net.Engine;
using js.net.jish;
using NUnit.Framework;

namespace js.net.tests.jish
{
  [TestFixture] public class MultiLineTests
  {
    private IEngine engine;
    private ICommandLineInterpreter cli;
    private TestingConsole console;
    
    [SetUp] public void SetUp()
    {
      engine = new JSNetEngine();
      console = new TestingConsole();      
      engine.SetGlobal("console", console);
      cli = new CommandLineInterpreter(engine, console);
    }

    [TearDown] public void TearDown()
    {
      engine.Dispose();
    }

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
