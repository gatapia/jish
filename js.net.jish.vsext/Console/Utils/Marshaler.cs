using System;
using Microsoft.VisualStudio.Shell;

namespace js.net.jish.vsext.Console.Utils
{
  internal class Marshaler<T> where T : class
  {
    protected readonly T impl;

    protected Marshaler(T impl) { this.impl = impl; }

    private static ThreadHelper ThreadHelper { get { return ThreadHelper.Generic; } }

    protected static void Invoke(Action action) { ThreadHelper.Invoke(action); }

    protected static TResult Invoke<TResult>(Func<TResult> func) { return ThreadHelper.Invoke(func); }
  }
}