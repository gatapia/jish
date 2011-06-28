using System;

namespace js.net.test.module
{
  public class CreateCommandComplexFake 
  {
    public string paramsTest(params int[] intparams) { return "intparams: " + String.Join(",", intparams); }    
    public string defaultValueTest(int defParam = 10) { return "defParam: " + defParam; }    
    public string genericsTest<T>(T genericParam) { return "genericParam: " + genericParam; }
  }
}
