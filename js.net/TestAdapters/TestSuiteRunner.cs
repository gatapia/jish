using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace js.net.TestAdapters
{
  public class TestSuiteRunner
  {
    private readonly IList<string> globalSourceFiles = new List<string>();
    private readonly ITestAdapterFactory adapterFactory;

    public TestSuiteRunner(ITestAdapterFactory adapterFactory)
    {
      this.adapterFactory = adapterFactory;
    }

    public void AddGlobalSourceFile(string file)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(file));
      Trace.Assert(File.Exists(file));
      globalSourceFiles.Add(file);
    }

    public TestSuiteResults TestFiles(IEnumerable<string> files)
    {      
      IEnumerable<TestResults> results = files.Select(RunSingleTest);
      return new TestSuiteResults(results);  
    }

    private TestResults RunSingleTest(string file)
    {
      ITestAdapter adapter = GetAdapter();
      foreach(string globalFile in globalSourceFiles)
      {
        adapter.LoadSourceFile(globalFile);
      }
      return adapter.RunTest(file);
    }

    private ITestAdapter GetAdapter()
    {
      return adapterFactory.CreateAdapter();
    }
  }
}
