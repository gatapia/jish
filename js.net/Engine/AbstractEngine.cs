namespace js.net.Engine
{
  public abstract class AbstractEngine : IEngine
  {
    public abstract object Run(string script);
    public abstract void SetGlobal(string name, object value);
    public abstract object GetGlobal(string name);
    public abstract void Dispose();
  }
}