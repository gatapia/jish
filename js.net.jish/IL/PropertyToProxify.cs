using System.Diagnostics;
using System.Reflection;

namespace js.net.jish.IL
{
  public class PropertyToProxify
  {    
    public PropertyToProxify(PropertyInfo realProperty, object realMethodTargetInstance)
    {
      Trace.Assert(realProperty != null);

      RealProperty = realProperty;
      TargetInstance = realMethodTargetInstance;
    }

    public object TargetInstance { get; private set; }
    public PropertyInfo RealProperty { get; private set; }

    public override string ToString() { return RealProperty.Name; }
  }
}