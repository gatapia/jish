using System.Collections.Generic;
using System.Reflection;

namespace js.net.jish.Util
{
  public interface ICurrentContextAssemblies {
    IEnumerable<Assembly> GetAllAssemblies();
  }
}