using System;
using System.Collections.Generic;
using System.Reflection;

namespace js.net.jish
{
  public class TypeImporter
  {    
    private readonly IJishInterpreter jish;

    public TypeImporter(IJishInterpreter jish)
    {      
      this.jish = jish;     
    }

    public void ImportType(string typeName)
    {
      string typeNameWithoutAssembly = typeName.IndexOf(',') >= 0 ? typeName.Substring(0, typeName.IndexOf(',')) : typeName;
      string className = typeNameWithoutAssembly.Substring(typeNameWithoutAssembly.LastIndexOf('.') + 1);
      Type t = new TypeLoader().LoadType(typeName, jish.GetLoadedAssemblies());
      if (t == null)
      {
        jish.JavaScriptConsole.log("Could not find type: " + typeName);
        return;
      }
      var dyn = new Dictionary<string, object>();
      ScrapeMethods(t, dyn);
      jish.SetGlobal(className, dyn);
      jish.JavaScriptConsole.log(typeNameWithoutAssembly + " imported.  Use like: " + className + ".Method(args);");
    }    

    private void ScrapeMethods(Type targetType, IDictionary<string, object> methods)
    {
      foreach (MethodInfo mi in targetType.GetMethods(BindingFlags.Public | BindingFlags.Static))
      {        
        // Note: Only adding one instance of each method (no overrides) and 
        // ignoring generic methods
        if (methods.ContainsKey(mi.Name) || mi.GetGenericArguments().Length > 0) { continue; }
        methods.Add(mi.Name, new DelegateBuilder().ToDelegate(mi, null));
      }
    }    
  }
}