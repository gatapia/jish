using System;
using System.Diagnostics;

namespace js
{
  public class JSConsole
  {
    public void print(string error) { log(error); }
    public void error(string error) { log(error); }
    public void debug(string message) { log(message); }
    public void info(string message) { log(message); }

    public virtual void log(string message)
    {
      Trace.Assert(message != null);

      Console.WriteLine(message);
    }
  }
}