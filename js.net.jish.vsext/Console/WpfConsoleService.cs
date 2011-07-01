using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace js.net.jish.vsext.Console
{
  [Export(typeof (IWpfConsoleService))] internal class WpfConsoleService : IWpfConsoleService
  {
    [Import] internal IContentTypeRegistryService ContentTypeRegistryService { get; set; }

    [Import] internal IVsEditorAdaptersFactoryService VsEditorAdaptersFactoryService { get; set; }

    [Import] internal IEditorOptionsFactoryService EditorOptionsFactoryService { get; set; }

    public IWpfConsole CreateConsole(IServiceProvider sp, string contentTypeName) { return new WpfConsole(this, sp, contentTypeName).MarshaledConsole; }
  }
}