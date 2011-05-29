using js.net.jish.Command;

namespace js.net.test.module
{
  public class TestCommand : EmptyCommand
  {
    public override string GetName()
    {
      return "testcommand";
    }

    public override string GetHelpDescription()
    {
      return "test command help";
    }

    public override void Execute(string input)
    {
      JishEngine.JavaScriptConsole.log("test command executed");
    }
  }
}