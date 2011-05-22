using js.net.Engine;

namespace js.net.tests.REPL
{
  public class TestingConsole : JSConsole
  {
    private string lastMessage;

    public TestingConsole(IEngine engine) : base(engine) {}

    public override string log(object message, bool newline)
    {
      return lastMessage = base.log(message, newline);
    }

    public string GetLastMessage()
    {
      return lastMessage;
    }

    public void Reset()
    {
      lastMessage = null;
    }
  }
}