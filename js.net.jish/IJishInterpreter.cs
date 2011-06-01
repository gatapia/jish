using System.Collections.Generic;
using js.net.jish.Command;
using Ninject;

namespace js.net.jish
{
  public interface IJishInterpreter
  {
    string ReadCommand();
    void ExecuteCommand(string command);
    void RunFile(string file, string[] args = null);
    void ClearBufferedCommand();
    IEnumerable<ICommand> GetCommands();
    void SetGlobal(string name, object valud);    
    void InitialiseDependencies(IKernel kernel);
    void InitialiseInputConsole();
    bool ThrowErrors { get; set; }
  }
}