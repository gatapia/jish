using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using js.net.jish.Util;
using js.net.Util;

namespace js.net.jish
{  
  public class MainRunner
  {            
    static MainRunner()
    {
      AppDomain.CurrentDomain.AssemblyResolve += EmbeddedAssemblyLoader.OnAssemblyResolve;
    }

    [STAThread] public static void Main(string[] args) { new MainRunner(args); }

    public MainRunner(string[] args)
    {
      IJishInterpreter interpreter = DefaultJishInterpreterFactory.CreateInterpreter(new JSConsole());

      if (args == null || args.Length == 0)
      {
        StartInputLoop(interpreter);
      } else
      {
        ExecuteArgs(args, interpreter);
      }
    }

    private void StartInputLoop(IJishInterpreter interpreter) 
    { 
      Console.WriteLine("Welcome to Jish. Type '.help' for more options.");
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