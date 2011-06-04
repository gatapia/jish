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

    public object CreateWrapper(Type templateType, ProxyMethod[] methods)
    {
      Trace.Assert(methods != null && methods.Length > 0);      

      string ns = templateType.Assembly.FullName;
      AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(ns), AssemblyBuilderAccess.RunAndSave);
      ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(ns, "testassembly.dll");       
      TypeBuilder wrapperBuilder = moduleBuilder.DefineType(templateType.FullName, TypeAttributes.Public, typeof(JishProxy), new Type[0]);
      CreateProxyConstructor(wrapperBuilder);

      for (int i = 0; i < methods.Length; i++)
      {     
        CreateProxyMethod(wrapperBuilder, i, methods.ElementAt(i));
      }

      Type wrapperType = wrapperBuilder.CreateType();
      assemblyBuilder.Save("testassembly.dll");
      return Activator.CreateInstance(wrapperType, new object[] { methods.Select(m => m.MethodContext).ToArray() });
    }

    private ProxyMethod[] GetAllMethods(Type templateType, object instance)
    {
      return templateType.GetMethods().Where(mi => !mi.Name.Equals("GetType")).Select(mi => new ProxyMethod(mi, instance)).ToArray();
    }

    private void CreateProxyConstructor(TypeBuilder wrapperBuilder)
    {
      var consBuilder = wrapperBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] {typeof(object[])});
      var gen = consBuilder.GetILGenerator();
      gen.Emit(OpCodes.Ldarg_0); // This
      gen.Emit(OpCodes.Ldarg_1); // thiss
      ConstructorInfo superConstructor = typeof(JishProxy).GetConstructor(new [] {typeof(object[])});
      gen.Emit(OpCodes.Call, superConstructor);
      gen.Emit(OpCodes.Ret);
    }

    private void CreateProxyMethod(TypeBuilder wrapperBuilder, int thissIdx, ProxyMethod method)
    {
      MethodInfo real = method.RealMethod;
      ParameterInfo[] realParams = real.GetParameters();
      IList<IEnumerable<Type>> parameterCombinations = GetAllParameterCombinations(real);      
      foreach (IEnumerable<Type> parameters in parameterCombinations)
      {
        var methodBuilder = wrapperBuilder.DefineMethod(method.OverrideMethodName ?? real.Name,
                                                        MethodAttributes.Public | MethodAttributes.Virtual,
                                                        real.ReturnType, parameters.Select(pt => pt).ToArray());
        var gen = methodBuilder.GetILGenerator();
        if (!real.IsStatic)
        {
          // Set 'this' to the result of JishProxy.GetInstance. This allows one 
          // class to proxy to methods from different source classes.
          SetAReferenceToAppropriateThis(gen, thissIdx);
        } 
        int len = parameters.Count() + (realParams.Length > 0 && IsParamsArray(realParams.Last())  ? 1 : 0);
        for (int i = 0; i < len; i++)
        {          
          if (IsParamsArray(realParams[i]))
          {
            CovertRemainingParametersToArray(parameters, gen, i, realParams[i].ParameterType.GetElementType());
            break; 
          }
          // Else add standard inline arg
          gen.Emit(OpCodes.Ldarg, i + 1);
        }
        if (realParams.Length > parameters.Count()) // Emitted Optional
        {
          Console.WriteLine("TODO");
          // TODO:
          // gen.Emit(OpCodes.Ldarg, real.GetParameters().Last().DefaultValue);
        }        

        gen.Emit(real.IsStatic ? OpCodes.Call : OpCodes.Callvirt, real); // Call the real method
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
          
        // if (parameters.ElementAt(i).IsValueType) { gen.Emit(OpCodes.Box, parameters.ElementAt(i)); } // Box if required

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