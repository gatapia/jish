using js.net.Engine;
using js.net.FrameworkAdapters;
using NUnit.Framework;

namespace js.net.tests.FrameworkAdapters
{
  [TestFixture] public class JSDomAdapterTest
  {
    private const string jsDomSource = @"C:\dev\libs\jsdom\lib\jsdom.js";

    [Test] public void TestLoad()
    {
      IEngine engine = new JSNetEngine();
      JSDomAdapter adapter = new JSDomAdapter(engine, jsDomSource);
      adapter.Initialise();
    }
  }
}