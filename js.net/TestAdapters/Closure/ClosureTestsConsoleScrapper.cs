using System;

namespace js.net.TestAdapters.Closure
{
  // TODO: We should not need a console scrapper to get results, we should
  // just plug into the test runner.
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
      if (message.IndexOf(" : PASSED") < 0 && message.IndexOf(" : FAILED") < 0) { return; }
      string testName = GetTestNameFromPassOrFailLogLine(message);
      if (String.IsNullOrWhiteSpace(testName)) { return; }

      if (message.IndexOf(" : PASSED") >= 0)
      {
        results.AddPassedTest(testName);
      } else if (message.IndexOf(" : FAILED") >= 0)
      {
        results.AddFailedTest(testName);
      }
    }

    private string GetTestNameFromPassOrFailLogLine(string message)
    {
      string testName = message.Substring(message.IndexOf(": ") + 2).Trim();
      return testName.Substring(0, testName.IndexOf(": "));
    }
  }
}