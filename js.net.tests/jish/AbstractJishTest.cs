using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using js.net.Engine;
using js.net.jish;
using js.net.jish.Command;
using js.net.jish.Util;
using js.net.Util;
using Ninject;
using NUnit.Framework;
using System.Linq;

namespace js.net.tests.jish
{
  public abstract class AbstractJishTest
  {
    private const string targetCommandDll = @"modules\js.net.test.module.dll";

    protected IJishInterpreter jish;
    protected TestingConsole console;

    [SetUp] public virtual void SetUp()
    {
      StandardKernel kernel = new StandardKernel();      
      IEngine engine = new JSNetEngine();
      kernel.Bind<IEngine>().ToConstant(engine);      
      kernel.Bind<ICurrentContextAssemblies>().To<TestCurrentContextAssemblies>();      
      console = new TestingConsole();
      LoadedAssembliesBucket bucket = new LoadedAssembliesBucket(kernel.Get<HelpMgr>(), kernel, console);
      kernel.Bind<LoadedAssembliesBucket>().ToConstant(bucket);            
      engine.SetGlobal("console", console);
      kernel.Bind<JSConsole>().ToConstant(console);
      kernel.Bind<IJishInterpreter>().To<JishInterpreter>().InSingletonScope();
      jish = kernel.Get<IJishInterpreter>();
      ((JishInterpreter) jish).ThrowErrors = true;
    }    
  }

  public class TestCurrentContextAssemblies : CurrentContextAssemblies
  {
    protected override IEnumerable<Assembly> GetCurrentDomainAssemblies()
    {
      return AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name.IndexOf("js.net.test.module") < 0);
    }    
    
  }
}