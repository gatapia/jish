using System;
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

    public override string ValidateArgumentsBeforeExecute(params string[] args)
    {
      return AssertExpectedArguments(null, args);
    }

    public override void Execute(params string[] args)
    {
      console.log("test command executed");
    }
  }
}