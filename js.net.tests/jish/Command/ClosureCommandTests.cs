using System;
using NUnit.Framework;

namespace js.net.tests.jish.Command
{
  [TestFixture]
  public class ClosureCommandTests : AbstractJishTest
  {
    const string basejsfile = @"C:\\dev\\Projects\\Misc\\closure-library\\closure\\goog\\base.js";

    [Test] public void TestClosureExists()
    {
      jish.ExecuteCommand("console.log(jish.closure);");
      Assert.AreNotEqual(String.Empty, console.GetLastMessage());
    }

    [Test] public void TestLoadeClosure()
    {
      jish.ExecuteCommand("jish.closure('" + basejsfile +"'); console.log(typeof(goog.bind));");
      Assert.AreEqual("function", console.GetLastMessage());
    }

    [Test] public void TestUseRequireAndLibraryUse()
    {
        jish.ExecuteCommand(
@"
jish.closure('" + basejsfile + @"');      
goog.require('goog.array');
var arr = [0,1,2,3,4];
var reduced = goog.array.reduce(arr, function(previousValue, currentValue, index, array) {    
  return previousValue + currentValue;
}, 0);
console.log(reduced);
");
      Assert.AreEqual("10", console.GetLastMessage());
    }
  }
}