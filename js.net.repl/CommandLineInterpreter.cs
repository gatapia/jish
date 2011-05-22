using System;
using System.Diagnostics;
using js.net.Engine;

namespace js.net.repl
{
  public class CommandLineInterpreter
  {    
    private readonly IEngine engine;
    private readonly JSConsole console;

    public CommandLineInterpreter(IEngine engine, JSConsole console)
    {
      Trace.Assert(engine != null);
      Trace.Assert(console != null);

      this.engine = engine;
      this.console = console;

      Console.WriteLine("Type '.help' for options.");
    }

    public bool ReadAndExecuteCommand()
    {
      Console.Write("js.net> ");
      string input = Console.ReadLine().Trim();
      if (input.Equals(".exit")) { return false; }
      try
      {
        object val = engine.Run(input);
        if (val != null) console.log(val);
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
      return true;
    }
  }
}