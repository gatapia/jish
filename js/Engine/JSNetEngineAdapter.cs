using System.Diagnostics;
using Noesis.Javascript;

namespace js.net.Engine
{
  public class JSNetEngineAdapter : AbstractEngineAdapter
  {
    static JSNetEngineAdapter()
    {
      DefaultTraceListener def = (DefaultTraceListener) Trace.Listeners[0];
      def.AssertUiEnabled = false; // No silly dialogs
      Trace.Listeners.Clear();
      Trace.Listeners.Add(def);
    }

    private readonly JavascriptContext ctx;

    public JSNetEngineAdapter()
    {
      ctx = new JavascriptContext();
    }

    public override object Run(string script)
    {
      return ctx.Run(script);
    }

    public override void SetGlobal(string name, object value)
    {
      ctx.SetParameter(name, value);
    }

    public override object GetGlobal(string name)
    {
      return ctx.GetParameter(name);
    }

    public override void Dispose()
    {
      ctx.Dispose();
    }
  }
}
