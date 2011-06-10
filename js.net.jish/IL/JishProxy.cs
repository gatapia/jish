using System.Reflection;

namespace js.net.jish.IL
{
  public class JishProxy
  {
    private readonly MethodToProxify[] methodsToProxify;

    public JishProxy(MethodToProxify[] methodsToProxify)
    {
      this.methodsToProxify = methodsToProxify;
    }

    public object GetInstance(int thisIdx)
    {
      return methodsToProxify[thisIdx].TargetInstance;
    }    

    public T GetOptionalParameterDefaultValue<T>(int thisIdx, int optionalParamIdx)
    {
      MethodInfo mi = methodsToProxify[thisIdx].RealMethod;
      return (T) mi.GetParameters()[optionalParamIdx].DefaultValue;
    }
  }
}