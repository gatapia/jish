using System;
using System.IO;
using js.net.Engine;

namespace js.net.repl
{
  public class REPL
  {
    private readonly IEngine engine;
    private readonly JSConsole console;

    public REPL(IEngine engine, JSConsole console)
    {
      this.engine = engine;
      this.console = console;
    }

    public void StartREPL()
    {   
      CommandLineInterpreter cli = new CommandLineInterpreter(engine, console);
      while (cli.ReadAndExecuteCommand()) {}
    }

    public void ExecuteArgs(string[] args)
    {
      string file = args[0];
      if (!File.Exists(file))
      {
        Console.WriteLine("Could not find " + file);
        return;
      }
      engine.Run(File.ReadAllText(file));
    }    
  }
}