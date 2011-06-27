using System;
using System.Collections.Generic;
using js.net.jish.Command;
using js.net.jish.Command.InlineCommand;

namespace js.net.test.module
{
  public class TestComplexInlineCommand : IInlineCommand
  {
    public string GetNameSpace()
    {
      return "complex_inline_command";
    }

    public string paramsTest(params int[] intparams) { return "intparams: " + String.Join(",", intparams); }    
    public string defaultValueTest(int defParam = 10) { return "defParam: " + defParam; }    
    public string genericsTest<T>(T genericParam) { return "genericParam: " + genericParam; }

    public string GetName() { return "test"; }
    public string GetHelpDescription() { return "test"; }
    public IEnumerable<CommandParam> GetParameters() { return new CommandParam[] { }; }
  }
}
