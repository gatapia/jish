using System;
using System.IO;
using System.Linq;

namespace js.net.jish
{
  public class InputLoop
  {
    private readonly IJishInterpreter interpretter;

    public InputLoop(IJishInterpreter interpretter)
    {
      this.interpretter = interpretter;
    }

    public void StartInputLoop()
    {   
      while (true)
      {
        string command = interpretter.ReadCommand();
        interpretter.ExecuteCommand(command);
      }
    }

    public void ExecuteArgs(string[] args)
    {
      string file = args[0];
      if (!File.Exists(file))
      {
        Console.WriteLine("Could not find " + file);
        return;
      }
      interpretter.RunFile(file, args.Skip(1).ToArray());            
    }    
  }
}