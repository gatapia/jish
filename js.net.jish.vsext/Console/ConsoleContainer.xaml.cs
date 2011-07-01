using System.Windows;
using System.Windows.Controls;

namespace js.net.jish.vsext.Console
{
  public partial class ConsoleContainer : UserControl
  {
    public ConsoleContainer() { InitializeComponent(); }

    public void AddConsoleEditor(UIElement content)
    {
      Grid.SetRow(content, 1);
      RootLayout.Children.Add(content);
    }

    public void NotifyInitializationCompleted() { RootLayout.Children.Remove(InitializeText); }
  }
}