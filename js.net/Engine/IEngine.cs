using System;

namespace js.net.Engine
{
  public interface IEngine : IDisposable
  {    
    object Run(string script, string fileName);
    void SetGlobal(string name, object value);
    object GetGlobal(string name);
  }
}
