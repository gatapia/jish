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
    private readonly Func<ITestAdapter> adapterFactory;

    public TestSuiteRunner(Func<ITestAdapter> adapterFactory)
    {
      Trace.Assert(adapterFactory != null);

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
      Trace.Assert(files != null);

      IEnumerable<ITestResults> results = files.Select(RunSingleTest);
      return new TestSuiteResults(results);  
    }

    private ITestResults RunSingleTest(string file)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(file));
      Trace.Assert(File.Exists(file), string.Format("Could not find file '{0}'", file));

      using (ITestAdapter adapter = GetAdapter())
      {
        foreach (string globalFile in globalSourceFiles)
        {
          adapter.LoadSourceFile(globalFile);
        }
        return adapter.RunTest(file);
      }
    }

    private ITestAdapter GetAdapter()
    {
      Trace.Assert(adapterFactory != null);

      return adapterFactory();
    }
  }
}
