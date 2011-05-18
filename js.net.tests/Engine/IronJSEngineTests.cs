using js.net.Engine;
using NUnit.Framework;

namespace js.net.tests.Engine
{
  [TestFixture, Ignore("IronJS is not cool, too slow so don't bother digging deeper for now")] public class IronJSEngineTests : AbstractEngineTests
  {
    protected override IEngine CreateEngine()
    {
      return new IronJSEngine();
    }
  }
}
