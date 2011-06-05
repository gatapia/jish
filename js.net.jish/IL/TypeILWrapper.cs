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
      Trace.Assert(methodsToProxify != null && methodsToProxify.Length > 0);      

      string ns = templateType.Assembly.FullName;
      AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(ns), AssemblyBuilderAccess.RunAndSave);
      ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(ns, "testassembly.dll");       
      TypeBuilder wrapperBuilder = moduleBuilder.DefineType(templateType.FullName, TypeAttributes.Public, typeof(JishProxy), new Type[0]);
      CreateProxyConstructor(wrapperBuilder);

      for (int i = 0; i < methodsToProxify.Length; i++)
      {     
        CreateProxyMethod(wrapperBuilder, i, methodsToProxify.ElementAt(i));
      }

      Type wrapperType = wrapperBuilder.CreateType();
      assemblyBuilder.Save("testassembly.dll");
      object[] thiss = methodsToProxify.Select(m => m.MethodContext).ToArray();
      MethodInfo[] realMethods = methodsToProxify.Select(pm => pm.RealMethod).ToArray();
      return Activator.CreateInstance(wrapperType, new object[] { thiss, realMethods });
    }

    private MethodToProxify[] GetAllMethods(Type templateType, object instance)
    {
      return templateType.GetMethods().Where(mi => !mi.Name.Equals("GetType")).Select(mi => new MethodToProxify(mi, instance)).ToArray();
    }

    private void CreateProxyConstructor(TypeBuilder wrapperBuilder)
    {
      var consBuilder = wrapperBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] {typeof(object[]), typeof(MethodInfo[])});

      var gen = consBuilder.GetILGenerator();
      gen.Emit(OpCodes.Ldarg_0); // This
      gen.Emit(OpCodes.Ldarg_1); // thiss
      gen.Emit(OpCodes.Ldarg_2); // realMethods
      ConstructorInfo superConstructor = typeof(JishProxy).GetConstructor(new [] {typeof(object[]), typeof(MethodInfo[])});
      gen.Emit(OpCodes.Call, superConstructor);
      gen.Emit(OpCodes.Ret);
    }

    private void CreateProxyMethod(TypeBuilder wrapperBuilder, int thissIdx, MethodToProxify methodToProxify)
    {
      MethodInfo real = methodToProxify.RealMethod;
      ParameterInfo[] realParams = real.GetParameters();
      IList<IEnumerable<Type>> parameterCombinations = GetAllParameterCombinations(real);      
      foreach (IEnumerable<Type> parameters in parameterCombinations)
      {
        var methodBuilder = wrapperBuilder.DefineMethod(methodToProxify.OverrideMethodName ?? real.Name,
                                                        MethodAttributes.Public | MethodAttributes.Virtual,
                                                        real.ReturnType, parameters.Select(pt => pt).ToArray());
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
          gen.Emit(OpCodes.Ldc_I4, i + 1);
          MethodInfo getLastOptional = typeof (JishProxy).GetMethod("GetOptionalParameterDefaultValue");
          getLastOptional = getLastOptional.MakeGenericMethod(new[] {realParams.Last().ParameterType});
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

    private void CovertRemainingParametersToArray(IEnumerable<Type> parameters, ILGenerator gen, int startingIndex, Type arrayType)
    {
      gen.Emit(OpCodes.Ldc_I4, parameters.Count() - startingIndex);
      gen.Emit(OpCodes.Newarr, arrayType);  
      for (int i = startingIndex; i < parameters.Count(); i++)
      {
        gen.Emit(OpCodes.Dup);
        gen.Emit(OpCodes.Ldc_I4, i - startingIndex);
        gen.Emit(OpCodes.Ldarg, i + 1);
                  
        gen.Emit(OpCodes.Stelem, arrayType);
      }
    }

    private void SetAReferenceToAppropriateThis(ILGenerator gen, int thissIdx)
    {
      gen.Emit(OpCodes.Ldarg_0); // Load this argument onto stack
      gen.Emit(OpCodes.Ldc_I4, thissIdx); // Load the this index into the stack for GetInstance param
      gen.Emit(OpCodes.Callvirt, typeof (JishProxy).GetMethod("GetInstance")); // Call get instance and pop the current this pointer
    }

    private IList<IEnumerable<Type>> GetAllParameterCombinations(MethodInfo mi)
    {
      IList<IEnumerable<Type>> combinations = new List<IEnumerable<Type>>();
      ParameterInfo[] realParams = mi.GetParameters();
      if (realParams.Length == 0)
      {
        combinations.Add(GetParamCombination(realParams, realParams.Length));
        return combinations;
      }

      bool isParams = IsParamsArray(realParams.Last());
      if (isParams)
      {
        for (int j = 0; j < 17; j++)
        {
          combinations.Add(GetParamCombination(realParams, realParams.Count() - 1, realParams.Last().ParameterType.GetElementType(), j));
        }
      }

      for (int i = realParams.Length; --i >= 0;)
      {
        // Already added an entry, so ignore first iteration
        if (i < realParams.Length - 1) { isParams = false; } 

        ParameterInfo param = realParams[i];
        if (param.IsOptional)
        {
          // combinations.Add(GetParamCombination(realParams, i));
          if (!isParams) combinations.Add(GetParamCombination(realParams, i + 1));
          if (i == 0) { combinations.Add(GetParamCombination(realParams, 0)); }
        } else // No more params or optionals
        {
          if (!isParams) combinations.Add(GetParamCombination(realParams, i + 1));
          break;
        }
      }
      return combinations;
    }

    private bool IsParamsArray(ParameterInfo param)
    {
      return Attribute.IsDefined(param, typeof (ParamArrayAttribute));
    }

    private IEnumerable<Type> GetParamCombination(ParameterInfo[] realParams, int until, Type extraParamType = null, int extraParams = 0)
    {
      IList<Type> combo = new List<Type>();
      for (int i = 0; i < until; i++)
      {
        ParameterInfo pi = realParams[i];
        combo.Add(pi.ParameterType);
      }
      for (int i = 0; i < extraParams; i++)
      {        
        combo.Add(extraParamType);
      }
      return combo;
    }
  }
}