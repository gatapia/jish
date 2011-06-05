using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace js.net.jish.Util
{
  public class TypeLoader
  {
    private readonly LoadedAssembliesBucket loadedAssemblies;

    public TypeLoader(LoadedAssembliesBucket loadedAssemblies)
    {
      this.loadedAssemblies = loadedAssemblies;
    }

    public Type LoadType(string typeName)
    {      
      if (typeName.IndexOf(',') > 0)
      {
        string assembly = typeName.Substring(typeName.IndexOf(',') + 1).Trim();
        if (loadedAssemblies.ContainsAssembly(assembly))
        {
          string shortTypeName = typeName.Substring(0, typeName.IndexOf(','));
          return GetTypeFromAssemblyGenericSafe(loadedAssemblies.GetAssembly(assembly), shortTypeName);
        }
      } else
      {
        foreach (Assembly assembly in loadedAssemblies.GetAllAssemblies())
        {
          Type type = GetTypeFromAssemblyGenericSafe(assembly, typeName);
          if (type != null) return type;
        }
      }
      return null; // Not found
    }

    private Type GetTypeFromAssemblyGenericSafe(Assembly ass,  string typeName)
    {
      Type[] allTypes = ass.GetTypes();
      // Get exact match or first generic match
      
      return allTypes.SingleOrDefault(t => t.FullName.Equals(typeName)) 
        ?? allTypes.Where(t => t.FullName.StartsWith(typeName + "`")).FirstOrDefault();
    }
  }
}