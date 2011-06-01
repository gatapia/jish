using System.Collections.Generic;
using js.net.jish;
using js.net.jish.Command;

namespace js.net.test.module
{
  public class TestCommand : EmptyCommand
  {
    private readonly JSConsole console;

    public TestCommand(JSConsole console)
    {
      this.console = console;
    }

    public override string GetName()
    {
      return "testcommand";
    }

    public override string GetHelpDescription()
    {
      return "test command help";
    }

    public override IEnumerable<CommandParam> GetParameters()
    {
      return new CommandParam[] { };
    }

    public override void Execute(params string[] args)
    {
      console.log("test command executed");
    }
  }
}