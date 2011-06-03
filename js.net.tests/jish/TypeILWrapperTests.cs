using js.net.jish.Util;
using NUnit.Framework;

namespace js.net.tests.jish
{
  [TestFixture] public class TypeILWrapperTests
  {  
    private readonly TypeILWrapper wrapper = new TypeILWrapper();
    
    [Test] public void TestStaticTypeWithSimpleArgs()
    {
      object wrapped = wrapper.CreateWrapper(typeof(StaticWithSimpleArgs));
      Assert.AreEqual("STRING[123]", wrapped.GetType().GetMethod("GetString").Invoke(wrapped, new object[] {123}));
    }
  }

  public static class StaticWithSimpleArgs
  {
    public static string GetString(int arg1)
    {
      return "STRING[" + arg1 +"]";
    }
  }
}
