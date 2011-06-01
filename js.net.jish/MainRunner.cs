using System;
using Ninject;

namespace js.net.jish
{  
  public class MainRunner
  {
    [STAThread] public static void Main(string[] args)
    {
      IKernel kernel = new StandardKernel(new JishNinjectModule());

      IJishInterpreter jish = kernel.Get<IJishInterpreter>();
      jish.InitialiseDependencies();
      jish.InitialiseInputConsole();
      
      InputLoop inputLoop = kernel.Get<InputLoop>();
      if (args == null || args.Length == 0)
      {
        inputLoop.StartInputLoop();
      }
      else
      {
        inputLoop.ExecuteArgs(args);
      }
    }
  }
}