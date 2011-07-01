using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;

namespace js.net.jish.vsext.Console.Utils
{
  internal static class ExtensionMethods
  {
    public static SnapshotPoint GetEnd(this ITextSnapshot snapshot) { return new SnapshotPoint(snapshot, snapshot.Length); }

    public static T GetService<T>(this System.IServiceProvider sp, Type serviceType) where T : class { return (T) sp.GetService(serviceType); }

    public static void ClearReadOnlyRegion(this IReadOnlyRegionEdit readOnlyRegionEdit, ref IReadOnlyRegion readOnlyRegion)
    {
      if (readOnlyRegion != null)
      {
        readOnlyRegionEdit.RemoveReadOnlyRegion(readOnlyRegion);
        readOnlyRegion = null;
      }
    }

    public static void Execute(this IOleCommandTarget target, VSConstants.VSStd2KCmdID idCommand, object args = null) { target.Execute(VSConstants.VSStd2K, (uint) idCommand, args); }

    private static void Execute(this IOleCommandTarget target, Guid guidCommand, uint idCommand, object args = null)
    {
      IntPtr varIn = IntPtr.Zero;
      try
      {
        if (args != null)
        {
          varIn = Marshal.AllocHGlobal(NativeMethods.VariantSize);
          Marshal.GetNativeVariantForObject(args, varIn);
        }

        int hr = target.Exec(ref guidCommand, idCommand, (uint) OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, varIn, IntPtr.Zero);
        ErrorHandler.ThrowOnFailure(hr);
      } finally
      {
        if (varIn != IntPtr.Zero)
        {
          NativeMethods.VariantClear(varIn);
          Marshal.FreeHGlobal(varIn);
        }
      }
    }
  }
}