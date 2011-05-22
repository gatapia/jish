namespace js.net.jish.Command
{
  public interface ICommand
  {
    string GetName();
    string GetHelpDescription();
    void Execute(string input);
  }
}
