using js.net.Engine;
using js.net.jish;
using NUnit.Framework;

namespace js.net.tests.jish
{
  public abstract class AbstractJishTest
  {
    private IEngine engine;
    protected IJishInterpreter jish;
    protected TestingConsole console;

    [SetUp] public virtual void SetUp()
    {
      engine = new JSNetEngine();
      console = new TestingConsole();      
      engine.SetGlobal("console", console);
      jish = new JishInterpreter(engine, console);
    }

    [TearDown] public virtual void TearDown()
    {
      engine.Dispose();
    }
  }
}