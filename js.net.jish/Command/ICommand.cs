namespace js.net.jish.Command
{
  public interface ICommand
  {
    ICommandLineInterpreter JishEngine { get; set; }
    JSConsole JavaScriptConsole { get; set; }

    string GetName();
    string GetHelpDescription();
    void Execute(string input);
  }
}
