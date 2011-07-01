using System;

namespace js.net.jish.vsext.VisualStudio
{
  public interface ISolutionManager
  {
    event EventHandler SolutionOpened;
    event EventHandler SolutionClosed;

    string SolutionDirectory { get; }

    bool IsSolutionOpen { get; }
  }
}