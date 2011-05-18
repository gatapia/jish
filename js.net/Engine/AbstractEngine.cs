namespace js.net.Engine
{
  public abstract class AbstractEngine : IEngine
  {    
    public virtual void Reset()
    {
      Run("for (var __globalMember in this) { delete this[__globalMember]; }; ");
    }

    public abstract object Run(string script);
    public abstract void SetGlobal(string name, object value);
    public abstract object GetGlobal(string name);
    public abstract void Dispose();
  }
}