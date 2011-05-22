namespace js.net.repl
{
  public interface ICommandLineInterpreter
  {
    string ReadCommand();
    void ExecuteCommand(string command);
    void RunFile(string file);
  }
}