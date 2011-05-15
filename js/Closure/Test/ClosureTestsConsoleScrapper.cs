namespace js.Closure.Test
{
  public class ClosureTestsConsoleScrapper : JSConsole
  {
    private readonly ClosureTestResults results;
    private readonly bool silent;
    public ClosureTestsConsoleScrapper(string testName, bool silent)
    {
      results = new ClosureTestResults(testName);
      this.silent = silent;
    }

    public ClosureTestResults GetResults()
    {
      return results;
    }

    public override void log(string message)
    {
      if (!silent) base.log(message);
      ScrapeResultInformationFromMessage(message);
    }

    private void ScrapeResultInformationFromMessage(string message)
    {
//      This will be usefull if we want to keep all the console.log messages
//      together with the actual test that generated them.
//
//      int startIdx = message.IndexOf("Running test: ");
//      if (startIdx >= 0)
//      {
//        string testName = message.Substring(message.IndexOf(": ", startIdx) + 2).Trim();
//        results.AddTest(testName);
//      }
      if (message.IndexOf(" : PASSED") >= 0)
      {
        results.AddPassedTest(GetTestNameFromPassOrFailLogLine(message));
      }     
      if (message.IndexOf(" : FAILED") >= 0)
      {
        results.AddFailedTest(GetTestNameFromPassOrFailLogLine(message));
      }      
    }

    private string GetTestNameFromPassOrFailLogLine(string message)
    {
      string testName = message.Substring(message.IndexOf(" : ") + 3).Trim();
      return testName.Substring(0, testName.IndexOf(" : "));
    }
  }
}