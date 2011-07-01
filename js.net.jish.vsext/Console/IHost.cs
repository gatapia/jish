namespace js.net.jish.vsext.Console
{
  public interface IHost
  {
    bool IsCommandEnabled { get; }
    void Initialize(IConsole console);
    string Prompt { get; }
    bool Execute(IConsole console, string command);
    void Abort();
  }
}