namespace js.net.jish.ConsoleCommand
{
  public interface IConsoleCommand : ICommandBase
  {
    void Execute(params string[] args);
  }
}
