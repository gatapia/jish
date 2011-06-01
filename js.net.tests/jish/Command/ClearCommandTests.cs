using NUnit.Framework;

namespace js.net.tests.jish.Command
{
  [TestFixture] public class ClearCommandTests : AbstractJishTest
  {
    [Test] public void TestClear()
    {
      jish.ExecuteCommand(".clear");
      Assert.AreEqual("Clearing context...", console.GetLastMessage());
    }
  }
}