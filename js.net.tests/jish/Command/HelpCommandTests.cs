using NUnit.Framework;

namespace js.net.tests.jish.Command
{
  [TestFixture] public class HelpCommandTests : AbstractJishTest
  {
    [Test] public void InlineCommandHelpIntegration()
    {
       jish.ExecuteCommand(".help");
      Assert.IsTrue(console.GetLastMessage().IndexOf("jish.assembly:") >= 0, console.GetLastMessage());
    }    

    [Test] public void InlinSpecialHelpIntegration()
    {
       jish.ExecuteCommand(".help");
      Assert.IsTrue(console.GetLastMessage().IndexOf(".exit:") >= 0, console.GetLastMessage());
    }
  }
}