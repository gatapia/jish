using js.net.Util;

namespace js.net.jish.vsext.Console.Jish
{
  public class VSConsole : JSConsole
  {
    public IConsole Console { private get; set; }

    protected override void logImpl(string message, bool newline)
    {
      if (newline)
      {
        Console.WriteLine(message);
      } else
      {
        Console.Write(message);
      }
    }
  }
}