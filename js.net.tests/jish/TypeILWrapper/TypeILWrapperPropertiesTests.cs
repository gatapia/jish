using System.Reflection;
using NUnit.Framework;

namespace js.net.tests.jish.TypeILWrapper
{
  [TestFixture] public class TypeILWrapperPropertiesTests
  {  
    private readonly net.jish.IL.TypeILWrapper typeIlWrapper = new net.jish.IL.TypeILWrapper();

    [Test] public void TestPropertyGetsCreated()
    {
      object wrapped = typeIlWrapper.CreateWrapperFromType(typeof(PropClz));

      Assert.IsNotNull(wrapped.GetType().GetProperty("PropInt"));
      Assert.IsNotNull(wrapped.GetType().GetProperty("PropString"));
    }

    [Test] public void TestGetAndSetOnly()
    {
      object wrapped = typeIlWrapper.CreateWrapperFromType(typeof(PropClz));

      PropertyInfo getOnly = wrapped.GetType().GetProperty("PropStringGetOnly");
      PropertyInfo setOnly = wrapped.GetType().GetProperty("PropStringSetOnly");
      
      Assert.IsTrue(getOnly.CanRead);
      Assert.IsFalse(getOnly.CanWrite);

      Assert.IsFalse(setOnly.CanRead);
      Assert.IsTrue(setOnly.CanWrite);
    }

    [Test] public void TestStandAloneGetSet()
    {
      object wrapped = typeIlWrapper.CreateWrapperFromType(typeof(PropClz));
      
      wrapped.GetType().GetProperty("PropInt").SetValue(wrapped, 999, null);
      wrapped.GetType().GetProperty("PropString").SetValue(wrapped, "stringval", null);

      Assert.AreEqual(999, wrapped.GetType().GetProperty("PropInt").GetValue(wrapped, null));
      Assert.AreEqual("stringval", wrapped.GetType().GetProperty("PropString").GetValue(wrapped, null));
    }

    [Test] public void TestPropertiesGetValue()
    {
      PropClz clz = new PropClz();
      object wrapped = typeIlWrapper.CreateWrapperFromInstance(clz);

      clz.PropInt = 10;
      clz.PropString = "100";

      Assert.AreEqual(10, wrapped.GetType().GetProperty("PropInt").GetValue(wrapped, null));
      Assert.AreEqual("100", wrapped.GetType().GetProperty("PropString").GetValue(wrapped, null));
    }

    [Test] public void TestPropertiesSetValue()
    {
      PropClz clz = new PropClz();
      object wrapped = typeIlWrapper.CreateWrapperFromInstance(clz);

      wrapped.GetType().GetProperty("PropInt").SetValue(wrapped, 999, null);
      wrapped.GetType().GetProperty("PropString").SetValue(wrapped, "stringval", null);

      Assert.AreEqual(999, clz.PropInt);
      Assert.AreEqual("stringval", clz.PropString);
    }
  }

  
  public class PropClz
  {
    public int PropInt { get; set; }
    public string PropString { get; set; }

    public string PropStringGetOnly { get; private set; }
    public string PropStringSetOnly { private get; set; }
  }
}
