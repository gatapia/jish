using System;
using Microsoft.VisualStudio.Shell;

namespace js.net.jish.vsext
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)] public sealed class ProvideBindingPathAttribute : RegistrationAttribute
  {    
    private static string GetPathToKey(RegistrationContext context) { return string.Concat(@"BindingPaths\", context.ComponentType.GUID.ToString("B").ToUpperInvariant()); }

    public override void Register(RegistrationContext context)
    {
      if (context == null)
      {
        throw new ArgumentNullException("context");
      }

      using (Key childKey = context.CreateKey(GetPathToKey(context)))
      {
        childKey.SetValue(context.ComponentPath, string.Empty);
      }
    }

    public override void Unregister(RegistrationContext context)
    {
      if (context == null)
      {
        throw new ArgumentNullException("context");
      }

      context.RemoveKey(GetPathToKey(context));
    }
  }
}