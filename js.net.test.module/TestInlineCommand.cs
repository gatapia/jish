using System.Collections.Generic;
using js.net.jish;
using js.net.jish.Command;
using js.net.jish.Command.InlineCommand;

namespace js.net.test.module
{
  public class TestInlineCommand : IInlineCommand
  {
    public string GetNameSpace()
    {
      return "inline_command";
    }

    public int add(int arg1, int arg2)
    {
      return arg1 + arg2;
    }    

    public string GetName()
    {
      return "add";
    }

    public string GetHelpDescription()
    {
      return "help";
    }

    public IEnumerable<CommandParam> GetParameters()
    {
      return new CommandParam[] { };
    }
  }
}
