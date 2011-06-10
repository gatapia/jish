using js.net.Engine;
using js.net.FrameworkAdapters;
using js.net.FrameworkAdapters.Closure;
using js.net.TestAdapters;
using js.net.TestAdapters.Closure;
using js.net.TestAdapters.Coverage;
using js.net.TestAdapters.Coverage.JSCoverage;
using js.net.TestAdapters.Jasmine;
using js.net.TestAdapters.JSUnit;
using js.net.TestAdapters.QUnit;
using Ninject;
using Ninject.Parameters;

namespace js.net
{
  // A helper utility class for easy use of the entire project
  public static class JSNet
  {
    private static readonly IKernel kernel;

    static JSNet()
    {
      kernel = new StandardKernel();
      kernel.Bind<IEngine>().To<JSNetEngine>();
      kernel.Bind<IFrameworkAdapter>().To<JSDomAdapter>().OnActivation(jsdom => ((JSDomAdapter)jsdom).Initialise());
      kernel.Bind<CWDFileLoader>().ToSelf().InSingletonScope();
      kernel.Bind<ClosureFrameworkAdapter>().ToSelf().OnActivation(closure => closure.Initialise());
    }

    public static ICoverageAdapter JSCoverage(ITestAdapter testAdapter)
    {
      return new JSCoverageTestAdapter(testAdapter);
    }

    public static ITestAdapter ClosureLibrary(string baseJsFile)
    {            
      ClosureFrameworkAdapter adapter = kernel.Get<ClosureFrameworkAdapter>(new[] { new ConstructorArgument("baseJsFile", baseJsFile) });
      return new ClosureTestAdapter(adapter);
    }

    public static TestSuiteRunner ClosureLibraryTestSuiteRunner(string baseJsFile)
    {      
      return new TestSuiteRunner(() => ClosureLibrary(baseJsFile));
    }

    public static ITestAdapter JSUnit(string jsUnitCoreFile)
    {
      return new JSUnitTestAdapter(kernel.Get<IFrameworkAdapter>(), jsUnitCoreFile);
    }

    public static TestSuiteRunner JSUnitTestSuiteRunner(string jsUnitCoreFile)
    {
      return new TestSuiteRunner(() => JSUnit(jsUnitCoreFile));
    }

    public static ITestAdapter QUnit(string qUnitJsFile)
    {
      return new QUnitTestAdapter(kernel.Get<IFrameworkAdapter>(), qUnitJsFile);
    }

    public static TestSuiteRunner QUnitTestSuiteRunner(string qUnitJsFile)
    {
      return new TestSuiteRunner(() => QUnit(qUnitJsFile));
    }

    public static ITestAdapter Jasmine(string jasmineJsFile)
    {
      return new JasmineTestAdapter(kernel.Get<IFrameworkAdapter>(), jasmineJsFile);
    }

    public static TestSuiteRunner JasmineTestSuiteRunner(string jasmineJsFile)
    {
      return new TestSuiteRunner(() => Jasmine(jasmineJsFile));
    }    
  }  
}
