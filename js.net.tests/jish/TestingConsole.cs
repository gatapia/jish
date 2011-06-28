
using System;
using System.Collections.Generic;
using System.Linq;
using js.net.Util;

namespace js.net.tests.jish
{
  public class TestingConsole : JSConsole
  {
    private readonly IList<string> messages = new List<string>();

    public override void log(object message, bool newline)
    {
      base.log(message, newline);
      messages.Add(message == null ? String.Empty : message.ToString());
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