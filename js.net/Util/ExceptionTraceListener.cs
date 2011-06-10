using System;
using System.Diagnostics;

namespace js.net.Util
{
  public class ExceptionTraceListener : TraceListener {
    public override void Write(string message)
    {
      throw new ApplicationException(message);
    }

    public override void WriteLine(string message)
    {
      throw new ApplicationException(message);
    }
  }
}