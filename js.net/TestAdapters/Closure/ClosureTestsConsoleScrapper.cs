﻿using System;
using System.Diagnostics;
using js.net.Util;

namespace js.net.TestAdapters.Closure
{
  public class ClosureTestsConsoleScrapper : JSConsole
  {
    private readonly TestResults results;

    public ClosureTestsConsoleScrapper(string testFixtureName)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(testFixtureName));      

      results = new TestResults(testFixtureName);
    }

    public TestResults GetResults()
    {
      return results;
    }

    protected override void logImpl(string message, bool newline)
    {
      base.logImpl(message, newline);

      if (!String.IsNullOrEmpty(message)) ScrapeResultInformationFromMessage(message);
    }

    private void ScrapeResultInformationFromMessage(string message)
    {            
      Trace.Assert(message != null);

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
      Trace.Assert(message != null);

      string testName = message.Substring(message.IndexOf(": ") + 2).Trim();
      return testName.Substring(0, testName.IndexOf(": "));
    }
  }
}