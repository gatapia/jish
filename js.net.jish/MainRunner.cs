using System;
using System.Diagnostics;
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
      kernel.Bind<IJishInterpreter>().To<JishInterpreter>().InSingletonScope().OnActivation(jish => ((JishInterpreter)jish).Initialise());
      kernel.Bind<LoadedAssembliesBucket>().ToSelf().InSingletonScope();
      
      StartInterpreter(kernel.Get<InputLoop>(), args, console);
    }   

    private static void StartInterpreter(InputLoop inputLoop, string[] args, JSConsole console)
    {
      Trace.Assert(inputLoop != null);
      Trace.Assert(console != null);

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