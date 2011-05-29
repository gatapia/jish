using System;
using System.Collections.Generic;
using System.Reflection;

namespace js.net.jish
{
  public class TypeLoader
  {
    public Type LoadType(string typeName, IDictionary<string, Assembly> assemblies)
    {
      if (typeName.IndexOf(',') > 0)
      {
        string assembly = typeName.Substring(typeName.IndexOf(',') + 1).Trim();
        if (assemblies.ContainsKey(assembly))
        {
          return assemblies[assembly].GetType(typeName.Substring(0, typeName.IndexOf(',')));
        }
      }
      return Type.GetType(typeName);
    }
  }
}