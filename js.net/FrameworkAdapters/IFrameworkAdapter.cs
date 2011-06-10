using System;

namespace js.net.FrameworkAdapters
{
  public interface IFrameworkAdapter : IDisposable
  {    
    object Run(string script, string fileName);
    void SetGlobal(string name, object value);
    object GetGlobal(string name);
    object LoadJSFile(string file, bool setCwd);
  } 
}
