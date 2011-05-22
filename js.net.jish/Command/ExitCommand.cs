using System;

namespace js.net.jish.Command
{
  public class ExitCommand : ICommand
  {
    private readonly ICommandLineInterpreter cli;
    private readonly JSConsole console;

    public ExitCommand(ICommandLineInterpreter cli, JSConsole console)
    {
      this.cli = cli;
      this.console = console;
    }

    public string GetName()
    {
      return "exit";
    }

    public string GetHelpDescription()
    {
      return "Exit Jish.";
    }

    public void Execute(string input)
    {
      Environment.Exit(0);
    }
  }
}
