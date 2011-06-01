using System;

namespace js.net.jish.Command
{
  public class ClearCommand : EmptyCommand
  {
    public override string GetName()
    {
      return "clear";
    }

    public override string GetHelpDescription()
    {
      return "Break, and also clear the local context.";
    }

    public override void Execute(params string[] args)
    {
      JishEngine.ClearBufferedCommand();      
      Console.WriteLine("Clearing context...");
      JishEngine.ExecuteCommand(
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
