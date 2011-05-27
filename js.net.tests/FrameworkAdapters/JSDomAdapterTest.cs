using js.net.Engine;
using js.net.FrameworkAdapters;
using NUnit.Framework;

namespace js.net.tests.FrameworkAdapters
{
  [TestFixture] public class JSDomAdapterTest
  {
    [Test] public void TestLoadJSDomAdapter()
    {
      IEngine engine = new JSNetEngine();
      JSDomAdapter adapter = new JSDomAdapter(engine);
      adapter.Initialise();
    }
  }
}