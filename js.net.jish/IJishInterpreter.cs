using js.net.Engine;

namespace js.net.jish
{
  public interface IJishInterpreter
  {
    string ReadCommand();
    void ExecuteCommand(string command);
    void RunFile(string file, string[] args = null);
    void ClearBufferedCommand();
    IEngine GetEngine();
  }
}