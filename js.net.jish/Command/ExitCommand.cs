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

    public override string ValidateArgumentsBeforeExecute(params string[] args)
    {
      return AssertExpectedArguments(null, args);
    }

    public override void Execute(params string[] args)
    {
      Environment.Exit(0);
    }
  }
}
