namespace js.net.jish.vsext.Console
{
  public interface IConsole
  {
    IHost Host { get; set; }
    IConsoleDispatcher Dispatcher { get; }
    void Write(string text);
    void WriteLine(string text);
    void Clear();
  }
}