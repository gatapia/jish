using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace js.net.jish.vsext.Console
{
  internal class OleCommandFilter : IOleCommandTarget
  {
    public const int OLECMDERR_E_NOTSUPPORTED = (int) Constants.OLECMDERR_E_NOTSUPPORTED;

    protected IOleCommandTarget OldChain { get; private set; }

    protected OleCommandFilter(IVsTextView vsTextView)
    {
      IOleCommandTarget oldChain;
      ErrorHandler.ThrowOnFailure(vsTextView.AddCommandFilter(this, out oldChain));

      OldChain = oldChain;
    }

    protected virtual int InternalQueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText) { return OLECMDERR_E_NOTSUPPORTED; }

    protected virtual int InternalExec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) { return OLECMDERR_E_NOTSUPPORTED; }

    public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
    {
      int hr = InternalQueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);

      if (hr == OLECMDERR_E_NOTSUPPORTED)
      {
        hr = OldChain.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
      }

      return hr;
    }

    public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
    {
      int hr = InternalExec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

      if (hr == OLECMDERR_E_NOTSUPPORTED)
      {
        hr = OldChain.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
      }

      return hr;
    }
  }
}