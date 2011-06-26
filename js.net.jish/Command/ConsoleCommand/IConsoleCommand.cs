namespace js.net.jish.Command.ConsoleCommand
{
  public interface IConsoleCommand : ICommandBase
  {
    void Execute(params string[] args);
  }
}
