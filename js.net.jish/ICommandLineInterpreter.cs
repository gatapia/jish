namespace js.net.jish
{
  public interface ICommandLineInterpreter
  {
    string ReadCommand();
    void ExecuteCommand(string command);
    void RunFile(string file);
  }
}