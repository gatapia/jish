using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using js.net.Engine;

namespace js.net.repl
{
  public class REPLTypeImporter
  {    
    private readonly IDictionary<string, Assembly> additionalLoadedAssemblies;
    private readonly IEngine engine;
    private readonly JSConsole console;
    private readonly string typeName;    

    public REPLTypeImporter(IDictionary<string, Assembly> additionalLoadedAssemblies, IEngine engine, string typeName, JSConsole console)
    {
      this.additionalLoadedAssemblies = additionalLoadedAssemblies;
      this.typeName = typeName;
      this.console = console;
      this.engine = engine;
    }

    public void ImportType()
    {
      string typeNameWithoutAssembly = typeName.IndexOf(',') >= 0 ? typeName.Substring(0, typeName.IndexOf(',')) : typeName;
      string className = typeNameWithoutAssembly.Substring(typeNameWithoutAssembly.LastIndexOf('.') + 1);
      Type t = LoadType();
      if (t == null)
      {
        console.log("Could not find type: " + typeName);
        return;
      }
      var dyn = new Dictionary<string, object>();
      ScrapeMethods(t, dyn);
      engine.SetGlobal(className, dyn);
      console.log(typeNameWithoutAssembly + " imported.  Use like: " + className + ".Method(args);");
    }

    private Type LoadType()
    {
      if (typeName.IndexOf(',') > 0)
      {
        string assembly = typeName.Substring(typeName.IndexOf(',') + 1).Trim();
        if (additionalLoadedAssemblies.ContainsKey(assembly))
        {
          return additionalLoadedAssemblies[assembly].GetType(typeName.Substring(0, typeName.IndexOf(',')));
        }
      }
      return Type.GetType(typeName);
    }

    private void ScrapeMethods(Type targetType, IDictionary<string, object> methods)
    {
      foreach (MethodInfo mi in targetType.GetMethods(BindingFlags.Public | BindingFlags.Static))
      {        
        // Note: Only adding one instance of each method (no overrides) and 
        // ignoring generic methods
        if (methods.ContainsKey(mi.Name) || mi.GetGenericArguments().Length > 0) { continue; }
        methods.Add(mi.Name, ToDelegate(mi, null));
      }
    }

    public Delegate ToDelegate(MethodInfo mi, object target)
    {
      Trace.Assert(mi != null);

      Type delegateType;

      List<Type> typeArgs = mi.GetParameters()
        .Select(p => p.ParameterType)
        .ToList();

      // builds a delegate type
      if (mi.ReturnType == typeof (void))
      {
        delegateType = Expression.GetActionType(typeArgs.ToArray());
      }
      else
      {
        typeArgs.Add(mi.ReturnType);
        delegateType = Expression.GetFuncType(typeArgs.ToArray());
      }

      // creates a binded delegate if target is supplied
      Delegate result = (target == null)
                          ? Delegate.CreateDelegate(delegateType, mi)
                          : Delegate.CreateDelegate(delegateType, target, mi);

      return result;
    }
  }
}