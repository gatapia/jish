using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace js.net.jish
{
  public class DelegateBuilder
  {
    public Delegate ToDelegate(MethodInfo mi, object target)
    {
      Trace.Assert(mi != null);
      

      Type delegateType;
      List<Type> typeArgs = mi.GetParameters().Select(p => p.ParameterType).ToList();      
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

    public Delegate ToOverridenMethodDelegate(IEnumerable<MethodInfo> mis, object target)
    {
      Trace.Assert(mis != null);
      Trace.Assert(mis.Count() > 1);
      Trace.Assert(mis.Select(mi => mi.Name).Distinct().Count() == 1);

      OverridenMethodDelegator delegator = new OverridenMethodDelegator(mis, target);
      return ToDelegate(typeof (OverridenMethodDelegator).GetMethod("DelegateTo"), delegator);
    }
  }

  public class OverridenMethodDelegator
  {
    private readonly IEnumerable<MethodInfo> methods;
    private readonly object target;
    public OverridenMethodDelegator(IEnumerable<MethodInfo> methods, object target)
    {
      this.methods = methods;
      this.target = target;
    }

    public object DelegateTo(object[] args)
    {
      int argCount = args == null ? 0 : args.Length;
      MethodInfo mi = methods.Single(m => m.GetParameters().Length == argCount);
      return mi.Invoke(target, args);
    }
  }
}