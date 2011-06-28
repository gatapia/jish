using System.Diagnostics;
using System.Reflection;

namespace js.net.jish.IL
{
  public class MethodToProxify
  {    
    public MethodToProxify(MethodInfo realMethod, object realMethodTargetInstance)
    {
      Trace.Assert(realMethod != null);

      RealMethod = realMethod;
      TargetInstance = realMethodTargetInstance;
    }

    public object TargetInstance { get; private set; }
    public MethodInfo RealMethod { get; private set; }
    public string OverrideMethodName { get; set; }

    public override string ToString() { return RealMethod.Name; }
  }
}