using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using js.net.jish.IL;
using js.net.Util;

namespace js.net.jish.Util
{
  public class TypeCreator
  {
    private readonly JSConsole console;
    private readonly TypeLoader typeLoader;

    public TypeCreator(JSConsole console, TypeLoader typeLoader)
    {
      Trace.Assert(console != null);
      Trace.Assert(typeLoader != null);

      this.typeLoader = typeLoader;
      this.console = console;
    }

    public object CreateType(string typeName, params object[] constructorArgs)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(typeName));

      Type t = typeLoader.LoadType(typeName);
      if (t == null)
      {
        console.log("Could not find a matching type: " + typeName);
        return null;
      }
      object[] realArgs = ConvertToActualArgs(constructorArgs, t);
      if (realArgs == null)
      {
        console.log("Could not find a matching constructor.");
        return null;
      }
      
      if (t.ContainsGenericParameters)
      {
        t = t.MakeGenericType(t.GetGenericArguments().Select(a => typeof(object)).ToArray());
      }
      TypeILWrapper wrapper = new TypeILWrapper();
      object instance = t.GetConstructors().Length == 0 
        ? wrapper.CreateWrapperFromType(t) 
        : wrapper.CreateWrapperFromInstance(Activator.CreateInstance(t, realArgs));            
      console.log("Created instance of " + typeName + ".");
      return instance;
    }

    private object[] ConvertToActualArgs(object[] args, Type type)
    {      
      Trace.Assert(type != null);

      if (args == null || args.Length == 0) return new object[] {};      
      IEnumerable<ConstructorInfo> possibleConstructors = type.GetConstructors().Where(c => c.GetParameters().Length == args.Length);      
      foreach (ConstructorInfo ci in possibleConstructors)
      {
        IEnumerable<object> realArgs = ConvertToActualArgs(args, ci);
        if (realArgs != null) return realArgs.ToArray();
      }
      return null;
    }

    private IEnumerable<object> ConvertToActualArgs(object[] args, ConstructorInfo ci)
    {
      Trace.Assert(args != null);
      Trace.Assert(ci != null);

      IList<object> realArgs =new List<object>();
      for (int i = 0; i < args.Length; i++)
      {
        Type t = ci.GetParameters()[i].ParameterType;
        Type exp = t.IsGenericParameter ? typeof(object) : t;
        object thisArg = args[i];
        try
        {
          object actual = Convert.ChangeType(thisArg, exp);
          if (actual == null && thisArg != null) return null; // Not matching        
          realArgs.Add(actual);        
        } catch (FormatException)
        {
          return null; // Not matching
        }
      }
      return realArgs;
    }
  }
}
