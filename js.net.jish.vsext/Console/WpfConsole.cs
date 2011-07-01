using System;
using System.Collections.Generic;
using js.net.jish.vsext.Console.Utils;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace js.net.jish.vsext.Console
{
  internal class WpfConsole : ObjectWithFactory<WpfConsoleService>
  {
    private IVsTextBuffer bufferAdapter;
    private IContentType contentType;
    private int currentHistoryInputIndex;
    private IPrivateConsoleDispatcher dispatcher;
    private IList<string> historyInputs;
    private IHost host;
    private InputHistory inputHistory;
    private SnapshotPoint? inputLineStart;
    private PrivateMarshaler marshaler;
    private uint pdwCookieForStatusBar;
    private IReadOnlyRegion readOnlyRegionBegin;
    private IReadOnlyRegion readOnlyRegionBody;
    private IVsTextView view;
    private IVsStatusbar vsStatusBar;
    private IWpfTextView wpfTextView;

    public WpfConsole(WpfConsoleService factory, IServiceProvider sp, string contentTypeName) : base(factory)
    {
      if (sp == null)
      {
        throw new ArgumentNullException("sp");
      }

      ServiceProvider = sp;
      ContentTypeName = contentTypeName;
    }

    private IServiceProvider ServiceProvider { get; set; }
    private string ContentTypeName { get; set; }

    public IPrivateConsoleDispatcher Dispatcher
    {
      get { return dispatcher ?? (dispatcher = new ConsoleDispatcher(Marshaler)); }
    }

    private IVsUIShell VsUIShell { get { return ServiceProvider.GetService<IVsUIShell>(typeof (SVsUIShell)); } }

    private IVsStatusbar VsStatusBar
    {
      get { return vsStatusBar ?? (vsStatusBar = ServiceProvider.GetService<IVsStatusbar>(typeof (SVsStatusbar))); }
    }

    private Microsoft.VisualStudio.OLE.Interop.IServiceProvider OleServiceProvider { get { return ServiceProvider.GetService<Microsoft.VisualStudio.OLE.Interop.IServiceProvider>(typeof (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)); } }

    private IContentType ContentType
    {
      get { return contentType ?? (contentType = Factory.ContentTypeRegistryService.GetContentType(ContentTypeName) ?? Factory.ContentTypeRegistryService.AddContentType(ContentTypeName, new[] {"text"})); }
    }

    private IVsTextBuffer VsTextBuffer
    {
      get
      {
        if (bufferAdapter == null)
        {
          bufferAdapter = Factory.VsEditorAdaptersFactoryService.CreateVsTextBufferAdapter(OleServiceProvider, ContentType);
          bufferAdapter.InitializeContent(string.Empty, 0);
        }

        return bufferAdapter;
      }
    }

    public IWpfTextView WpfTextView
    {
      get { return wpfTextView ?? (wpfTextView = Factory.VsEditorAdaptersFactoryService.GetWpfTextView(VsTextView)); }
    }

    private IWpfTextViewHost WpfTextViewHost
    {
      get
      {
        var userData = VsTextView as IVsUserData;
        object data;
        Guid guidIWpfTextViewHost = DefGuidList.guidIWpfTextViewHost;
        userData.GetData(ref guidIWpfTextViewHost, out data);
        var wpfTextViewHost = data as IWpfTextViewHost;

        return wpfTextViewHost;
      }
    }

    public SnapshotPoint? InputLineStart
    {
      get
      {
        if (inputLineStart != null)
        {
          ITextSnapshot snapshot = WpfTextView.TextSnapshot;
          if (inputLineStart.Value.Snapshot != snapshot)
          {
            inputLineStart = inputLineStart.Value.TranslateTo(snapshot, PointTrackingMode.Negative);
          }
        }
        return inputLineStart;
      }
    }

    public SnapshotSpan InputLineExtent { get { return GetInputLineExtent(); } }

    public SnapshotSpan AllInputExtent
    {
      get
      {
        SnapshotPoint start = InputLineStart.Value;
        return new SnapshotSpan(start, start.Snapshot.GetEnd());
      }
    }

    private PrivateMarshaler Marshaler
    {
      get { return marshaler ?? (marshaler = new PrivateMarshaler(this)); }
    }

    public IWpfConsole MarshaledConsole { get { return Marshaler; } }

    public IHost Host
    {
      get { return host; }
      private set
      {
        if (host != null)
        {
          throw new InvalidOperationException();
        }
        host = value;
      }
    }

    private InputHistory InputHistory
    {
      get { return inputHistory ?? (inputHistory = new InputHistory()); }
    }

    public IVsTextView VsTextView
    {
      get
      {
        if (view == null)
        {
          view = Factory.VsEditorAdaptersFactoryService.CreateVsTextViewAdapter(OleServiceProvider);
          view.Initialize(VsTextBuffer as IVsTextLines, IntPtr.Zero, (uint) (TextViewInitFlags.VIF_HSCROLL | TextViewInitFlags.VIF_VSCROLL) | (uint) TextViewInitFlags3.VIF_NO_HWND_SUPPORT, null);

          // Set font and color
          var propCategoryContainer = view as IVsTextEditorPropertyCategoryContainer;
          if (propCategoryContainer != null)
          {
            IVsTextEditorPropertyContainer propContainer;
            Guid guidPropCategory = DefGuidList.guidEditPropCategoryViewMasterSettings;
            int hr = propCategoryContainer.GetPropertyCategory(ref guidPropCategory, out propContainer);
            if (hr == 0)
            {
              propContainer.SetProperty(VSEDITPROPID.VSEDITPROPID_ViewGeneral_FontCategory, new Guid(JishVSPackage.FontAndColoursGuid));
              propContainer.SetProperty(VSEDITPROPID.VSEDITPROPID_ViewGeneral_ColorCategory, new Guid(JishVSPackage.FontAndColoursGuid));
            }
          }

          // add myself as IConsole
          WpfTextView.TextBuffer.Properties.AddProperty(typeof (IConsole), this);

          // Initial mark readonly region. Must call Start() to start accepting inputs.
          SetReadOnlyRegionType(ReadOnlyRegionType.All);

          // Set some EditorOptions: -DragDropEditing, +WordWrap
          IEditorOptions editorOptions = Factory.EditorOptionsFactoryService.GetOptions(WpfTextView);
          editorOptions.SetOptionValue(DefaultTextViewOptions.DragDropEditingId, false);
          editorOptions.SetOptionValue(DefaultTextViewOptions.WordWrapStyleId, WordWrapStyles.WordWrap);
          
          // Create my Command Filter
          new WpfConsoleKeyProcessor(this);
        }

        return view;
      }
    }

    private object Content { get { return WpfTextViewHost.HostControl; } }

    private void SetReadOnlyRegionType(ReadOnlyRegionType value)
    {
      ITextBuffer buffer = WpfTextView.TextBuffer;
      ITextSnapshot snapshot = buffer.CurrentSnapshot;

      using (IReadOnlyRegionEdit edit = buffer.CreateReadOnlyRegionEdit())
      {
        edit.ClearReadOnlyRegion(ref readOnlyRegionBegin);
        edit.ClearReadOnlyRegion(ref readOnlyRegionBody);

        switch (value)
        {
          case ReadOnlyRegionType.BeginAndBody:
            if (snapshot.Length > 0)
            {
              readOnlyRegionBegin = edit.CreateReadOnlyRegion(new Span(0, 0), SpanTrackingMode.EdgeExclusive, EdgeInsertionMode.Deny);
              readOnlyRegionBody = edit.CreateReadOnlyRegion(new Span(0, snapshot.Length));
            }
            break;

          case ReadOnlyRegionType.All:
            readOnlyRegionBody = edit.CreateReadOnlyRegion(new Span(0, snapshot.Length), SpanTrackingMode.EdgeExclusive, EdgeInsertionMode.Deny);
            break;
        }

        edit.Apply();
      }
    }

    private SnapshotSpan GetInputLineExtent(int start = 0, int length = -1)
    {
      SnapshotPoint beginPoint = InputLineStart.Value + start;
      return length >= 0 ? new SnapshotSpan(beginPoint, length) : new SnapshotSpan(beginPoint, beginPoint.GetContainingLine().End);
    }

    private void BeginInputLine()
    {
      if (inputLineStart == null)
      {
        SetReadOnlyRegionType(ReadOnlyRegionType.BeginAndBody);
        inputLineStart = WpfTextView.TextSnapshot.GetEnd();
      }
    }

    public SnapshotSpan? EndInputLine(bool isEcho = false)
    {
      // Reset history navigation upon end of a command line
      ResetNavigateHistory();

      if (inputLineStart != null)
      {
        SnapshotSpan inputSpan = InputLineExtent;

        inputLineStart = null;
        SetReadOnlyRegionType(ReadOnlyRegionType.All);
        if (!isEcho)
        {
          Dispatcher.PostInputLine(new InputLine(inputSpan));
        }

        return inputSpan;
      }

      return null;
    }

    private void Write(string text)
    {
      if (inputLineStart == null) // If not in input mode, need unlock to enable output
      {
        SetReadOnlyRegionType(ReadOnlyRegionType.None);
      }

      // Append text to editor buffer
      ITextBuffer textBuffer = WpfTextView.TextBuffer;
      textBuffer.Insert(textBuffer.CurrentSnapshot.Length, text);

      // Ensure caret visible (scroll)
      WpfTextView.Caret.EnsureVisible();

      if (inputLineStart == null) // If not in input mode, need lock again
      {
        SetReadOnlyRegionType(ReadOnlyRegionType.All);
      }
    }

    private void WriteLine(string text)
    {
      // If append \n only, text becomes 1 line when copied to notepad.
      Write(text + Environment.NewLine);
    }

    private void ResetNavigateHistory()
    {
      historyInputs = null;
      currentHistoryInputIndex = -1;
    }

    public void NavigateHistory(int offset)
    {
      if (historyInputs == null)
      {
        historyInputs = InputHistory.History ?? new string[] {};

        currentHistoryInputIndex = historyInputs.Count;
      }

      int index = currentHistoryInputIndex + offset;
      if (index >= -1 && index <= historyInputs.Count)
      {
        currentHistoryInputIndex = index;
        string input = (index >= 0 && index < historyInputs.Count) ? historyInputs[currentHistoryInputIndex] : string.Empty;

        // Replace all text after InputLineStart with new text
        WpfTextView.TextBuffer.Replace(AllInputExtent, input);
        WpfTextView.Caret.EnsureVisible();
      }
    }

    private void HideProgress() { VsStatusBar.Progress(ref pdwCookieForStatusBar, 0 /* completed */, String.Empty, 100, 100); }

    private void SetExecutionMode(bool isExecuting)
    {
      if (!isExecuting)
      {
        HideProgress();

        VsUIShell.UpdateCommandUI(0 /* false = update UI asynchronously */);
      }
    }

    private void Clear()
    {
      SetReadOnlyRegionType(ReadOnlyRegionType.None);

      ITextBuffer textBuffer = WpfTextView.TextBuffer;
      textBuffer.Delete(new Span(0, textBuffer.CurrentSnapshot.Length));
      inputLineStart = null;
    }

    public void ClearConsole()
    {
      if (inputLineStart != null)
      {
        Dispatcher.ClearConsole();
      }
    }

    private class PrivateMarshaler : Marshaler<WpfConsole>, IPrivateWpfConsole
    {
      public PrivateMarshaler(WpfConsole impl) : base(impl) { }

      public SnapshotPoint? InputLineStart { get { return Invoke(() => impl.InputLineStart); } }

      public void BeginInputLine() { Invoke(() => impl.BeginInputLine()); }

      public InputHistory InputHistory { get { return Invoke(() => impl.InputHistory); } }

      public IHost Host { get { return Invoke(() => impl.Host); } set { Invoke(() => { impl.Host = value; }); } }

      public IConsoleDispatcher Dispatcher { get { return Invoke(() => impl.Dispatcher); } }

      public void Write(string text) { Invoke(() => impl.Write(text)); }

      public void WriteLine(string text) { Invoke(() => impl.WriteLine(text)); }

      public void Clear() { Invoke(impl.Clear); }

      public void SetExecutionMode(bool isExecuting) { Invoke(() => impl.SetExecutionMode(isExecuting)); }

      public object Content { get { return Invoke(() => impl.Content); } }

      public object VsTextView { get { return Invoke(() => impl.VsTextView); } }
    }

    private enum ReadOnlyRegionType
    {
      None,
      BeginAndBody,
      All
    };
  }
}