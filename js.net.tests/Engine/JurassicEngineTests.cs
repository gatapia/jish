using js.net.Engine;
using NUnit.Framework;

namespace js.net.tests.Engine
{
  [TestFixture] public class JurassicEngineTests : AbstractEngineTests
  {
    protected override IEngine CreateEngine()
    {
      return new JurassicEngine();
    }

    [Test, Ignore("Known limitation of Jurassic")] public override void MemoryUsageTests() { base.MemoryUsageTests(); }
    [Test, Ignore("Known limitation of Jurassic")] public override void TestCallingOptionals() { base.TestCallingOptionals(); }
    [Test, Ignore("Known limitation of Jurassic")] public override void TestCallingParams() { base.TestCallingParams(); }
    [Test, Ignore("Known limitation of Jurassic")] public override void TestCallbackIntoCSharp() { base.TestCallbackIntoCSharp(); }
    [Test, Ignore("Known limitation of Jurassic")] public override void TestDynamics() { base.TestDynamics(); }
    [Test, Ignore("Known limitation of Jurassic")] public override void TestSetGlobalEmbedded() { base.TestSetGlobalEmbedded(); }
    [Test, Ignore("Known limitation of Jurassic")] public override void TestSetGlobal() { base.TestSetGlobal(); }
  }
}
