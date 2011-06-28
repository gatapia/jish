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
    private const bool SAVE_TEST_DLL = false;

    private object instance;
    public object CreateWrapperFromInstance(object instance)
    {      
      Trace.Assert(instance != null);
      this.instance = instance;

      Type templateType = instance.GetType();
      return CreateWrapperFromType(templateType, GetAllMethods(templateType), GetAllProperties(templateType));
    }    

    public object CreateWrapperFromType(Type templateType)
    {
      Trace.Assert(templateType != null);

      return CreateWrapperFromType(templateType, GetAllMethods(templateType), GetAllProperties(templateType));
    }  

    public object CreateWrapperFromType(Type templateType, MethodToProxify[] methodsToProxify, PropertyToProxify[] propsToProxify = null)
    {
      Trace.Assert(templateType != null);
      Trace.Assert(methodsToProxify != null && methodsToProxify.Length > 0);      

      string ns = templateType.Assembly.FullName;
      AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(ns), SAVE_TEST_DLL ? AssemblyBuilderAccess.RunAndSave : AssemblyBuilderAccess.Run);
      ModuleBuilder moduleBuilder = SAVE_TEST_DLL ? assemblyBuilder.DefineDynamicModule(ns, "testil.dll") : assemblyBuilder.DefineDynamicModule(ns);       
      TypeBuilder wrapperBuilder = moduleBuilder.DefineType(templateType.FullName + "Proxy", TypeAttributes.Public, typeof(JishProxy), new Type[0]);
      CreateProxyConstructor(wrapperBuilder);

      for (int i = 0; i < methodsToProxify.Length; i++)
      {     
        CreateProxyMethod(wrapperBuilder, i, methodsToProxify.ElementAt(i));
      }
      if (propsToProxify != null)
      {
        for (int i = 0; i < propsToProxify.Length; i++)
        {
          GenerateProxyProperty(wrapperBuilder, i, propsToProxify.ElementAt(i));
        }
      }
      Type wrapperType = wrapperBuilder.CreateType();                  
      
      if (SAVE_TEST_DLL) assemblyBuilder.Save("testil.dll");
      
      return Activator.CreateInstance(wrapperType, new object[] { methodsToProxify });
    }

    private void GenerateProxyProperty(TypeBuilder wrapperBuilder, int thisidx, PropertyToProxify propertyToProxify) {
      Trace.Assert(wrapperBuilder != null);
      Trace.Assert(thisidx >= 0);
      Trace.Assert(propertyToProxify != null);
      
      PropertyInfo realProp = propertyToProxify.RealProperty;
      Console.WriteLine("GenerateProxyProperty: " + realProp.Name);
      PropertyBuilder propBuilder = wrapperBuilder.DefineProperty(realProp.Name, PropertyAttributes.HasDefault, realProp.PropertyType, new [] {realProp.PropertyType});
      
      if (realProp.CanRead && realProp.GetGetMethod(false) != null)
      {
        MethodBuilder getMethod = wrapperBuilder.DefineMethod("get_internal_" + realProp.Name, MethodAttributes.Public, realProp.PropertyType, new Type[] {});
        ILGenerator gen = getMethod.GetILGenerator();

        SetReferenceToAppropriateThis(gen, thisidx);
        gen.Emit(OpCodes.Call, realProp.GetGetMethod()); 
        gen.Emit(OpCodes.Ret);      
 
        propBuilder.SetGetMethod(getMethod);
      }

      if (realProp.CanWrite && realProp.GetSetMethod(false) != null)
      {
        MethodBuilder setMethod = wrapperBuilder.DefineMethod("set_internal_" + realProp.Name, MethodAttributes.Public, null, new[] {realProp.PropertyType});
        ILGenerator gen = setMethod.GetILGenerator();        

        SetReferenceToAppropriateThis(gen, 0);
        gen.Emit(OpCodes.Ldarg_1);
        gen.Emit(OpCodes.Call, realProp.GetSetMethod()); 
        gen.Emit(OpCodes.Ret);     

        propBuilder.SetSetMethod(setMethod);
      }      
      
    }

    private bool IsValidDelegateMethod(MethodToProxify mp)
    {
      ParameterInfo[] parameters = mp.RealMethod.GetParameters();
      ParameterInfo[] delegateParams = parameters.Where(IsParamDelegate).ToArray();      
      if (delegateParams.Length == 0) return false;
      if (delegateParams.Length > 1) throw new NotSupportedException("Only single delegate callback methods supported. I.e. Cannot have a callback and an errback.");
      return true;
    }

    private bool IsParamDelegate(ParameterInfo p) { return typeof (Delegate).IsAssignableFrom(p.ParameterType.BaseType); }

    private MethodToProxify[] GetAllMethods(Type templateType)
    {
      Trace.Assert(templateType != null);

      IEnumerable<MethodInfo> methodInfos = templateType.GetMethods().Where(mi => !mi.Name.Equals("GetType"));      
      MethodToProxify[] methods = methodInfos.Select(mi => new MethodToProxify(mi, instance)).ToArray();      
      return methods;
    }    

    private PropertyToProxify[] GetAllProperties(Type templateType)
    {
      Trace.Assert(templateType != null);

      IEnumerable<PropertyInfo> props = templateType.GetProperties();      
      PropertyToProxify[] propertiesToProxify = props.Select(pi => new PropertyToProxify(pi, instance)).ToArray();      
      return propertiesToProxify;
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

      MethodInfo realMethod = methodToProxify.RealMethod;
      ParameterInfo[] realParams = realMethod.GetParameters();
      IList<Type[]> parameterCombinations = GetAllParameterCombinations(realMethod);      
      foreach (Type[] parameters in parameterCombinations)
      {
        var methodBuilder = wrapperBuilder.DefineMethod(GetProxMethodName(methodToProxify, realMethod),
                                                        MethodAttributes.Public | MethodAttributes.Virtual,
                                                        realMethod.ReturnType, parameters);
        if (realMethod.GetGenericArguments().Length > 0)
        {
          // Generics all get changed to objects
          realMethod = realMethod.MakeGenericMethod(realMethod.GetGenericArguments().Select(a => typeof (Object)).ToArray());
        }
        EmitMethodWithParameterCombo(thissIdx, realMethod, parameters, methodBuilder, realParams);
      }
    }

    private void EmitMethodWithParameterCombo(int thissIdx, MethodInfo realMethod, Type[] parameters, MethodBuilder methodBuilder, ParameterInfo[] realParams) {      
      var gen = methodBuilder.GetILGenerator();
      if (!realMethod.IsStatic)
      {
        // Set 'this' to the result of JishProxy.GetInstance. This allows one 
        // class to proxy to methods from different source classes.
        SetReferenceToAppropriateThis(gen, thissIdx);
      } 
      for (int i = 0; i < parameters.Length; i++)
      {          
        if (IsParamsArray(realParams[i]))
        {            
          break;  // Break as this is the last parameter (params must always be last)
        } 
        // if (IsParamDelegate(realParams[i])) // TODO: This is in the wrong place
        // {
        // If the param is a delegate it needs to be replaced with a string which
        // will be used to find the 'real' delegate in the jish_internal scope.

        // }
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
      gen.Emit(realMethod.IsStatic ? OpCodes.Call : OpCodes.Callvirt, realMethod); 
      gen.Emit(OpCodes.Ret);
    }

    private string GetProxMethodName(MethodToProxify methodToProxify, MethodInfo realMethod)
    {
      string name = methodToProxify.OverrideMethodName ?? realMethod.Name;
      return IsValidDelegateMethod(methodToProxify) ? (name + "_internal") : name;
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

    private void SetReferenceToAppropriateThis(ILGenerator gen, int thissIdx)
    {
      Trace.Assert(gen != null);
      Trace.Assert(thissIdx >= 0);

      gen.Emit(OpCodes.Ldarg_0); // Load this argument onto stack
      gen.Emit(OpCodes.Ldc_I4, thissIdx); // Load the this index into the stack for GetInstance param
      gen.Emit(OpCodes.Callvirt, typeof (JishProxy).GetMethod("GetInstance")); // Call get instance and pop the current this pointer
    }

    private IList<Type[]> GetAllParameterCombinations(MethodInfo mi)
    {
      Trace.Assert(mi != null);

      IList<Type[]> combinations = new List<Type[]>();
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

    private Type[] GetParamCombination(ParameterInfo[] realParams, int until, Type extraParamType = null, int extraParams = 0)
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
        combo.Add(GetProxyParamType(pi));
      }
      for (int i = 0; i < extraParams; i++)
      {        
        combo.Add(extraParamType);
      }
      return combo.ToArray();
    }

    private Type GetProxyParamType(ParameterInfo pi)
    {
      Type realType = pi.ParameterType;      
      return IsParamDelegate(pi) ? typeof (string) : 
        realType.IsGenericParameter ? typeof(object) : realType;
    }
  }
}