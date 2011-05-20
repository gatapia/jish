using js.net.Util;
using Noesis.Javascript;

namespace js.net.Engine
{
  public class JSNetEngine : AbstractEngine
  {    
    static JSNetEngine()
    {
      // No need to include 2 assemblies, just load the embedded resource.
      new EmbeddedAssemblyLoader("Noesis.Javascript.dll", "js.net.Noesis.Javascript.dll").
        CopyAssemblyToExecutable(); 
    }
    private readonly JavascriptContext ctx;

    public JSNetEngine()
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
