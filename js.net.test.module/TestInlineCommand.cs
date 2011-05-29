using js.net.jish.InlineCommand;

namespace js.net.test.module
{
  public class TestInlineCommand : IInlineCommand
  {
    public string GetNameSpace()
    {
      return "inline_command";
    }

    public int Add(int arg1, int arg2)
    {
      return arg1 + arg2;
    }
  }
}
