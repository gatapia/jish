using System.Diagnostics;
using IronJS.Hosting;

namespace js.net.Engine
{
  public class IronJSEngineAdapter : AbstractEngineAdapter, IEngine
  {
    // TODO: Find a better place for this initiaser
    static IronJSEngineAdapter()
    {
      DefaultTraceListener def = (DefaultTraceListener) Trace.Listeners[0];
      def.AssertUiEnabled = false; // No silly dialogs
      Trace.Listeners.Clear();
      Trace.Listeners.Add(def);
    }

    private readonly CSharp.Context ctx;
    
    public IronJSEngineAdapter()
    {
      ctx = new CSharp.Context();
    }

    public override object Run(string script)
    {
      return ctx.Execute(script);
    }

    public override void SetGlobal(string name, object value)
    {
      ctx.SetGlobal(name, value);
    }

    public override object GetGlobal(string name)
    {
      return ctx.GetGlobal(name);
    }

    public override void Dispose() {}
  }
}
