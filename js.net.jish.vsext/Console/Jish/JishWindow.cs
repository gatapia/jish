using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Shell;

namespace js.net.jish.vsext.Console.Jish
{
  [Export(typeof (IJishWindow))] internal class JishWindow : IJishWindow, IDisposable
  {
    public const string ContentType = "PackageConsole";

    private Dictionary<string, HostInfo> hostInfos;
    private HostInfo activeHostInfo;

    [Import(typeof (SVsServiceProvider))] internal IServiceProvider ServiceProvider { get; set; }

    [Import] internal IWpfConsoleService WpfConsoleService { get; set; }

    [ImportMany] private IEnumerable<Lazy<IHostProvider, IHostMetadata>> HostProviders { get; set; }

    private Dictionary<string, HostInfo> HostInfos
    {
      get
      {
        if (hostInfos == null)
        {
          hostInfos = new Dictionary<string, HostInfo>();
          foreach (Lazy<IHostProvider, IHostMetadata> p in HostProviders)
          {
            HostInfo info = new HostInfo(this, p);
            hostInfos[info.HostName] = info;
          }
        }
        return hostInfos;
      }
    }

    internal HostInfo ActiveHostInfo
    {
      get { return activeHostInfo ?? (activeHostInfo = HostInfos.Values.FirstOrDefault()); }
    }

    void IDisposable.Dispose()
    {
      if (hostInfos != null)
      {
        foreach (IDisposable hostInfo in hostInfos.Values)
        {
          if (hostInfo != null)
          {
            hostInfo.Dispose();
          }
        }
      }
    }
  }
}