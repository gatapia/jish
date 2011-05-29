using System;

namespace js.net.jish.Command
{
  public class ExitCommand : EmptyCommand
  {
    public override string GetName()
    {
      return "exit";
    }

    public override string GetHelpDescription()
    {
      return "Exit Jish.";
    }

    public override void Execute(string input)
    {
      Environment.Exit(0);
    }
  }
}
