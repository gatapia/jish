using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using js.net.Engine;

namespace js.net.repl
{
  public class REPLTypeImporter
  {    
    private readonly IEngine engine;
    private readonly JSConsole console;
    private readonly string typeName;

    public REPLTypeImporter(IEngine engine, string typeName, JSConsole console)
    {
      this.typeName = typeName;
      this.console = console;
      this.engine = engine;
    }

    public void ImportType()
    {      
      string className = typeName.Substring(typeName.LastIndexOf('.') + 1);
      Type t = Type.GetType(typeName);
      if (t == null)
      {
        console.log("Could not find type: " + typeName);
        return;
      }
      var dyn = new Dictionary<string, object>();
      ScrapeMethods(t, dyn);
      engine.SetGlobal(className, dyn);
      console.log(typeName + " imported.  Use like: " + className + ".Method(args);");
    }

    private void ScrapeMethods(Type targetType, IDictionary<string, object> methods)
    {
      foreach (MethodInfo mi in targetType.GetMethods(BindingFlags.Public | BindingFlags.Static))
      {
        // Note: Only adding one instance of each method (no overrides)
        if (methods.ContainsKey(mi.Name)) { continue; }
        methods.Add(mi.Name, ToDelegate(mi, null));
      }
    }

    public Delegate ToDelegate(MethodInfo mi, object target)
    {
      if (mi == null) throw new ArgumentNullException("mi");

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