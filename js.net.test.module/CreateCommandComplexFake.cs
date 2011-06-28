using System;
using System.Collections.Generic;
using js.net.jish.Command;
using js.net.jish.Command.InlineCommand;

namespace js.net.test.module
{
  public class CreateCommandComplexFake 
  {
    public string paramsTest(params int[] intparams) { return "intparams: " + String.Join(",", intparams); }    
    public string defaultValueTest(int defParam = 10) { return "defParam: " + defParam; }    
    public string genericsTest<T>(T genericParam) { return "genericParam: " + genericParam; }
  }

  public class CreateCommandComplexFakeCommand : CreateCommandComplexFake, IInlineCommand
  {
    public string GetName() { return "tst"; }
    public string GetHelpDescription() { return "hlp"; }
    public IEnumerable<CommandParam> GetParameters() { return null; }
    public string GetNameSpace() { return "tst"; }
  }
}
