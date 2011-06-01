using System.Collections.Generic;

namespace js.net.jish.Command
{
  public class BreakCommand : EmptyCommand
  {
    private readonly IJishInterpreter jish;

    public BreakCommand(IJishInterpreter jish)
    {
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

    public override IEnumerable<CommandParm> GetParameters()
    {
      return new CommandParm[] {};
    }

    public override void Execute(params string[] args)
    {
      jish.ClearBufferedCommand();
    }
  }
}
