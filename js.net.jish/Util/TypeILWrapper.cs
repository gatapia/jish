using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace js.net.jish.Util
{
  public class TypeILWrapper
  {
    public object CreateWrapper(Type staticType)
    {
      string ns = staticType.Assembly.FullName;      
      ModuleBuilder moduleBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(ns), AssemblyBuilderAccess.Run).DefineDynamicModule(ns); 
      TypeBuilder wrapperBuilder = moduleBuilder.DefineType(staticType.FullName, TypeAttributes.Public, null, new Type[0]);  
      foreach (MethodInfo method in staticType.GetMethods().Where(mi => !mi.Name.Equals("GetType")))
      {
        CreateProxyMethod(wrapperBuilder, method);
      }
      Type wrapperType = wrapperBuilder.CreateType();
      object instance = Activator.CreateInstance(wrapperType);
      return instance;
    }

    private void CreateProxyMethod(TypeBuilder wrapperBuilder, MethodInfo method)
    {
      var parameters = method.GetParameters();

      var methodBuilder = wrapperBuilder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, method.ReturnType, parameters.Select(p => p.ParameterType).ToArray());
      var gen = methodBuilder.GetILGenerator();
      
      for (int i = 1; i < parameters.Length + 1; i++)
      {
        gen.Emit(OpCodes.Ldarg, i); 
      }
      gen.Emit(OpCodes.Call, method); 
      gen.Emit(OpCodes.Ret);
    }
  }
}