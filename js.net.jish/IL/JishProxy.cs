using System.Linq;
using System.Reflection;

namespace js.net.jish.IL
{
  public class JishProxy
  {
    private readonly object[] thiss;
    private readonly MethodInfo[] realMethods;

    public JishProxy(object[] thiss, MethodInfo[] realMethods)
    {
      this.thiss = thiss;
      this.realMethods = realMethods;
    }

    public object GetInstance(int thisIdx)
    {
      return thiss[thisIdx];
    }    

    public T GetOptionalParameterDefaultValue<T>(int thisIdx, int optionalParamIdx)
    {
      MethodInfo mi = realMethods[thisIdx];
      return (T) mi.GetParameters()[optionalParamIdx].DefaultValue;
    }
  }
}