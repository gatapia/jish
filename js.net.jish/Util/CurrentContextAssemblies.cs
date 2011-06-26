using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace js.net.jish.Util
{
  public class CurrentContextAssemblies
  {
    public IEnumerable<Assembly> GetAllAssemblies()
    {
      Assembly[] defaultAssemlies = AppDomain.CurrentDomain.GetAssemblies();
      if (!Directory.Exists("modules")) return defaultAssemlies;
      string[] assemblyFiles = Directory.GetFiles("modules", "*.dll", SearchOption.AllDirectories);
      if (assemblyFiles.Length == 0) return defaultAssemlies;      
      IEnumerable<Assembly> moduleAssemblies = assemblyFiles.Select(Assembly.LoadFrom);
      IEnumerable<Assembly> all = defaultAssemlies.Concat(moduleAssemblies);
      return all.Distinct(new AssemblyNameComparer());
    } 
  }
}
