using NUnit.Framework;
using js.net.TestAdapters;

namespace js.net.tests.TestAdapters
{
  [TestFixture] public class JSNetNunitFixtureTests : JSNetNUnitFixture {
    private const string qUnitJS = @"C:\dev\libs\qunit\qunit\qunit.js";

    public JSNetNunitFixtureTests() : base(JSNet.QUnitTestSuiteRunner(qUnitJS).TestFiles(new[] { @"C:\dev\libs\qunit\test\same.js" })) {}    

    [Test] public void JavaScriptTest([ValueSource("GetTestNames")] string testName) {
      Assert.IsTrue(Passed(testName));
    }
  }
}