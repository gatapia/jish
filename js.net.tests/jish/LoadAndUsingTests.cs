using js.net.Engine;
using js.net.jish;
using NUnit.Framework;

namespace js.net.tests.jish
{
  [TestFixture] public class LoadAndUsingTests
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

    [Test] public void TestUsingRecognisedRegardlessOfParenthesisOrQuotes(
      [Values(".using(System.IO.File)", ".using('System.IO.File')", ".using(\"System.IO.File\")", ".using ('System.IO.File\")")] string input)
    {       
      // Values attribute not supported in the current resharper.
      cli.ExecuteCommand(input);
      Assert.IsTrue(console.GetLastMessage().StartsWith("System.IO.File imported"));
    }

    [Test] public void TestUsingFullyQualifiedName()
    {
      cli.ExecuteCommand(".using(js.net.tests.jish.LoadAndUsingTests)");
      Assert.AreEqual("Could not find type: js.net.tests.jish.LoadAndUsingTests", console.GetLastMessage());      
      cli.ExecuteCommand(".using(js.net.tests.jish.LoadAndUsingTests, js.net.tests)");
      Assert.IsTrue(console.GetLastMessage().StartsWith("js.net.tests.jish.LoadAndUsingTests imported"));
    }

    
    [Test] public void TestAssemblyLoad()
    {
      cli.ExecuteCommand(".using(PicNet2.CryptoUtils, PicNet2)");
      Assert.AreEqual("Could not find type: PicNet2.CryptoUtils, PicNet2", console.GetLastMessage());
      cli.ExecuteCommand(@".load(..\..\..\lib\PicNet2.dll)");
      Assert.AreEqual("Assembly 'PicNet2' loaded.", console.GetLastMessage());
      cli.ExecuteCommand(".using(PicNet2.CryptoUtils, PicNet2)");
      Assert.IsTrue(console.GetLastMessage().StartsWith("PicNet2.CryptoUtils imported"));
    }

    [Test] public void TestUsingStaticsWithGenericsWhichAreIgnored()
    {
      TestAssemblyLoad();

      cli.ExecuteCommand(".using(PicNet2.CollectionUtils, PicNet2)");
      Assert.IsTrue(console.GetLastMessage().StartsWith("PicNet2.CollectionUtils imported"));
    }
  }
}
