
using System.Collections.Generic;
using System.Linq;

namespace js.net.tests.jish
{
  public class TestingConsole : JSConsole
  {
    private readonly IList<string> messages = new List<string>();

    public override string log(object message, bool newline)
    {
      string realMessage = base.log(message, newline);
      messages.Add(realMessage);
      return realMessage;
    }

    public string GetLastMessage()
    {
      return messages.Last();
    }

    public void Reset()
    {
      messages.Clear();
    }
  }
}