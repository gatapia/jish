using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace js.net.jish.Util
{
  public class LoadedAssembliesBucket
  {
    private readonly IDictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();
    
    public void AddAssembly(Assembly a)
    {
      if (assemblies.ContainsKey(a.GetName().Name))
      {
        return;
      }
      assemblies.Add(a.GetName().Name, a);
    }

    public bool ContainsAssembly(string assemblyName)
    {
      return assemblies.ContainsKey(GetShortNameFrom(assemblyName));
    }

    public Assembly GetAssembly(string assemblyName)
    {
      return assemblies[GetShortNameFrom(assemblyName)];
    }

    private string GetShortNameFrom(string assemblyName)
    {
      if (assemblyName.IndexOf(',') < 0 ) return assemblyName;
      return assemblyName.Substring(0, assemblyName.IndexOf(','));
    }

    public IEnumerable<Assembly> GetAllAssemblies()
    {
      return assemblies.Values.ToArray();
    }
  }
}