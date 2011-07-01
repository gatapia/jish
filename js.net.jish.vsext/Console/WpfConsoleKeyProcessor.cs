using System;
using System.Linq;
using System.Windows;
using js.net.jish.vsext.Console.Utils;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace js.net.jish.vsext.Console
{
  internal class WpfConsoleKeyProcessor : OleCommandFilter
  {
    private WpfConsole WpfConsole { get; set; }
    private IWpfTextView WpfTextView { get; set; }

    public WpfConsoleKeyProcessor(WpfConsole wpfConsole) : base(wpfConsole.VsTextView)
    {
      WpfConsole = wpfConsole;
      WpfTextView = wpfConsole.WpfTextView;
    }

    private bool IsCaretInReadOnlyRegion
    {
      get
      {
        return WpfConsole.InputLineStart == null || // shortcut -- no inut allowed
          WpfTextView.TextBuffer.IsReadOnly(WpfTextView.Caret.Position.BufferPosition.Position);
      }
    }

    private bool IsCaretOnInputLine
    {
      get
      {
        SnapshotPoint? inputStart = WpfConsole.InputLineStart;
        if (inputStart != null)
        {
          SnapshotSpan inputExtent = inputStart.Value.GetContainingLine().ExtentIncludingLineBreak;
          SnapshotPoint caretPos = CaretPosition;
          return inputExtent.Contains(caretPos) || inputExtent.End == caretPos;
        }

        return false;
      }
    }

    private bool IsCaretAtInputLineStart { get { return WpfConsole.InputLineStart == WpfTextView.Caret.Position.BufferPosition; } }

    private SnapshotPoint CaretPosition { get { return WpfTextView.Caret.Position.BufferPosition; } }

    private bool IsSelectionReadonly
    {
      get
      {
        if (!WpfTextView.Selection.IsEmpty)
        {
          ITextBuffer buffer = WpfTextView.TextBuffer;
          return WpfTextView.Selection.SelectedSpans.Any(span => buffer.IsReadOnly(span));
        }
        return false;
      }
    }

    private void ExecuteCommand(VSConstants.VSStd2KCmdID idCommand, object args = null) { OldChain.Execute(idCommand, args); }

    protected override int InternalExec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
    {
      int hr = OLECMDERR_E_NOTSUPPORTED;

      if (WpfConsole.Host == null)
      {
        return hr;
      }

      if (!WpfConsole.Host.IsCommandEnabled)
      {
        return hr;
      }

      // if the console has not been successfully started, do not accept any key inputs, unless
      // we are in the middle of a ReadKey call. This happens when the execution group policy setting
      // is set to AllSigned, and PS is asking user to trust the certificate.
      if (!WpfConsole.Dispatcher.IsStartCompleted)
      {
        return hr;
      }

      // if the console is in the middle of executing a command, do not accept any key inputs unless
      // we are in the middle of a ReadKey call. 
      if (WpfConsole.Dispatcher.IsExecutingCommand)
      {
        return hr;
      }

      if (pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97)
      {
        //Debug.Print("Exec: GUID_VSStandardCommandSet97: {0}", (VSConstants.VSStd97CmdID)nCmdID);

        switch ((VSConstants.VSStd97CmdID) nCmdID)
        {
          case VSConstants.VSStd97CmdID.Paste:
            if (IsCaretInReadOnlyRegion || IsSelectionReadonly)
            {
              hr = VSConstants.S_OK; // eat it
            } else
            {
              PasteText(ref hr);
            }
            break;
        }
      } else if (pguidCmdGroup == VSConstants.VSStd2K)
      {
        switch ((VSConstants.VSStd2KCmdID) nCmdID)
        {
          case VSConstants.VSStd2KCmdID.TYPECHAR:
            if (IsSelectionReadonly)
            {
              WpfTextView.Selection.Clear();
            }
            if (IsCaretInReadOnlyRegion)
            {
              WpfTextView.Caret.MoveTo(WpfConsole.InputLineExtent.End);
            }
            break;

          case VSConstants.VSStd2KCmdID.LEFT:
          case VSConstants.VSStd2KCmdID.LEFT_EXT:
          case VSConstants.VSStd2KCmdID.LEFT_EXT_COL:
          case VSConstants.VSStd2KCmdID.WORDPREV:
          case VSConstants.VSStd2KCmdID.WORDPREV_EXT:
          case VSConstants.VSStd2KCmdID.WORDPREV_EXT_COL:
            if (IsCaretAtInputLineStart)
            {
              //
              // Note: This simple implementation depends on Prompt containing a trailing space.
              // When caret is on the right of InputLineStart, editor will handle it correctly,
              // and caret won't move left to InputLineStart because of the trailing space.
              //
              hr = VSConstants.S_OK; // eat it
            }
            break;

          case VSConstants.VSStd2KCmdID.BOL:
          case VSConstants.VSStd2KCmdID.BOL_EXT:
          case VSConstants.VSStd2KCmdID.BOL_EXT_COL:
            if (IsCaretOnInputLine)
            {
              VirtualSnapshotPoint oldCaretPoint = WpfTextView.Caret.Position.VirtualBufferPosition;

              WpfTextView.Caret.MoveTo(WpfConsole.InputLineStart.Value);
              WpfTextView.Caret.EnsureVisible();

              if ((VSConstants.VSStd2KCmdID) nCmdID == VSConstants.VSStd2KCmdID.BOL)
              {
                WpfTextView.Selection.Clear();
              } else if ((VSConstants.VSStd2KCmdID) nCmdID != VSConstants.VSStd2KCmdID.BOL) // extend selection
              {
                VirtualSnapshotPoint anchorPoint = WpfTextView.Selection.IsEmpty ? oldCaretPoint.TranslateTo(WpfTextView.TextSnapshot) : WpfTextView.Selection.AnchorPoint;
                WpfTextView.Selection.Select(anchorPoint, WpfTextView.Caret.Position.VirtualBufferPosition);
              }

              hr = VSConstants.S_OK;
            }
            break;

          case VSConstants.VSStd2KCmdID.UP:
            if (IsCaretInReadOnlyRegion)
            {
              ExecuteCommand(VSConstants.VSStd2KCmdID.END);
            }
            WpfConsole.NavigateHistory(-1);
            hr = VSConstants.S_OK;
            break;

          case VSConstants.VSStd2KCmdID.DOWN:
            if (IsCaretInReadOnlyRegion)
            {
              ExecuteCommand(VSConstants.VSStd2KCmdID.END);
            }
            WpfConsole.NavigateHistory(+1);
            hr = VSConstants.S_OK;
            break;

          case VSConstants.VSStd2KCmdID.RETURN:
            if (IsCaretOnInputLine || !IsCaretInReadOnlyRegion)
            {
              ExecuteCommand(VSConstants.VSStd2KCmdID.END);
              ExecuteCommand(VSConstants.VSStd2KCmdID.RETURN);

              WpfConsole.EndInputLine();
            }
            hr = VSConstants.S_OK;
            break;

          case VSConstants.VSStd2KCmdID.TAB:
            hr = VSConstants.S_OK;
            break;

          case VSConstants.VSStd2KCmdID.CANCEL:
            if (!IsCaretInReadOnlyRegion)
            {
              // Delete all text after InputLineStart
              WpfTextView.TextBuffer.Delete(WpfConsole.AllInputExtent);
              hr = VSConstants.S_OK;
            }
            break;
          case VSConstants.VSStd2KCmdID.CUTLINE:
            // clears the console when CutLine shortcut key is pressed,
            // usually it is Ctrl + L
            WpfConsole.ClearConsole();
            hr = VSConstants.S_OK;
            break;
        }
      }
      return hr;
    }
    
    private static readonly char[] NEWLINE_CHARS = new[] {'\n', '\r'};

    private void PasteText(ref int hr)
    {
      string text = Clipboard.GetText();
      int iLineStart = 0;
      int iNewLine;
      if (!string.IsNullOrEmpty(text) && (iNewLine = text.IndexOfAny(NEWLINE_CHARS)) >= 0)
      {
        ITextBuffer textBuffer = WpfTextView.TextBuffer;
        while (iLineStart < text.Length)
        {
          string pasteLine = (iNewLine >= 0 ? text.Substring(iLineStart, iNewLine - iLineStart) : text.Substring(iLineStart));

          if (iLineStart == 0)
          {
            if (!WpfTextView.Selection.IsEmpty)
            {
              textBuffer.Replace(WpfTextView.Selection.SelectedSpans[0], pasteLine);
            } else
            {
              textBuffer.Insert(WpfTextView.Caret.Position.BufferPosition.Position, pasteLine);
            }

            this.Execute(VSConstants.VSStd2KCmdID.RETURN);
          } else
          {
            WpfConsole.Dispatcher.PostInputLine(new InputLine(pasteLine, iNewLine >= 0));
          }

          if (iNewLine < 0)
          {
            break;
          }

          iLineStart = iNewLine + 1;
          char c;
          if (iLineStart < text.Length && (c = text[iLineStart]) != text[iNewLine] && (c == '\n' || c == '\r'))
          {
            iLineStart++;
          }
          iNewLine = (iLineStart < text.Length ? text.IndexOfAny(NEWLINE_CHARS, iLineStart) : -1);
        }

        hr = VSConstants.S_OK; // completed, eat it
      }
    }
  }
}