using System;

namespace js.net.TestAdapters.Closure
{
  public class ClosureTestsConsoleScrapper : JSConsole
  {
    private readonly TestResults results;
    private readonly bool silent;
    public ClosureTestsConsoleScrapper(string testName, bool silent)
    {
      results = new TestResults(testName);
      this.silent = silent;
    }

    public TestResults GetResults()
    {
      return results;
    }

    public override void log(object message)
    {
      if (!silent) base.log(message);
      string msg = message as string;
      if (String.IsNullOrEmpty(msg)) return;
      ScrapeResultInformationFromMessage(msg);
    }

    private void ScrapeResultInformationFromMessage(string message)
    {      
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