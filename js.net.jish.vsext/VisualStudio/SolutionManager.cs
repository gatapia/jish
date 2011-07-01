using System;
using System.ComponentModel.Composition;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace js.net.jish.vsext.VisualStudio
{
  [PartCreationPolicy(CreationPolicy.Shared), Export(typeof (ISolutionManager))] public class SolutionManager : ISolutionManager
  {
    private readonly DTE dte;
    private readonly SolutionEvents solutionEvents;

    private SolutionManager()
    {
      dte = (DTE) Package.GetGlobalService(typeof (DTE));

      solutionEvents = dte.Events.SolutionEvents;
      solutionEvents.Opened += OnSolutionOpened;
      solutionEvents.AfterClosing += OnAfterClosing;

      if (dte.Solution.IsOpen)
      {
        OnSolutionOpened();
      }
    }

    public event EventHandler SolutionOpened;

    public event EventHandler SolutionClosed;

    public bool IsSolutionOpen { get { return dte != null && dte.Solution != null && dte.Solution.IsOpen; } }

    public string SolutionDirectory
    {
      get
      {
        if (!IsSolutionOpen)
        {
          return null;
        }
        // Use .Properties.Item("Path") instead of .FullName because .FullName might not be
        // available if the solution is just being created
        Property property = dte.Solution.Properties.Item("Path");
        if (property == null)
        {
          return null;
        }
        string solutionFilePath = property.Value;
        if (String.IsNullOrEmpty(solutionFilePath))
        {
          return null;
        }
        return Path.GetDirectoryName(solutionFilePath);
      }
    }

    private void OnAfterClosing()
    {
      if (SolutionClosed != null)
      {
        SolutionClosed(this, EventArgs.Empty);
      }
    }

    private void OnSolutionOpened()
    {
      if (SolutionOpened != null)
      {
        SolutionOpened(this, EventArgs.Empty);
      }
    }
  }
}