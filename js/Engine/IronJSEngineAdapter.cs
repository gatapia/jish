using IronJS.Hosting;

namespace js.Engine
{
  public class IronJSEngineAdapter : IEngine
  {
    private readonly CSharp.Context ctx;
    
    public IronJSEngineAdapter()
    {
      ctx = new CSharp.Context();
    }

    public object Run(string script)
    {
      return ctx.Execute(script);
    }

    public void SetGlobal(string name, object value)
    {
      ctx.SetGlobal(name, value);
    }

    public object GetGlobal(string name)
    {
      return ctx.GetGlobal(name);
    }
    
    public void Dispose() {}
  }
}
