﻿using js.net.Engine;
using js.net.jish;
using js.net.Util;
using Ninject;
using NUnit.Framework;

namespace js.net.tests.jish
{
  public abstract class AbstractJishTest
  {
    protected IJishInterpreter jish;
    protected TestingConsole console;

    [SetUp] public virtual void SetUp()
    {
      StandardKernel kernel = new StandardKernel();      
      IEngine engine = new JSNetEngine();
      kernel.Bind<IEngine>().ToConstant(engine);      
      console = new TestingConsole();      
      engine.SetGlobal("console", console);
      kernel.Bind<JSConsole>().ToConstant(console);
      kernel.Bind<IJishInterpreter>().To<JishInterpreter>().InSingletonScope();
      jish = kernel.Get<IJishInterpreter>();
      ((JishInterpreter) jish).ThrowErrors = true;
    }    
  }
}