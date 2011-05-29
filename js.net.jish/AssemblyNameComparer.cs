using System.Collections.Generic;
using System.Reflection;

namespace js.net.jish
{
  internal class AssemblyNameComparer : IEqualityComparer<Assembly>
  {
    public bool Equals(Assembly x, Assembly y)
    {
      return x.GetName().Name == y.GetName().Name;
    }

    public int GetHashCode(Assembly a)
    {
      return a.GetName().Name.GetHashCode();
    }
  }
}