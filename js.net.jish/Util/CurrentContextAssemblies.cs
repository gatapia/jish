using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace js.net.jish.Util
{
  public class CurrentContextAssemblies : ICurrentContextAssemblies
  {
    public IEnumerable<Assembly> GetAllAssemblies()
    {
      IEnumerable<Assembly> defaultAssemlies = GetCurrentDomainAssemblies();
      if (!Directory.Exists("modules")) return defaultAssemlies;
      string[] assemblyFiles = Directory.GetFiles("modules", "*.dll", SearchOption.AllDirectories);
      if (assemblyFiles.Length == 0) return defaultAssemlies;      
      IEnumerable<Assembly> moduleAssemblies = assemblyFiles.Select(Assembly.LoadFrom);
      IEnumerable<Assembly> all = defaultAssemlies.Concat(moduleAssemblies);
      return all.Distinct(new AssemblyNameComparer());
    }

    protected virtual IEnumerable<Assembly> GetCurrentDomainAssemblies() { return AppDomain.CurrentDomain.GetAssemblies(); }
  }
}
