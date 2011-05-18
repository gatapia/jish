using System;
using System.Collections.Generic;
using js.net.TestAdapters.Closure;

namespace js.net.TestAdapters
{
  public class TestSuiteRunner
  {
    private readonly ITestAdapterFactory adapterFactory;

    public TestSuiteRunner(ITestAdapterFactory adapterFactory)
    {
      this.adapterFactory = adapterFactory;
    }

    public TestSuiteResults TestFiles(string[] files)
    {      
      IList<TestResults> results = new List<TestResults>();
      Array.ForEach(files, f => results.Add(RunSingleTest(f)));
      return new TestSuiteResults(results);  
    }

    private TestResults RunSingleTest(string file)
    {
      ITestAdapter adapter = GetAdapter();
      return adapter.RunTest(file);
    }

    private ITestAdapter GetAdapter()
    {
      return adapterFactory.CreateAdapter();
    }
  }
}
