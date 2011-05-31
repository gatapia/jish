using System;

namespace js.net.jish.Command
{
  public class HelpCommand : EmptyCommand
  {
    private readonly JSConsole console;

    public HelpCommand(JSConsole console)
    {
      this.console = console;
    }

    public override string GetName()
    {
      return "help";
    }

    public override string GetHelpDescription()
    {
      return "Displays this screen.";
    }

    public override void Execute(string input)
    {
      foreach (ICommand command in JishEngine.GetCommands())
      {
        console.log("." + command.GetName() + " - " + command.GetHelpDescription());
      }
    }
  }
}
