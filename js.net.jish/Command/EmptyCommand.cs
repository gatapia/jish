namespace js.net.jish.Command
{
  public abstract class EmptyCommand : ICommand
  {
    public ICommandLineInterpreter JishEngine { get; set; }
    public JSConsole JavaScriptConsole { get; set; }

    public abstract string GetName();
    public abstract string GetHelpDescription();
    public abstract void Execute(string input);
  }
}
