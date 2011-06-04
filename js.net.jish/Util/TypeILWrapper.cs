using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace js.net.jish.Util
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
      ModuleBuilder moduleBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(ns), AssemblyBuilderAccess.Run).DefineDynamicModule(ns); 
      TypeBuilder wrapperBuilder = moduleBuilder.DefineType(templateType.FullName, TypeAttributes.Public, typeof(JishProxy), new Type[0]);
      CreateProxyConstructor(wrapperBuilder);

      for (int i = 0; i < methods.Length; i++)
      {     
        CreateProxyMethod(wrapperBuilder, i, methods.ElementAt(i));
      }

      Type wrapperType = wrapperBuilder.CreateType();
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
      MethodInfo mi = method.MethodInfo;
      var parameters = mi.GetParameters();

      var methodBuilder = wrapperBuilder.DefineMethod(method.OverrideMethodName ?? mi.Name, MethodAttributes.Public | MethodAttributes.Virtual, mi.ReturnType, parameters.Select(p => p.ParameterType).ToArray());
      var gen = methodBuilder.GetILGenerator();      
      if (!mi.IsStatic)
      {
        gen.Emit(OpCodes.Ldarg_0); // this

        gen.Emit(OpCodes.Ldc_I4, thissIdx); 
        gen.Emit(OpCodes.Call, typeof(JishProxy).GetMethod("GetInstance")); // pop this
      }
      for (int i = 1; i < parameters.Length + 1; i++)
      {
        gen.Emit(OpCodes.Ldarg, i); 
      }      
      gen.Emit(mi.IsStatic ? OpCodes.Call : OpCodes.Callvirt, mi); 
      gen.Emit(OpCodes.Ret);
    }
  }

  public class ProxyMethod
  {    
    public ProxyMethod(MethodInfo mi, object methodContext)
    {
      MethodInfo = mi;
      MethodContext = methodContext;
    }

    public object MethodContext { get; private set; }
    public MethodInfo MethodInfo { get; private set; }
    public string OverrideMethodName { get; set; }
  }

  public class JishProxy
  {
    private readonly object[] thiss;

    public JishProxy(object[] thiss)
    {
      this.thiss = thiss;
    }

    public object GetInstance(int idx)
    {
      return thiss[idx];
    }
  }
}