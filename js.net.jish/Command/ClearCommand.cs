using System;
using System.Collections.Generic;

namespace js.net.jish.Command
{
  public class ClearCommand : EmptyCommand
  {
    private readonly IJishInterpreter jish;

    public ClearCommand(IJishInterpreter jish)
    {
      this.jish = jish;
    }

    public override string GetName()
    {
      return "clear";
    }

    public override string GetHelpDescription()
    {
      return "Break, and also clear the local context.";
    }

    public override IEnumerable<CommandParam> GetParameters()
    {
      return new CommandParam[] {};
    }

    public override void Execute(params string[] args)
    {
      jish.ClearBufferedCommand();      
      Console.WriteLine("Clearing context...");
      jish.ExecuteCommand(
        @"
for (var i in this) {
  if (i === 'console' || i === 'global') continue;
  delete this[i];
}
"
        );
    }
  }
}
