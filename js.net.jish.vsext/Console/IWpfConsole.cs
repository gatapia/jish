namespace js.net.jish.vsext.Console
{
  public interface IWpfConsole : IConsole
  {
    object Content { get; }
    void SetExecutionMode(bool isExecuting);
    object VsTextView { get; }
  }
}