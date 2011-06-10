using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace js.net.jish.IL
{
  public class TypeILWrapper
  {
    public object CreateWrapper(object instance)
    {
      Trace.Assert(instance != null);

      Type templateType = instance.GetType();
      return CreateWrapper(templateType, GetAllMethods(templateType, instance));
    }

    public object CreateWrapper(Type templateType)
    {
      Trace.Assert(templateType != null);

      return CreateWrapper(templateType, GetAllMethods(templateType, null));
    }    

    public object CreateWrapper(Type templateType, MethodToProxify[] methodsToProxify)
    {
      Trace.Assert(templateType != null);
      Trace.Assert(methodsToProxify != null && methodsToProxify.Length > 0);      

      string ns = templateType.Assembly.FullName;
      AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(ns), AssemblyBuilderAccess.Run);
      ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(ns);       
      TypeBuilder wrapperBuilder = moduleBuilder.DefineType(templateType.FullName + "Proxy", TypeAttributes.Public, typeof(JishProxy), new Type[0]);
      CreateProxyConstructor(wrapperBuilder);

      for (int i = 0; i < methodsToProxify.Length; i++)
      {     
        CreateProxyMethod(wrapperBuilder, i, methodsToProxify.ElementAt(i));
      }

      Type wrapperType = wrapperBuilder.CreateType();      
      return Activator.CreateInstance(wrapperType, new object[] { methodsToProxify });
    }

    private MethodToProxify[] GetAllMethods(Type templateType, object instance)
    {
      Trace.Assert(templateType != null);

      return templateType.GetMethods().Where(mi => !mi.Name.Equals("GetType")).Select(mi => new MethodToProxify(mi, instance)).ToArray();
    }

    private void CreateProxyConstructor(TypeBuilder wrapperBuilder)
    {
      Trace.Assert(wrapperBuilder != null);

      var consBuilder = wrapperBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] {typeof(MethodToProxify[])});

      var gen = consBuilder.GetILGenerator();
      gen.Emit(OpCodes.Ldarg_0); // This
      gen.Emit(OpCodes.Ldarg_1); // methodsToProxify
      ConstructorInfo superConstructor = typeof(JishProxy).GetConstructor(new [] {typeof(MethodToProxify[])});
      gen.Emit(OpCodes.Call, superConstructor);
      gen.Emit(OpCodes.Ret);
    }

    private void CreateProxyMethod(TypeBuilder wrapperBuilder, int thissIdx, MethodToProxify methodToProxify)
    {
      Trace.Assert(wrapperBuilder != null);
      Trace.Assert(thissIdx >= 0);
      Trace.Assert(methodToProxify != null);

      MethodInfo real = methodToProxify.RealMethod;
      ParameterInfo[] realParams = real.GetParameters();
      IList<IEnumerable<Type>> parameterCombinations = GetAllParameterCombinations(real);      
      foreach (IEnumerable<Type> parameters in parameterCombinations)
      {
        var methodBuilder = wrapperBuilder.DefineMethod(methodToProxify.OverrideMethodName ?? real.Name,
                                                        MethodAttributes.Public | MethodAttributes.Virtual,
                                                        real.ReturnType, parameters.Select(pt => pt).ToArray());
        if (real.GetGenericArguments().Length > 0) // Generics all get changed to objects
        {
          real = real.MakeGenericMethod(real.GetGenericArguments().Select(a => typeof (Object)).ToArray());
        }
        var gen = methodBuilder.GetILGenerator();
        if (!real.IsStatic)
        {
          // Set 'this' to the result of JishProxy.GetInstance. This allows one 
          // class to proxy to methods from different source classes.
          SetAReferenceToAppropriateThis(gen, thissIdx);
        } 
        for (int i = 0; i < parameters.Count(); i++)
        {          
          if (IsParamsArray(realParams[i]))
          {            
            break;  // Break as this is the last parameter (params must always be last)
          }
          // Else add standard inline arg
          gen.Emit(OpCodes.Ldarg, i + 1);
        }

        for (int i = parameters.Count(); i < realParams.Length; i++)
        {
          if (IsParamsArray(realParams[i])) break;

          gen.Emit(OpCodes.Ldarg_0);
          gen.Emit(OpCodes.Ldc_I4, thissIdx); // Load the this index into the stack for GetInstance param          
          gen.Emit(OpCodes.Ldc_I4, i);
          MethodInfo getLastOptional = typeof (JishProxy).GetMethod("GetOptionalParameterDefaultValue");
          getLastOptional = getLastOptional.MakeGenericMethod(new[] {realParams[i].ParameterType});
          gen.Emit(OpCodes.Callvirt, getLastOptional);
        }
        ParameterInfo last = realParams.Any() ? realParams.Last() : null;
        if (last != null && IsParamsArray(last))
        {
          CovertRemainingParametersToArray(parameters, gen, realParams.Count() - 1, last.ParameterType.GetElementType());
        }
        // Call the real method
        gen.Emit(real.IsStatic ? OpCodes.Call : OpCodes.Callvirt, real); 
        gen.Emit(OpCodes.Ret);
      }
    }

    private void CovertRemainingParametersToArray(IEnumerable<Type> parameters, ILGenerator gen, int startingIndex, Type arrayElementType)
    {
      Trace.Assert(parameters != null);
      Trace.Assert(gen != null);
      Trace.Assert(startingIndex >= 0);
      Trace.Assert(arrayElementType != null);

      gen.Emit(OpCodes.Ldc_I4, Math.Max(0, parameters.Count() - startingIndex));
      gen.Emit(OpCodes.Newarr, arrayElementType);  
      for (int i = startingIndex; i < parameters.Count(); i++)
      {
        gen.Emit(OpCodes.Dup);
        gen.Emit(OpCodes.Ldc_I4, i - startingIndex);
        gen.Emit(OpCodes.Ldarg, i + 1);
                  
        gen.Emit(OpCodes.Stelem, arrayElementType);
      }
    }

    private void SetAReferenceToAppropriateThis(ILGenerator gen, int thissIdx)
    {
      Trace.Assert(gen != null);
      Trace.Assert(thissIdx >= 0);

      gen.Emit(OpCodes.Ldarg_0); // Load this argument onto stack
      gen.Emit(OpCodes.Ldc_I4, thissIdx); // Load the this index into the stack for GetInstance param
      gen.Emit(OpCodes.Callvirt, typeof (JishProxy).GetMethod("GetInstance")); // Call get instance and pop the current this pointer
    }

    private IList<IEnumerable<Type>> GetAllParameterCombinations(MethodInfo mi)
    {
      Trace.Assert(mi != null);

      IList<IEnumerable<Type>> combinations = new List<IEnumerable<Type>>();
      ParameterInfo[] realParams = mi.GetParameters();
      if (realParams.Length == 0 || (!realParams.Last().IsOptional && !IsParamsArray(realParams.Last())))
      {
        combinations.Add(GetParamCombination(realParams, realParams.Length));
        return combinations;
      }

      int firstNonRequiredIndex = 0;
      // First pass finds the required method signature
      for (int i = 0; i < realParams.Length; i++)
      {
        ParameterInfo pi = realParams[i];
        if (pi.IsOptional || IsParamsArray(pi))
        {
          firstNonRequiredIndex = i;
          break;
        }        
      }
      // Add all required params as first combination
      combinations.Add(GetParamCombination(realParams, firstNonRequiredIndex));
      for (int i = firstNonRequiredIndex; i < realParams.Length; i++)
      {
        ParameterInfo pi = realParams[i];
        if (pi.IsOptional)
        {
          combinations.Add(GetParamCombination(realParams, i + 1));
        } else if (IsParamsArray(pi))
        {
          for (int j = 1; j < 17; j++)
          {
            combinations.Add(GetParamCombination(realParams, realParams.Count() - 1, realParams.Last().ParameterType.GetElementType(), j));
          }
        }
      }
      return combinations;
    }

    private bool IsParamsArray(ParameterInfo param)
    {
      Trace.Assert(param != null);

      return Attribute.IsDefined(param, typeof (ParamArrayAttribute));
    }

    private IEnumerable<Type> GetParamCombination(ParameterInfo[] realParams, int until, Type extraParamType = null, int extraParams = 0)
    {
      Trace.Assert(realParams != null);
      Trace.Assert(until >= 0);
      Trace.Assert(extraParams >= 0);
      if (extraParams > 0) Trace.Assert(extraParamType != null);
      else Trace.Assert(extraParamType == null);

      IList<Type> combo = new List<Type>();
      for (int i = 0; i < until; i++)
      {
        ParameterInfo pi = realParams[i];
        combo.Add(pi.ParameterType.IsGenericParameter ? typeof(object) : pi.ParameterType);
      }
      for (int i = 0; i < extraParams; i++)
      {        
        combo.Add(extraParamType);
      }
      return combo;
    }
  }
}