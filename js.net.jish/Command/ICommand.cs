namespace js.net.jish.Command
{
  public interface ICommand : ICommandBase
  {
    void Execute(params string[] args);
  }
}
