using System;
using System.IO;
using js.Engine;
using js.NodeFacade;
using NUnit.Framework;

namespace nodetests
{
  [TestFixture] public class NodeTestSuite
  {
    private const string NODE_TEST_DIR = @"C:\dev\projects\play\picnetjs\lib\Nodelib\test\";

    private JSNetEngineAdapter engine;
    private NodeContext ctx;

    [SetUp] public void SetUp()
    {
      ctx = new NodeContext(engine = new JSNetEngineAdapter(), ".");
      ctx.Initialise();
    }

    [TearDown] public void TearDown()
    {
      engine.Dispose();
    }

    [Test] public void RunAllTests()
    {
      string[] tests = Directory.GetFiles(NODE_TEST_DIR, "test-*.js", SearchOption.AllDirectories);
      foreach (string test in tests)
      {
        Console.WriteLine("Running Test: " + test.Replace(NODE_TEST_DIR, ""));
        engine.Run(File.ReadAllText(test));
      }
    }
  }
}
