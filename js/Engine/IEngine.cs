using System;

namespace js.net.Engine
{
  public interface IEngine : IDisposable
  {    
    object Run(string script);
    void SetGlobal(string name, object value);
    object GetGlobal(string name);

    /// <summary>
    /// Resets the engine to a 'just intiialised' state.  I.e. It deletes
    /// all registered globals.
    /// </summary>
    void Reset();
  }
}
