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