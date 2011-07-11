using System.Collections.Generic;
using System.Linq;

namespace js.net.TestAdapters
{
  /// <summary>
  /// This file gives NUnit runners a chance to nicely display JavaScript
  /// unit test results.
  /// To use this file do the following:
  /// <code>
  /// // Must inherit: 'JSNetNUnitFixture' 
  /// [TestFixture] public class JavaScriptTests : JSNetNUnitFixture {  
  /// 
  ///   // If running tests in the constructor is not an option, you can also 
  ///   // call base.SetResults at a later stage.
  ///   public JSNetNunitFixtureTests() : 
  ///     base(JSNet.QUnitTestSuiteRunner(@"..\lib\qunit.js").
  ///       TestFiles(new[] { @"..\src\tests.js" })) {}    
  /// 
  ///   // Implement a test that has the ValueSource("GetTestNames") attribute
  ///   [Test] public void JavaScriptTest(
  ///       [ValueSource("GetTestNames")] string testName) {  
  ///     Assert.IsTrue(Passed(testName));
  ///   }
  /// </code>
  /// </summary>
  public class JSNetNUnitFixture {
    
    private TestSuiteResults results;

    public JSNetNUnitFixture(TestSuiteResults results = null) { 
      SetJavaScriptTestResults(results);
    }

    public void SetJavaScriptTestResults(TestSuiteResults results) {
      this.results = results;
    }

    protected IEnumerable<string> GetTestNames() {
      if (results == null) { throw new InvalidOperationException("GetTestNames called before SetResults"); }
      return results.Failed.Concat(results.Passed);
    }

    protected bool Passed(string testName) { return results.Passed.Contains(testName); }
  }
}