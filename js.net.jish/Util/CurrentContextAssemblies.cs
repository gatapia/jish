using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
<<<<<<< HEAD

namespace js.net.jish.Util
{
  public class CurrentContextAssemblies : ICurrentContextAssemblies
  {
    public IEnumerable<Assembly> GetAllAssemblies()
    {
      IEnumerable<Assembly> defaultAssemlies = GetCurrentDomainAssemblies();
=======
using System.Text;

namespace js.net.jish.Util
{
  public class CurrentContextAssemblies
  {
    public IEnumerable<Assembly> GetAllAssemblies()
    {
      Assembly[] defaultAssemlies = AppDomain.CurrentDomain.GetAssemblies();
>>>>>>> 31c2234f1232e849840f8c61486e30f242a28fd1
      if (!Directory.Exists("modules")) return defaultAssemlies;
      string[] assemblyFiles = Directory.GetFiles("modules", "*.dll", SearchOption.AllDirectories);
      if (assemblyFiles.Length == 0) return defaultAssemlies;      
      IEnumerable<Assembly> moduleAssemblies = assemblyFiles.Select(Assembly.LoadFrom);
      IEnumerable<Assembly> all = defaultAssemlies.Concat(moduleAssemblies);
      return all.Distinct(new AssemblyNameComparer());
<<<<<<< HEAD
    }

    protected virtual IEnumerable<Assembly> GetCurrentDomainAssemblies() { return AppDomain.CurrentDomain.GetAssemblies(); }
=======
    } 
>>>>>>> 31c2234f1232e849840f8c61486e30f242a28fd1
  }
}
