namespace js.net.jish.Command
{
  public interface ICommand
  {
    IJishInterpreter JishEngine { get; set; }

    string GetName();
    string GetHelpDescription();
    
    string ValidateArgumentsBeforeExecute(params string[] args);
    void Execute(params string[] args);
  }
}
