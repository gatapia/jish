using System.IO;
using NUnit.Framework;

namespace js.net.tests.jish
{
  [TestFixture] class CreateCommandTests : AbstractJishTest 
  {
    [Test] public void TestCreateFileInfoSuccess()
    {
      const string file = @"..\..\..\lib\PicNet2.dll";
      cli.ExecuteCommand(".create(System.IO.FileInfo, '" + file + "', 'dllFile')");
      cli.ExecuteCommand("console.log(dllFile.FullName);");
      Assert.AreEqual(new FileInfo(file).FullName, console.GetLastMessage());
    }

    [Test] public void TestNoArgsConstructor()
    {
      cli.ExecuteCommand(@".load(js.net.tests.dll)");      
      cli.ExecuteCommand(".create('js.net.tests.jish.TestCreateTarget, js.net.tests', 'test')");
      cli.ExecuteCommand("console.log(test.ConstructorType);");
      Assert.AreEqual("No Args", console.GetLastMessage());
    }

    [Test] public void TestCreateWithMultipleconstructorArgs()
    {
      cli.ExecuteCommand(@".load(js.net.tests.dll)");      
      cli.ExecuteCommand(".create('js.net.tests.jish.TestCreateTarget, js.net.tests', 'str', '1', 'test')");
      cli.ExecuteCommand("console.log(test.ConstructorType);");
      Assert.AreEqual("String[str] and Int[1] Arg", console.GetLastMessage());
    }

    [Test] public void TestCreateWithMultiplePossibleConstructors()
    {
      cli.ExecuteCommand(@".load(js.net.tests.dll)");      
      cli.ExecuteCommand(".create('js.net.tests.jish.TestCreateTarget, js.net.tests', 'str1', 'str2', 'test')");
      cli.ExecuteCommand("console.log(test.GetConstructorType());");
      Assert.AreEqual("String[str1] and String[str2] Arg", console.GetLastMessage());
    }
  }

  public class TestCreateTarget
  {
    private readonly string constructorType;

    public TestCreateTarget()
    {
      constructorType = "No Args";
    }

    public TestCreateTarget(string strArg1, int intArg2)
    {
      constructorType = "String[" + strArg1 + "] and Int[" + intArg2 + "] Arg";
    }

    public TestCreateTarget(string strArg1, string strArg2)
    {
      constructorType = "String[" + strArg1 +"] and String[" + strArg2 +"] Arg";
    }

    public string ConstructorType { get { return constructorType; }}
    public string GetConstructorType()
    {
      return constructorType;
    }
  }
}
