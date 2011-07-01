using Microsoft.VisualStudio.Text;

namespace js.net.jish.vsext.Console
{
  internal interface IPrivateWpfConsole : IWpfConsole
  {
    SnapshotPoint? InputLineStart { get; }
    InputHistory InputHistory { get; }
    void BeginInputLine();
  }
}