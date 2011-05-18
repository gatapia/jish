using js.net.Engine;
using NUnit.Framework;

namespace js.net.tests.Engine
{
  [TestFixture] public class JSNetEngineTests : AbstractEngineTests
  {
    protected override IEngine CreateEngine()
    {
      return new JSNetEngine();
    }
  }
}
