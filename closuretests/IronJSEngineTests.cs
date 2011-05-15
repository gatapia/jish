using js.Engine;
using NUnit.Framework;

namespace closuretests
{
  [TestFixture] public class IronJSEngineTests : AbstractEngineTests
  {
    protected override IEngine CreateEngine()
    {
      return new IronJSEngineAdapter();
    }
  }
}
