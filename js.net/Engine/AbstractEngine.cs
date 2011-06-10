using System.Diagnostics;
using js.net.Util;

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
}