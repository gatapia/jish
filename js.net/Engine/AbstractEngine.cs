using System;
using System.Diagnostics;

namespace js.net.Engine
{
  public abstract class AbstractEngine : IEngine
  {
    static AbstractEngine()
    {
      Trace.Listeners.Clear();
      Trace.Listeners.Add(new ExceptionTraceListener());
    }

    public abstract object Run(string script, string fileName);
    public abstract void SetGlobal(string name, object value);
    public abstract object GetGlobal(string name);
    public abstract void Dispose();
  }

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