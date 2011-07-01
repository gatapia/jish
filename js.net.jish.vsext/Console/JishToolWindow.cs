using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using js.net.jish.vsext.Console.Jish;
using js.net.jish.vsext.Console.Utils;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace js.net.jish.vsext.Console
{
  [Guid("0AD07096-BBA9-4900-A651-0598D26F6D24")] public sealed class JishToolWindow : ToolWindowPane, IOleCommandTarget
  {
    private static readonly Guid guidNuGetConsoleCmdSet = new Guid("298cdc02-f910-447c-881e-cbf0ea7df11a");

    private IComponentModel ComponentModel { get { return this.GetService<IComponentModel>(typeof (SComponentModel)); } }
    
    private JishWindow JishWindow { get { return ComponentModel.GetService<IJishWindow>() as JishWindow; } }

    private IVsUIShell VsUIShell { get { return this.GetService<IVsUIShell>(typeof (SVsUIShell)); } }

    private bool IsToolbarEnabled { get { return wpfConsole != null && wpfConsole.Dispatcher.IsStartCompleted && wpfConsole.Host != null && wpfConsole.Host.IsCommandEnabled; } }

    public JishToolWindow() : base(null)
    {
      Caption = "Jish - JavaScript Interactive Shell";
      BitmapResourceID = 301;
      BitmapIndex = 0;
      ToolBar = new CommandID(guidNuGetConsoleCmdSet, 0x1010);
    }

    protected override void Initialize()
    {
      base.Initialize();

      OleMenuCommandService mcs = GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
      if (mcs != null)
      {
        // clear console command
        CommandID clearHostCommandID = new CommandID(guidNuGetConsoleCmdSet, 0x0300);
        mcs.AddCommand(new OleMenuCommand(ClearHost_Exec, clearHostCommandID));
      }
    }

    public override void OnToolWindowCreated()
    {
      // Register key bindings to use in the editor
      var windowFrame = (IVsWindowFrame) Frame;
      Guid cmdUi = VSConstants.GUID_TextEditorFactory;
      windowFrame.SetGuidProperty((int) __VSFPROPID.VSFPROPID_InheritKeyBindings, ref cmdUi);

      // pause for a tiny moment to let the tool window open before initializing the host
      var timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(0)};
      timer.Tick += (o, e) =>
      {
        timer.Stop();
        LoadConsoleEditor();
      };
      timer.Start();

      base.OnToolWindowCreated();
    }

    protected override bool PreProcessMessage(ref Message m)
    {
      IVsWindowPane vsWindowPane = VsTextView as IVsWindowPane;
      if (vsWindowPane != null)
      {
        MSG[] pMsg = new MSG[1];
        pMsg[0].hwnd = m.HWnd;
        pMsg[0].message = (uint) m.Msg;
        pMsg[0].wParam = m.WParam;
        pMsg[0].lParam = m.LParam;

        return vsWindowPane.TranslateAccelerator(pMsg) == 0;
      }

      return base.PreProcessMessage(ref m);
    }

    int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
    {
      // examine buttons within our toolbar
      if (pguidCmdGroup == guidNuGetConsoleCmdSet)
      {
        prgCmds[0].cmdf = (uint) (IsToolbarEnabled ? (OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED) : OLECMDF.OLECMDF_SUPPORTED);
        return VSConstants.S_OK;
      }

      int hr = OleCommandFilter.OLECMDERR_E_NOTSUPPORTED;

      if (VsTextView != null)
      {
        IOleCommandTarget cmdTarget = (IOleCommandTarget) VsTextView;
        hr = cmdTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
      }

      if (hr == OleCommandFilter.OLECMDERR_E_NOTSUPPORTED)
      {
        IOleCommandTarget target = GetService(typeof (IOleCommandTarget)) as IOleCommandTarget;
        if (target != null)
        {
          hr = target.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
      }

      return hr;
    }

    int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
    {
      int hr = OleCommandFilter.OLECMDERR_E_NOTSUPPORTED;

      if (VsTextView != null)
      {
        IOleCommandTarget cmdTarget = (IOleCommandTarget) VsTextView;
        hr = cmdTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
      }

      if (hr == OleCommandFilter.OLECMDERR_E_NOTSUPPORTED)
      {
        IOleCommandTarget target = GetService(typeof (IOleCommandTarget)) as IOleCommandTarget;
        if (target != null)
        {
          hr = target.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }
      }

      return hr;
    }

    private void ClearHost_Exec(object sender, EventArgs e)
    {
      if (WpfConsole != null)
      {
        WpfConsole.Dispatcher.ClearConsole();
      }
    }

    private HostInfo ActiveHostInfo { get { return JishWindow.ActiveHostInfo; } }

    private void LoadConsoleEditor()
    {
      if (WpfConsole != null)
      {
        FrameworkElement consolePane = WpfConsole.Content as FrameworkElement;
        ConsoleParentPane.AddConsoleEditor(consolePane);

        // WPF doesn't handle input focus automatically in this scenario. We
        // have to set the focus manually, otherwise the editor is displayed but
        // not focused and not receiving keyboard inputs until clicked.
        if (consolePane != null)
        {
          PendingMoveFocus(consolePane);
        }
      }
    }

    private void PendingMoveFocus(FrameworkElement consolePane)
    {
      if (consolePane.IsLoaded && consolePane.IsConnectedToPresentationSource())
      {
        PendingFocusPane = null;
        MoveFocus(consolePane);
      } else
      {
        PendingFocusPane = consolePane;
      }
    }

    private FrameworkElement pendingFocusPane;
    private FrameworkElement PendingFocusPane
    {
      get { return pendingFocusPane; }
      set
      {
        if (pendingFocusPane != null)
        {
          pendingFocusPane.Loaded -= PendingFocusPane_Loaded;
        }
        pendingFocusPane = value;
        if (pendingFocusPane != null)
        {
          pendingFocusPane.Loaded += PendingFocusPane_Loaded;
        }
      }
    }

    private void PendingFocusPane_Loaded(object sender, RoutedEventArgs e)
    {
      MoveFocus(PendingFocusPane);
      PendingFocusPane = null;
    }

    private void MoveFocus(FrameworkElement consolePane)
    {
      // TAB focus into editor (consolePane.Focus() does not work due to editor layouts)
      consolePane.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

      // Try start the console session now. This needs to be after the console
      // pane getting focus to avoid incorrect initial editor layout.
      StartConsoleSession(consolePane);
    }

    private void StartConsoleSession(FrameworkElement consolePane)
    {
      if (WpfConsole != null && WpfConsole.Content == consolePane && WpfConsole.Host != null)
      {
        try
        {
          if (WpfConsole.Dispatcher.IsStartCompleted)
          {
            OnDispatcherStartCompleted();
          } else
          {
            WpfConsole.Dispatcher.StartCompleted += (sender, args) => OnDispatcherStartCompleted();
            WpfConsole.Dispatcher.Start();
          }
        } catch (Exception x)
        {
          // hide the text "initialize host" when an error occurs.
          ConsoleParentPane.NotifyInitializationCompleted();

          WpfConsole.WriteLine(x.ToString());
        }
      }
    }

    private void OnDispatcherStartCompleted()
    {
      ConsoleParentPane.NotifyInitializationCompleted();

      // force the UI to update the toolbar
      VsUIShell.UpdateCommandUI(0 /* false = update UI asynchronously */);
    }

    private IWpfConsole wpfConsole;

    private IWpfConsole WpfConsole
    {
      get
      {
        if (wpfConsole == null)
        {
          try
          {
            wpfConsole = ActiveHostInfo.WpfConsole;
          } catch (Exception x)
          {
            wpfConsole = ActiveHostInfo.WpfConsole;
            wpfConsole.Write(x.ToString());
          }
        }

        return wpfConsole;
      }
    }

    private IVsTextView vsTextView;

    private IVsTextView VsTextView
    {
      get
      {
        if (vsTextView == null && wpfConsole != null)
        {
          vsTextView = (IVsTextView) (WpfConsole.VsTextView);
        }
        return vsTextView;
      }
    }

    private ConsoleContainer consoleParentPane;

    private ConsoleContainer ConsoleParentPane
    {
      get { return consoleParentPane ?? (consoleParentPane = new ConsoleContainer()); }
    }

    public override object Content { get { return ConsoleParentPane; } set { base.Content = value; } }
  }
}