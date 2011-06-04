using System;

namespace js.net.jish.IL
{
  public class JishProxy
  {
    private readonly object[] thiss;

    public JishProxy(object[] thiss)
    {
      this.thiss = thiss;
    }

    public object GetInstance(int idx)
    {
      Console.WriteLine("GetInstance: " + thiss[idx]);
      return thiss[idx];
    }    
  }
}