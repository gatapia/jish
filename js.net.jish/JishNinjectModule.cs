using js.net.Engine;
using js.net.jish.Util;
using Ninject.Modules;

namespace js.net.jish
{
  public class JishNinjectModule : NinjectModule {

    public override void Load()
    {
      Bind<IJishInterpreter>().To<JishInterpreter>().InSingletonScope();
      Bind<IEngine>().To<JSNetEngine>().InSingletonScope();
      Bind<JSConsole>().To<JSConsole>();      
      Bind<LoadedAssembliesBucket>().ToSelf().InSingletonScope();
    }
  }
}