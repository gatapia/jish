using System;
using System.IO;
using js.net.jish.vsext.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;

namespace js.net.jish.vsext.Console.Jish
{
  public class JishHost : IHost
  {
    private readonly IJishInterpreter jish;
    private readonly VSConsole jsConsole;
    private readonly ISolutionManager solutionManager;

    private bool initialized;

    public JishHost(VSConsole jsConsole, IJishInterpreter jish)
    {
      this.jsConsole = jsConsole;
      this.jish = jish;            
      solutionManager = ((IComponentModel) Package.GetGlobalService(typeof (SComponentModel))).GetService<ISolutionManager>();

      IsCommandEnabled = true;
    }

    private IConsole ActiveConsole { get; set; }
    public bool IsCommandEnabled { get; private set; }
    public string Prompt { get { return "> "; } }

    public void Initialize(IConsole console)
    {      
      ActiveConsole = console;     
      jsConsole.Console = console;

      DisplayWelcomeText();
      if (initialized)
      {
        return;
      }
      initialized = true;

      try
      {        
        solutionManager.SolutionOpened += (o, e) => UpdateWorkingDirectory();
        UpdateWorkingDirectory();
        solutionManager.SolutionClosed += (o, e) => UpdateWorkingDirectory();
      } catch (Exception ex)
      {
        // catch all exception as we don't want it to crash VS
        initialized = false;
        IsCommandEnabled = false;
        ReportError(ex);

        ExceptionHelper.WriteToActivityLog(ex);
      }
    }

    private void UpdateWorkingDirectory()
    {
      if (solutionManager.IsSolutionOpen)
      {
        Directory.SetCurrentDirectory(solutionManager.SolutionDirectory);
      }
    }

    private bool ExecuteHost(string command)
    {
      try
      {
        jish.ExecuteCommand(command);
      } catch (Exception e)
      {
        ExceptionHelper.WriteToActivityLog(e);
        throw;
      }

      return true;
    }

    public bool Execute(IConsole console, string command)
    {
      if (console == null)
      {
        throw new ArgumentNullException("console");
      }

      if (command == null)
      {
        throw new ArgumentNullException("command");
      }

      ActiveConsole = console;

      return ExecuteHost(command);
    }

    public void Abort() { jish.ClearBufferedCommand(); }

    private void DisplayWelcomeText() { WriteLine(@"Welcome to Jish - JavaScript Interactive Shell

Press .help for help."); }

    private void ReportError(Exception exception)
    {
      exception = ExceptionUtility.Unwrap(exception);
      WriteErrorLine(exception.Message);
    }

    private void WriteErrorLine(string message)
    {
      if (ActiveConsole != null)
      {
        ActiveConsole.Write(message + Environment.NewLine);
      }
    }

    private void WriteLine(string message = "")
    {
      if (ActiveConsole != null)
      {
        ActiveConsole.WriteLine(message);
      }
    }    
  }  
}