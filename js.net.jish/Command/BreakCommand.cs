namespace js.net.jish.Command
{
  public class BreakCommand : ICommand
  {
    private readonly ICommandLineInterpreter cli;
    private readonly JSConsole console;

    public BreakCommand(ICommandLineInterpreter cli, JSConsole console)
    {
      this.cli = cli;
      this.console = console;
    }

    public string GetName()
    {
      return "break";
    }

    public string GetHelpDescription()
    {
      return "Cancels the execution of a multi-line command.";
    }

    public void Execute(string input)
    {
      cli.ClearBufferedCommand();
    }
  }
}
