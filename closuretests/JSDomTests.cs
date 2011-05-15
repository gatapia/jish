using System.IO;
using js.Engine;
using js.NodeFacade;
using NUnit.Framework;

namespace closuretests
{
  [TestFixture] public class JSDomTests
  {
    private JSNetEngineAdapter ctx;
    [SetUp] public void SetUp()
    {
      ctx = new JSNetEngineAdapter();
      new NodeContext(ctx, @"C:\dev\projects\picnetjs\lib\jsdom\").Initialise();
    }

    [TearDown] public void TearDown()
    {
      ctx.Dispose();
    }

    [Test] public void TestLoadJSDom()
    {
      Assert.AreEqual(null, ctx.Run(File.ReadAllText(@"C:\dev\projects\picnetjs\lib\jsdom\jsdom.js")));
    }
  }
}
