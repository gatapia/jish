using System.IO;
using NUnit.Framework;

namespace js.net.tests.jish.Command
{
  [TestFixture]
  internal class CreateCommandTests : AbstractJishTest
  {
    [Test]
    public void TestCreateFileInfoSuccess()
    {
      const string file = @"..\\..\\..\\lib\\PicNet2.dll";
      jish.ExecuteCommand("var dllFile = jish.create('System.IO.FileInfo', '" + file + "');");
      jish.ExecuteCommand("console.log(dllFile.FullName);");
      Assert.AreEqual(new FileInfo(file).FullName, console.GetLastMessage());
    }

    [Test]
    public void TestCreateWithMultiplePossibleConstructors()
    {
      jish.ExecuteCommand(@"jish.assembly('js.net.tests.dll')");
      jish.ExecuteCommand("var test = jish.create('js.net.tests.jish.TestCreateTarget, js.net.tests', 'str1', 'str2');");
      jish.ExecuteCommand("console.log(test.GetConstructorType());");
      Assert.AreEqual("String[str1] and String[str2] Arg", console.GetLastMessage());
    }

    [Test]
    public void TestCreateWithMultipleconstructorArgs()
    {
      jish.ExecuteCommand(@"jish.assembly('js.net.tests.dll')");
      jish.ExecuteCommand("var test = jish.create('js.net.tests.jish.TestCreateTarget, js.net.tests', 'str', '1');");
      jish.ExecuteCommand("console.log(test.ConstructorType);");
      Assert.AreEqual("String[str] and Int[1] Arg", console.GetLastMessage());
    }

    [Test]
    public void TestMultipleInstances()
    {
      jish.ExecuteCommand(
        @"
jish.assembly('js.net.tests.dll');
var x1 = jish.create('js.net.tests.jish.TestCreateTarget, js.net.tests', '1', '1');
var x2 = jish.create('js.net.tests.jish.TestCreateTarget, js.net.tests', '2', '2');
console.log('x1 - ' + x1.GetConstructorType() + ' x2 - ' + x2.GetConstructorType());
");
      Assert.AreEqual("x1 - String[1] and Int[1] Arg x2 - String[2] and Int[2] Arg", console.GetLastMessage());
    }

    [Test]
    public void TestNoArgsConstructor()
    {
      jish.ExecuteCommand(@"jish.assembly('js.net.tests.dll')");
      jish.ExecuteCommand("var test = jish.create('js.net.tests.jish.TestCreateTarget, js.net.tests');");
      jish.ExecuteCommand("console.log(test.ConstructorType);");
      Assert.AreEqual("No Args", console.GetLastMessage());
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
      constructorType = "String[" + strArg1 + "] and String[" + strArg2 + "] Arg";
    }

    public string ConstructorType
    {
      get { return constructorType; }
    }

    public string GetConstructorType()
    {
      return constructorType;
    }
  }
}