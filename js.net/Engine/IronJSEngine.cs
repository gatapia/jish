using System;
using System.Diagnostics;
using IronJS.Hosting;

namespace js.net.Engine
{
  public class IronJSEngine : AbstractEngine, IEngine
  {
    // TODO: Find a better place for this initiaser
    static IronJSEngine()
    {
      DefaultTraceListener def = (DefaultTraceListener) Trace.Listeners[0];
      def.AssertUiEnabled = false; // No silly dialogs
      Trace.Listeners.Clear();
      Trace.Listeners.Add(def);
    }

    private readonly CSharp.Context ctx;
    
    public IronJSEngine()
    {
      ctx = new CSharp.Context();
    }

    public override object Run(string script)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(script));
      Trace.Assert(ctx != null);

      return ctx.Execute(script);
    }

    public override void SetGlobal(string name, object value)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(name));
      Trace.Assert(ctx != null);

      ctx.SetGlobal(name, value);
    }

    public override object GetGlobal(string name)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(name));
      Trace.Assert(ctx != null);

      return ctx.GetGlobal(name);
    }

    public override void Dispose() {}
  }
}
