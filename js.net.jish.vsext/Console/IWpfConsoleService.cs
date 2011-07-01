using System;

namespace js.net.jish.vsext.Console
{
  public interface IWpfConsoleService
  {
    IWpfConsole CreateConsole(IServiceProvider sp, string contentTypeName);
  }
}