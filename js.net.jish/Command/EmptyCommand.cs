namespace js.net.jish.Command
{
  public abstract class EmptyCommand : ICommand
  {
    public IJishInterpreter JishEngine { get; set; }    

    public abstract string GetName();
    public abstract string GetHelpDescription();
    public abstract void Execute(string input);
  }
}
