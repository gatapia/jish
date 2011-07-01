using System.ComponentModel.Composition;

namespace js.net.jish.vsext.Console.Jish
{
  [Export(typeof (IHostProvider)), HostName("Jish.Host")] internal class JishProvider : IHostProvider
  {
    public IHost CreateHost()
    {
      VSConsole console = new VSConsole();
      return new JishHost(console, DefaultJishInterpreterFactory.CreateInterpreter(console));
    }
  }
}