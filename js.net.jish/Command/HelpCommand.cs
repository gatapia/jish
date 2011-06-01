using System.Collections.Generic;

namespace js.net.jish.Command
{
  public class HelpCommand : EmptyCommand
  {
    private readonly IJishInterpreter jish;
    private readonly JSConsole console;

    public HelpCommand(IJishInterpreter jish, JSConsole console)
    {
      this.jish = jish;
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

    public override IEnumerable<CommandParm> GetParameters()
    {
      return new CommandParm[] {};
    }

    public override void Execute(params string[] args)
    {
      foreach (ICommand command in jish.GetAllActiveCommands())
      {
        console.log("." + command.GetName() + "(args) - " + command.GetHelpDescription());
      }
    }
  }
}
