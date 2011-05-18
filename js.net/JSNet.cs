using System.Collections.Generic;
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
  // A helper utility class for easy use of the project
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
      return new ClosureTestAdapter(new ClosureAdapter(baseJsFile, new JSNetEngine()));
    }

    public static TestSuiteRunner ClosureLibraryTestSuiteRunner(string baseJsFile)
    {
      ClosureTestAdapterFactory fact = new ClosureTestAdapterFactory(baseJsFile, new DefaultEngineFactory()) { Silent = true };
      return new TestSuiteRunner(fact);
    }

    public static ITestAdapter JSUnit(string jsUnitCoreFile)
    {
      return new JsUnitTestAdapter(new SimpleDOMAdapter(new JSNetEngine()), jsUnitCoreFile);
    }

    public static TestSuiteRunner JSUnitTestSuiteRunner(string jsUnitCoreFile)
    {
      JsUnitTestAdapterFactory fact = new JsUnitTestAdapterFactory(jsUnitCoreFile, new DefaultEngineFactory()) { Silent = true };
      return new TestSuiteRunner(fact);
    }

    public static ITestAdapter QUnit(string qUnitJsFile)
    {
      return new QUnitTestAdapter(new SimpleDOMAdapter(new JSNetEngine()), qUnitJsFile);
    }

    public static TestSuiteRunner QUnitTestSuiteRunner(string qUnitJsFile)
    {
      QUnitTestAdapterFactory fact = new QUnitTestAdapterFactory(qUnitJsFile, new DefaultEngineFactory()) { Silent = true };
      return new TestSuiteRunner(fact);
    }

    public static ITestAdapter Jasmine(string jasmineJsFile)
    {
      return new JasmineTestAdapter(new SimpleDOMAdapter(new JSNetEngine()), jasmineJsFile);
    }

    public static TestSuiteRunner JasmineTestSuiteRunner(string jasmineJsFile)
    {
      JasmineTestAdapterFactory fact = new JasmineTestAdapterFactory(jasmineJsFile, new DefaultEngineFactory()) { Silent = true };
      return new TestSuiteRunner(fact);
    }
  }  
}
