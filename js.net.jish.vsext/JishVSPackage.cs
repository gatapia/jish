using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using js.net.jish.vsext.Console;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace js.net.jish.vsext
{  
  [PackageRegistration(UseManagedResourcesOnly = true), 
   InstalledProductRegistration("Jish - The JavaScript Interactive Shell", "Jish - The JavaScript Interactive Shell", "0.0.1", IconResourceID = 400), 
   ProvideMenuResource("Menus.ctmenu", 1),    
   ProvideToolWindow(typeof (JishToolWindow), Style = 
     VsDockStyle.Tabbed, 
     // this is the guid of the Output tool window, which is present in both VS and VWD
     Window = "{34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3}",
     Orientation = ToolWindowOrientation.Right), 
   ProvideBindingPath, 
   Guid("796ee83e-2e4a-419e-9e79-69c2ab2294b3")] // Definition dll needs to be on VS binding path
  public sealed class JishVSPackage : Package
  {    
    private void ShowToolWindow(object sender, EventArgs e)
    {
      // Get the instance number 0 of this tool window. This window is single instance so this instance
      // is actually the only one.
      // The last flag is set to true so that if the tool window does not exists it will be created.
      ToolWindowPane window = FindToolWindow(typeof (JishToolWindow), 0, true);
      if (window == null || window.Frame == null) { throw new NotSupportedException("Could not create or find the Jish console window."); }
      IVsWindowFrame windowFrame = (IVsWindowFrame) window.Frame;
      ErrorHandler.ThrowOnFailure(windowFrame.Show());
    }

    protected override void Initialize()
    {
      base.Initialize();

      OleMenuCommandService mcs = GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
      if (null != mcs)
      {
        CommandID toolwndCommandID = new CommandID(new Guid("298cdc02-f910-447c-881e-cbf0ea7df11a"), 0x0100);
        MenuCommand menuToolWin = new MenuCommand(ShowToolWindow, toolwndCommandID);
        mcs.AddCommand(menuToolWin);
      }
    }
  }
}