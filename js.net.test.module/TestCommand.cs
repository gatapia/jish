using js.net.jish;
using js.net.jish.Command;

namespace js.net.test.module
{
  public class TestCommand : ICommand
  {
    private readonly ICommandLineInterpreter cli;
    private readonly JSConsole console;

    public TestCommand(ICommandLineInterpreter cli, JSConsole console)
    {
      this.cli = cli;
      this.console = console;
    }

    public string GetName()
    {
      return "testcommand";
    }

    public string GetHelpDescription()
    {
      return "test command help";
    }

    public void Execute(string input)
    {
      console.log("test command executed");
    }
  }
}