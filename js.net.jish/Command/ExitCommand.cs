using System;
using System.Collections.Generic;

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

    public override IEnumerable<CommandParm> GetParameters()
    {
      return new CommandParm[] {};
    }

    public override void Execute(params string[] args)
    {
      Environment.Exit(0);
    }
  }
}
