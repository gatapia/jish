using System;
using System.Diagnostics;
using System.IO;
<<<<<<< HEAD
using System.Reflection;
=======
using System.Linq;
>>>>>>> Major refactorings
using js.net.Engine;
using js.net.jish.Util;
using js.net.Util;
using Ninject;

namespace js.net.jish
{  
  public class MainRunner
  {    
<<<<<<< HEAD
    static MainRunner()
    {
      AppDomain.CurrentDomain.AssemblyResolve += EmbeddedAssemblyLoader.OnAssemblyResolve;
    } 
    
    [STAThread] public static void Main(string[] args)
    {      
=======
    [STAThread] public static void Main(string[] args) { new MainRunner(args); }

    private MainRunner(string[] args) {
>>>>>>> Major refactorings
      IKernel kernel = new StandardKernel(new NinjectSettings { UseReflectionBasedInjection = true });
      IEngine engine = new JSNetEngine();
      JSConsole console = new JSConsole();
      engine.SetGlobal("console", console);
      
      kernel.Bind<IEngine>().ToConstant(engine);
      kernel.Bind<JSConsole>().ToConstant(console);
      kernel.Bind<IJishInterpreter>().To<JishInterpreter>().InSingletonScope();
      kernel.Bind<LoadedAssembliesBucket>().ToSelf().InSingletonScope();

      IJishInterpreter interpreter = kernel.Get<IJishInterpreter>();

      if (args == null || args.Length == 0)
      {
        StartInputLoop(console, interpreter);
      } else
      {
        ExecuteArgs(args, interpreter);
      }
    }

    private void StartInputLoop(JSConsole console, IJishInterpreter interpreter) 
    { 
      console.log("Welcome to Jish. Type '.help' for more options.");
      while (true)
      {
        string command = interpreter.ReadCommand();
        interpreter.ExecuteCommand(command);
      }
    }

    private void ExecuteArgs(string[] args, IJishInterpreter interpreter)
    {
      Trace.Assert(args != null);
      Trace.Assert(args.Any());

      string file = args[0];
      if (!File.Exists(file))
      {
        Console.WriteLine("Could not find " + file);
        return;
      }
      interpreter.RunFile(file, args.Skip(1).ToArray());            
    }  

  }
}