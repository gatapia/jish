using System;
using Noesis.Javascript;

namespace js.Engine
{
  public class JSNetEngineAdapter : IEngine
  {
    private readonly JavascriptContext ctx;

    public JSNetEngineAdapter()
    {
      ctx = new JavascriptContext();
    }

    public object Run(string script)
    {
      return ctx.Run(script);
    }

    public void SetGlobal(string name, object value)
    {
      ctx.SetParameter(name, value);
    }

    public object GetGlobal(string name)
    {
      return ctx.GetParameter(name);
    }

    public void Dispose()
    {
      ctx.Dispose();
    }
  }
}
