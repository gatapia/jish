using System;
using js.net.Engine;
using js.net.jish.Util;
using Ninject;

namespace js.net.jish
{  
  public class MainRunner
  {    
    [STAThread] public static void Main(string[] args)
    {
      IKernel kernel = new StandardKernel(new NinjectSettings { UseReflectionBasedInjection = true });
      IEngine engine = new JSNetEngine();
      JSConsole console = new JSConsole();
      engine.SetGlobal("console", console);
      
      kernel.Bind<IEngine>().ToConstant(engine);
      kernel.Bind<JSConsole>().ToConstant(console);
      kernel.Bind<IJishInterpreter>().To<JishInterpreter>().InSingletonScope();
      kernel.Bind<LoadedAssembliesBucket>().ToSelf().InSingletonScope();
      
      InitialiseJishInterpreter(kernel);
      StartInterpreter(kernel, args, console);
    }
   
    private static void InitialiseJishInterpreter(IKernel kernel)
    {
      IJishInterpreter jish = kernel.Get<IJishInterpreter>();
      jish.InitialiseDependencies();
      jish.InitialiseInputConsole();
    }

    private static void StartInterpreter(IKernel kernel, string[] args, JSConsole console)
    {
      InputLoop inputLoop = kernel.Get<InputLoop>();
      if (args == null || args.Length == 0)
      {
        console.log("Welcome to Jish. Type '.help' for more options.");

        inputLoop.StartInputLoop();
      }
      else
      {
        inputLoop.ExecuteArgs(args);
      }
    }

  }
}