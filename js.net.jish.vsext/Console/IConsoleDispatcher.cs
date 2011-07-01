using System;

namespace js.net.jish.vsext.Console
{
  public interface IConsoleDispatcher
  {
    void Start();
    event EventHandler StartCompleted;
    bool IsStartCompleted { get; }
    bool IsExecutingCommand { get; }
    void ClearConsole();
  }
}