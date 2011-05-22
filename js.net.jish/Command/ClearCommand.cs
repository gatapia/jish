using System;

namespace js.net.jish.Command
{
  public class ClearCommand : ICommand
  {
    private readonly ICommandLineInterpreter cli;
    private readonly JSConsole console;

    public ClearCommand(ICommandLineInterpreter cli, JSConsole console)
    {
      this.cli = cli;
      this.console = console;
    }

    public string GetName()
    {
      return "clear";
    }

    public string GetHelpDescription()
    {
      return "Break, and also clear the local context.";
    }

    public void Execute(string input)
    {
      cli.ClearBufferedCommand();      
      Console.WriteLine("Clearing context...");
      cli.ExecuteCommand(
        @"
for (var i in this) {
  if (i === 'console' || i === 'global') continue;
  delete this[i];
}
"
        );
    }
  }
}
