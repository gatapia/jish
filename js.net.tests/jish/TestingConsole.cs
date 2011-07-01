using System.Collections.Generic;
using System.Linq;
using js.net.Util;

namespace js.net.tests.jish
{
  public class TestingConsole : JSConsole
  {
    private readonly IList<string> messages = new List<string>();

    protected override void logImpl(string message, bool newline)
    {
      base.logImpl(message, newline);
      messages.Add(message);
    }

    public IEnumerable<string> GetAllMessages()
    {
      return messages;
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