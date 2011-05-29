using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace js.net.jish
{
  public class TypeImporter
  {    
    private readonly IJishInterpreter jish;
    private readonly DelegateBuilder delegateBuilder = new DelegateBuilder();

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
      if (!dyn.Any())
      {
        jish.JavaScriptConsole.log("Could not import type: " + typeName);
        return;
      }
      jish.SetGlobal(className, dyn);
      jish.JavaScriptConsole.log(typeNameWithoutAssembly + " imported.  Use like: " + className + "." + dyn.First().Key + "(args);");
    }    

    private void ScrapeMethods(Type targetType, IDictionary<string, object> methods)
    {
      MethodInfo[] mis = targetType.GetMethods(BindingFlags.Public | BindingFlags.Static);
      foreach (MethodInfo mi in mis)
      {                
        // TODO: ignoring generic methods
        if (methods.ContainsKey(mi.Name) || mi.GetGenericArguments().Length > 0) { continue; }
        MethodInfo[] allOverrides = mis.Where(minf => minf.Name.Equals(mi.Name)).ToArray();
        Delegate del = allOverrides.Count() == 1 ? 
          delegateBuilder.ToDelegate(mi, null) : 
          delegateBuilder.ToOverridenMethodDelegate(allOverrides, null);
        methods.Add(mi.Name, del);
      }
    }    
  }
}