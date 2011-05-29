namespace js.net.jish.Command
{
  public interface ICommand
  {
    IJishInterpreter JishEngine { get; set; }

    string GetName();
    string GetHelpDescription();
    void Execute(string input);
  }
}
