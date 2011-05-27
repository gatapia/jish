using System;
using System.Diagnostics;
using Noesis.Javascript;

namespace js.net.Engine
{
  public class JSNetEngine : AbstractEngine
  {        
    private readonly JavascriptContext ctx;

    public JSNetEngine()
    {      
      ctx = new JavascriptContext();
    }

    public override object Run(string script, string fileName)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(script));
      Trace.Assert(ctx != null);

      return ctx.Run(script, fileName);
    }

    public override void SetGlobal(string name, object value)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(name));
      Trace.Assert(ctx != null);

      ctx.SetParameter(name, value);
    }

    public override object GetGlobal(string name)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(name));
      Trace.Assert(ctx != null);

      return ctx.GetParameter(name);
    }

    public override void Dispose()
    {
      ctx.Dispose();
    }
  }
}
