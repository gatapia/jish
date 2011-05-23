using System.IO;
using js.net.Engine;
using js.net.FrameworkAdapters;
using js.net.jish;
using NUnit.Framework;

namespace js.net.tests.jish
{
  [TestFixture] public class GlobalTests
  {
    const string command = "console.log('success');";
    private const string TEST_FILE = "test.js";

    private IEngine engine;
    private ICommandLineInterpreter cli;
    private TestingConsole console;
    
    [TestFixtureSetUp] public void TestFixtureSetUp()
    {
      engine = new JSNetEngine();
      console = new TestingConsole(engine);      
      cli = new CommandLineInterpreter(engine, console);
      new JSGlobal(engine, new CWDFileLoader(engine)).BindToGlobalScope();              
    }

    [TestFixtureTearDown] public void TestFixtureTearDown()
    {
      engine.Dispose();
    }

    [SetUp] public void SetUp()
    {      
      File.WriteAllText(TEST_FILE, command);
    }
    [TearDown] public void TearDown()
    {
      if (File.Exists(TEST_FILE)) File.Delete(TEST_FILE);
    }

    [Test] public void TestRequire()
    {      
      cli.ExecuteCommand("require('" + TEST_FILE + "')");
      Assert.AreEqual("success", console.GetLastMessage());
    }

    [Test] public void TestReplRunFile()
    {
      cli.RunFile(TEST_FILE);
      Assert.AreEqual("success", console.GetLastMessage());
    }

  }  
}
