using System;
using System.ComponentModel.Composition;

namespace js.net.jish.vsext.Console
{
  // Note: This class is required
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false), MetadataAttribute] public sealed class HostNameAttribute : Attribute
  {
    public string HostName { get; private set; }

    public HostNameAttribute(string hostName)
    {
      if (hostName == null)
      {
        throw new ArgumentNullException("hostName");
      }
      HostName = hostName;
    }
  }
}