using System;
using System.Reflection;

namespace js.net.jish
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
          return loadedAssemblies.GetAssembly(assembly).GetType(typeName.Substring(0, typeName.IndexOf(',')));
        }
      } else
      {
        foreach (Assembly assembly in loadedAssemblies.GetAllAssemblies())
        {
          Type type = assembly.GetType(typeName);
          if (type != null) return type;
        }
      }
      return null; // Not found
    }
  }
}