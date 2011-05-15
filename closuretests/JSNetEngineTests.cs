using js.Engine;
using NUnit.Framework;

namespace closuretests
{
  [TestFixture] public class JSNetEngineTests : AbstractEngineTests
  {
    protected override IEngine CreateEngine()
    {
      return new JSNetEngineAdapter();
    }
  }
}
