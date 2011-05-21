using System;
using System.Diagnostics;
using System.IO;
using js.net.Engine;

namespace js.net.TestAdapters
{
  public abstract class AbstractTestAdapterFactory : ITestAdapterFactory
  {    
    private readonly IEngineFactory engineFactory;
    private readonly string frameworkJsFile;
    private IEngine engine;
    

    protected AbstractTestAdapterFactory(string frameworkJsFile, IEngineFactory engineFactory)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(frameworkJsFile));
      Trace.Assert(File.Exists(frameworkJsFile));
      Trace.Assert(engineFactory != null);

      this.frameworkJsFile = frameworkJsFile;
      this.engineFactory = engineFactory;
    }

    public ITestAdapter CreateAdapter()
    {
      Trace.Assert(engine == null);

      engine = engineFactory.CreateEngine();      
      ITestAdapter adapter = CreateTestAdapter(engine, frameworkJsFile);
      return adapter;
    }

    protected abstract ITestAdapter CreateTestAdapter(IEngine engine, string frameworkJsFile1);    

    public IEngine GetInternalEngine()
    {
      Trace.Assert(engine != null);
      return engine;
    }
  }
}