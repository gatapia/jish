using System;

namespace js.Engine
{
  public interface IEngine : IDisposable
  {
    object Run(string script);
    void SetGlobal(string name, object value);
    object GetGlobal(string name);
  }
}
