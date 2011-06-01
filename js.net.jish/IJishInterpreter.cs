using System;
using System.Collections.Generic;
using js.net.jish.Command;

namespace js.net.jish
{
  public interface IJishInterpreter
  {
    string ReadCommand();
    void ExecuteCommand(string command);
    void RunFile(string file, string[] args = null);
    void ClearBufferedCommand();
    void SetGlobal(string name, object valud);    
    void InitialiseDependencies();
    void InitialiseInputConsole();
    bool ThrowErrors { get; set; }
    IEnumerable<ICommand> GetAllActiveCommands();
  }
}