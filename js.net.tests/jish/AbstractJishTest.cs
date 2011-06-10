using js.net.Engine;
using js.net.jish;
using js.net.jish.Util;
using Ninject;
using NUnit.Framework;

namespace js.net.tests.jish
{
  public abstract class AbstractJishTest
  {
    protected IJishInterpreter jish;
    protected TestingConsole console;

    [SetUp] public virtual void SetUp()
    {
      StandardKernel kernel = new StandardKernel();      
      kernel.Bind<LoadedAssembliesBucket>().ToSelf().InSingletonScope();
      IEngine engine = new JSNetEngine();
      kernel.Bind<IEngine>().ToConstant(engine);      
      console = new TestingConsole();      
      engine.SetGlobal("console", console);
      kernel.Bind<JSConsole>().ToConstant(console);
      kernel.Bind<IJishInterpreter>().To<JishInterpreter>().InSingletonScope().OnActivation(j2 =>
      {
        JishInterpreter j = (JishInterpreter) j2;
        j.ThrowErrors = true;
        j.Initialise();
      });
      jish = kernel.Get<IJishInterpreter>();
    }    
  }
}