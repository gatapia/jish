using System;
using System.Runtime.InteropServices;

namespace js.net.jish.vsext.Console
{
  internal static class NativeMethods
  {
    public const int VariantSize = 16;
    [DllImport("Oleaut32.dll", PreserveSig = false)] public static extern void VariantClear(IntPtr var);
  }
}