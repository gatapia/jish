using System;

namespace js.net.jish.Command
{
  public class HelpCommand : ICommand
  {
    private readonly ICommandLineInterpreter cli;
    private readonly JSConsole console;

    public HelpCommand(ICommandLineInterpreter cli, JSConsole console)
    {
      this.cli = cli;
      this.console = console;
    }

    public string GetName()
    {
      return "help";
    }

    public string GetHelpDescription()
    {
      return "Displays this screen.";
    }

    public void Execute(string input)
    {
      foreach (ICommand command in cli.GetCommands())
      {
        console.log("." + command.GetName() + " - " + command.GetHelpDescription());
      }
    }
  }
}
