using System;
using js.net.jish.vsext.Console.Utils;

namespace js.net.jish.vsext.Console.Jish
{
  internal class HostInfo : ObjectWithFactory<JishWindow>
  {
    private Lazy<IHostProvider, IHostMetadata> HostProvider { get; set; }

    public HostInfo(JishWindow factory, Lazy<IHostProvider, IHostMetadata> hostProvider) : base(factory)
    {
      if (hostProvider == null)
      {
        throw new ArgumentNullException("hostProvider");
      }
      HostProvider = hostProvider;
    }

    public string HostName { get { return HostProvider.Metadata.HostName; } }

    private IWpfConsole wpfConsole;
    public IWpfConsole WpfConsole
    {
      get
      {
        if (wpfConsole == null)
        {
          wpfConsole = Factory.WpfConsoleService.CreateConsole(Factory.ServiceProvider, JishWindow.ContentType);
          wpfConsole.Host = HostProvider.Value.CreateHost();
        }
        return wpfConsole;
      }
    }   
  }
}