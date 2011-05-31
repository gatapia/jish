using js.net.jish.Command;

namespace js.net.test.module
{
  public class TestCommand : EmptyCommand
  {
    private readonly JSConsole console;

    public TestCommand(JSConsole console)
    {
      this.console = console;
    }

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
      console.log("test command executed");
    }
  }
}