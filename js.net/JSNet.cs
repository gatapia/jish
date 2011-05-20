using System.Diagnostics;
using js.net.Engine;
using js.net.FrameworkAdapters;
using js.net.FrameworkAdapters.Closure;
using js.net.TestAdapters;
using js.net.TestAdapters.Closure;
using js.net.TestAdapters.Jasmine;
using js.net.TestAdapters.JsUnit;
using js.net.TestAdapters.QUnit;

namespace js.net
{
  // A helper utility class for easy use of the entire project
  public static class JSNet
  {
    static JSNet()
    {      
      DefaultTraceListener def = (DefaultTraceListener) Trace.Listeners[0];
      def.AssertUiEnabled = false; // No silly dialogs
      Trace.Listeners.Clear();
      Trace.Listeners.Add(def);
    }

    public static ITestAdapter ClosureLibrary(string baseJsFile)
    {
      ClosureAdapter adapter = new ClosureAdapter(baseJsFile, new JSNetEngine());
      adapter.Initialise();
      return new ClosureTestAdapter(adapter);
    }

    public static TestSuiteRunner ClosureLibraryTestSuiteRunner(string baseJsFile)
    {
      ClosureTestAdapterFactory fact = new ClosureTestAdapterFactory(baseJsFile, new DefaultEngineFactory()) { Silent = true };
      return new TestSuiteRunner(fact);
    }

    public static ITestAdapter JSUnit(string jsUnitCoreFile)
    {
      return new JsUnitTestAdapter(GetSimpleDOMAdapter(), jsUnitCoreFile);
    }

    public static TestSuiteRunner JSUnitTestSuiteRunner(string jsUnitCoreFile)
    {
      JsUnitTestAdapterFactory fact = new JsUnitTestAdapterFactory(jsUnitCoreFile, new DefaultEngineFactory()) { Silent = true };
      return new TestSuiteRunner(fact);
    }

    public static ITestAdapter QUnit(string qUnitJsFile)
    {
      return new QUnitTestAdapter(GetSimpleDOMAdapter(), qUnitJsFile);
    }

    public static TestSuiteRunner QUnitTestSuiteRunner(string qUnitJsFile)
    {
      QUnitTestAdapterFactory fact = new QUnitTestAdapterFactory(qUnitJsFile, new DefaultEngineFactory()) { Silent = true };
      return new TestSuiteRunner(fact);
    }

    public static ITestAdapter Jasmine(string jasmineJsFile)
    {
      return new JasmineTestAdapter(GetSimpleDOMAdapter(), jasmineJsFile);
    }

    public static TestSuiteRunner JasmineTestSuiteRunner(string jasmineJsFile)
    {
      JasmineTestAdapterFactory fact = new JasmineTestAdapterFactory(jasmineJsFile, new DefaultEngineFactory()) { Silent = true };
      return new TestSuiteRunner(fact);
    }
    
    private static SimpleDOMAdapter GetSimpleDOMAdapter()
    {
      SimpleDOMAdapter adapter = new SimpleDOMAdapter(new JSNetEngine());
      adapter.Initialise();
      return adapter;
    }
  }  
}
