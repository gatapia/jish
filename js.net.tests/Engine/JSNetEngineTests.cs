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
   
    [Test, Ignore("Known Memory Leak in JavaScript.Net")] public override void MemoryUsageTests() { base.MemoryUsageTests(); }
    [Test, Ignore("Not supported in JavaScript.Net")] public override void TestCallbackIntoCSharp() { base.TestCallbackIntoCSharp(); }    
    [Test, Ignore("Not supported in JavaScript.Net")] public override void TestSetGlobalEmbedded() { base.TestSetGlobalEmbedded(); }
    [Test, Ignore("Not supported in JavaScript.Net")] public override void TestDynamics() { base.TestDynamics(); }
    [Test, Ignore("Not supported in JavaScript.Net")] public override void TestCallingParams() { base.TestCallingParams(); }
    [Test, Ignore("Not supported in JavaScript.Net")] public override void TestCallingOptionals() { base.TestCallingOptionals(); }
  }
}
