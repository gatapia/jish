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

    public override string ValidateArgumentsBeforeExecute(params string[] args)
    {
      return AssertExpectedArguments(null);
    }

    public override void Execute(params string[] args)
    {
      foreach (ICommand command in JishEngine.GetCommands())
      {
        console.log("." + command.GetName() + " - " + command.GetHelpDescription());
      }
    }
  }
}
