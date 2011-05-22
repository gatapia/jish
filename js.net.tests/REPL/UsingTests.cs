using System;
using js.net.Engine;
using js.net.repl;
using NUnit.Framework;

namespace js.net.tests.REPL
{
  [TestFixture] public class UsingTests
  {
    private IEngine engine;
    private ICommandLineInterpreter cli;
    private TestingConsole console;
    // private repl.REPL repl;
    
    [TestFixtureSetUp] public void TestFixtureSetUp()
    {
      engine = new JSNetEngine();
      console = new TestingConsole();
      cli = new CommandLineInterpreter(engine, console);
      //repl = new repl.REPL(cli);
    }

    [TestFixtureTearDown] public void TestFixtureTearDown()
    {
      engine.Dispose();
    }

    [Test] public void TestUsingRecognisedRegardlessOfParenthesisOrQuotes(
      [Values(".using(System.IO.File)", ".using('System.IO.File')", ".using(\"System.IO.File\")", ".using ('System.IO.File\")")] string input)
    {       
      // Values attribute not supported in the current resharper.
      cli.ExecuteCommand(input);
      Assert.IsTrue(console.GetLastMessage().StartsWith("System.IO.File imported"));
    }
  }

  public class TestingConsole : JSConsole
  {
    private string lastMessage;

    public override string log(object message, bool newline = true)
    {
      return lastMessage = base.log(message, newline);
    }

    public string GetLastMessage()
    {
      return lastMessage;
    }

    public void Reset()
    {
      lastMessage = null;
    }
  }
}
