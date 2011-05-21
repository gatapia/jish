using System;
using System.Linq;
using js.net.TestAdapters;
using NUnit.Framework;

namespace js.net.tests.TestAdapters
{
  [TestFixture] public class JasmineTests
  {
    private const string jasmineJsFile = @"C:\dev\libs\jasmine\lib\jasmine-1.0.2\jasmine.js";

    [Test] public void RunFailingTests()
    {      
      using (ITestAdapter adapter = JSNet.Jasmine(jasmineJsFile))
      {
        adapter.LoadSourceFile(@"C:\dev\libs\jasmine\src\Player.js");
        ITestResults results = adapter.RunTest(@"C:\dev\libs\jasmine\spec\PlayerSpec.js"); 

        Assert.AreEqual(5, results.Failed.Count());
        Assert.AreEqual(0, results.Passed.Count());

        Assert.IsNotNull(results);
        Console.WriteLine(results);
      }            
    }
  }
}
