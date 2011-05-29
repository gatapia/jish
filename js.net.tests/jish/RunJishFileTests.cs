using System.Linq;
using NUnit.Framework;

namespace js.net.tests.jish
{
  [TestFixture] public class RunJishFileTests : AbstractJishTest
  {
    [Test] public void RunAndAssertNuGetBuildJS()
    {
      cli.RunFile(@"..\..\..\nugetbuild.js");      
      Assert.AreEqual(1, console.GetAllMessages().Count(m => m.StartsWith("Not updating version numbers")));      
      Assert.AreEqual(1, console.GetAllMessages().Count(m => m.StartsWith("Not \"pushing\". To push please execute with \"push\" argmuent")));
      Assert.AreEqual(2, console.GetAllMessages().Count(m => m.StartsWith("Attempting to build package from")));
      Assert.AreEqual(2, console.GetAllMessages().Count(m => m.IndexOf("Successfully created package") > 0));      
    }
  }
}
