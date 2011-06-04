using System.Reflection;

namespace js.net.jish.IL
{
  public class ProxyMethod
  {    
    public ProxyMethod(MethodInfo mi, object methodContext)
    {
      RealMethod = mi;
      MethodContext = methodContext;
    }

    public object MethodContext { get; private set; }
    public MethodInfo RealMethod { get; private set; }
    public string OverrideMethodName { get; set; }
  }
}