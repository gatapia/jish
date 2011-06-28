using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace js.net.jish.Util
{
  public class AssemblyNameComparer : IEqualityComparer<Assembly>
  {
    public bool Equals(Assembly x, Assembly y)
    {
      Trace.Assert(x != null);
      Trace.Assert(y != null);

      return x.GetName().Name == y.GetName().Name;
    }

    public int GetHashCode(Assembly a)
    {
      Trace.Assert(a != null);

      return a.GetName().Name.GetHashCode();
    }
  }
}