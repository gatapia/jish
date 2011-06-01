namespace js.net.jish.Command
{
  public class BreakCommand : EmptyCommand
  {
    public override string GetName()
    {
      return "break";
    }

    public override string GetHelpDescription()
    {
      return "Cancels the execution of a multi-line command.";
    }

    public override void Execute(params string[] args)
    {
      JishEngine.ClearBufferedCommand();
    }
  }
}
