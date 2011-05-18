using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace js.net
{
  public class JSConsole
  {
    public void print(string error) { log(error); }
    public void error(string error) { log(error); }
    public void debug(string message) { log(message); }
    public void info(string message) { log(message); }

    public virtual void log(object message)
    {
      Trace.Assert(message != null);

      if (message is IDictionary<string, object>)
      {
        PrintObjectMessage((IDictionary<string, object>) message);
      } else
      {
        Console.WriteLine(message);
      }
    }

    private void PrintObjectMessage(IDictionary<string, object> objectMessage)
    {
      foreach (var prop in objectMessage)
      {
        Console.WriteLine(prop.Key + " - " + prop.Value);
      }
    }
  }
}