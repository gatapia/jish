using System.Collections.Generic;
using System.Reflection;
using js.net.jish.Command;

namespace js.net.jish
{
  public interface ICommandLineInterpreter
  {
    string ReadCommand();
    void ExecuteCommand(string command);
    void RunFile(string file, string[] args);
    void ClearBufferedCommand();
    IEnumerable<ICommand> GetCommands();
    void SetGlobal(string name, object valud);
    IDictionary<string, Assembly> GetLoadedAssemblies();
  }
}