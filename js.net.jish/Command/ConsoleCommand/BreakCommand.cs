using System.Collections.Generic;
using System.Diagnostics;

namespace js.net.jish.Command.ConsoleCommand
{
  public class BreakCommand : EmptyConsoleCommand
  {
    private readonly IJishInterpreter jish;

    public BreakCommand(IJishInterpreter jish)
    {
      Trace.Assert(jish != null);

      this.jish = jish;
    }

    public override string GetName()
    {
      return "break";
    }

    public override string GetHelpDescription()
    {
      return "Cancels the execution of a multi-line command.";
    }

    public override IEnumerable<CommandParam> GetParameters()
    {
      return new CommandParam[] {};
    }

    public override void Execute(params string[] args)
    {
      jish.ClearBufferedCommand();
    }
  }
}
