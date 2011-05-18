using js.net.Engine;
using NUnit.Framework;

namespace js.net.engine.tests
{
  [TestFixture] public class IronJSEngineTests : AbstractEngineTests
  {
    protected override IEngine CreateEngine()
    {
      return new IronJSEngineAdapter();
    }
  }
}
