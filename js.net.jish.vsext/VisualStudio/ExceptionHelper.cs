using System;
using Microsoft.VisualStudio.Shell;

namespace js.net.jish.vsext.VisualStudio
{
  public static class ExceptionHelper
  {
    private const string LogEntrySource = "Jish";

    public static void WriteToActivityLog(Exception exception)
    {
      if (exception == null)
      {
        throw new ArgumentNullException("exception");
      }

      exception = ExceptionUtility.Unwrap(exception);

      ActivityLog.LogError(LogEntrySource, exception.Message + exception.StackTrace);
    }
  }
}