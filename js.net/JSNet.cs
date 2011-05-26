﻿using System.Diagnostics;
using js.net.Engine;
using js.net.FrameworkAdapters;
using js.net.FrameworkAdapters.Closure;
using js.net.TestAdapters;
using js.net.TestAdapters.Closure;
using js.net.TestAdapters.Jasmine;
using js.net.TestAdapters.JSCoverage;
using js.net.TestAdapters.JSUnit;
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

    public static ICoverageAdapter JSCoverage(ITestAdapter testAdapter)
    {
      JSCoverageTestAdapter jsCoverage = new JSCoverageTestAdapter(testAdapter);
      return jsCoverage;
    }

    public static ITestAdapter ClosureLibrary(string baseJsFile, string jsDomSourceFile)
    {
      ClosureAdapter adapter = new ClosureAdapter(baseJsFile, jsDomSourceFile, new JSNetEngine());
      adapter.Initialise();
      return new ClosureTestAdapter(adapter);
    }

    public static TestSuiteRunner ClosureLibraryTestSuiteRunner(string baseJsFile, string jsDomSourceFile)
    {
      ClosureTestAdapterFactory fact = new ClosureTestAdapterFactory(baseJsFile, jsDomSourceFile, new DefaultEngineFactory());
      return new TestSuiteRunner(fact);
    }

    public static ITestAdapter JSUnit(string jsUnitCoreFile)
    {
      return new JSUnitTestAdapter(GetSimpleDOMAdapter(), jsUnitCoreFile);
    }

    public static TestSuiteRunner JSUnitTestSuiteRunner(string jsUnitCoreFile)
    {
      JSUnitTestAdapterFactory fact = new JSUnitTestAdapterFactory(jsUnitCoreFile, new DefaultEngineFactory());
      return new TestSuiteRunner(fact);
    }

    public static ITestAdapter QUnit(string qUnitJsFile)
    {
      return new QUnitTestAdapter(GetSimpleDOMAdapter(), qUnitJsFile);
    }

    public static TestSuiteRunner QUnitTestSuiteRunner(string qUnitJsFile)
    {
      QUnitTestAdapterFactory fact = new QUnitTestAdapterFactory(qUnitJsFile, new DefaultEngineFactory());
      return new TestSuiteRunner(fact);
    }

    public static ITestAdapter Jasmine(string jasmineJsFile)
    {
      return new JasmineTestAdapter(GetSimpleDOMAdapter(), jasmineJsFile);
    }

    public static TestSuiteRunner JasmineTestSuiteRunner(string jasmineJsFile)
    {
      JasmineTestAdapterFactory fact = new JasmineTestAdapterFactory(jasmineJsFile, new DefaultEngineFactory());
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
