using System;

namespace js.net.jish.vsext.Console.Utils
{
  internal class ObjectWithFactory<T> where T : class
  {
    protected T Factory { get; private set; }

    protected ObjectWithFactory(T factory)
    {
      if (factory == null)
      {
        throw new ArgumentNullException("factory");
      }
      Factory = factory;
    }
  }
}