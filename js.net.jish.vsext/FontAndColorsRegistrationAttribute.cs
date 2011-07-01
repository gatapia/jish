using Microsoft.VisualStudio.Shell;

namespace js.net.jish.vsext
{
  public class FontAndColorsRegistrationAttribute : RegistrationAttribute
  {
    // this GUID is used by all VSPackages that use the default font and color configurations.
    // http://msdn.microsoft.com/en-us/library/bb165737.aspx
    private const string PackageGuid = "{F5E7E71D-1401-11D1-883B-0000F87579D2}";

    // this ID is set in VSPackage.resx
    private const int CategoryNameResourceID = 200;

    private string CategoryGuid { get; set; }
    private string ToolWindowPackageGuid { get; set; }
    private string CategoryKey { get; set; }

    public FontAndColorsRegistrationAttribute(string categoryKeyName, string categoryGuid, string toolWindowPackageGuid)
    {
      CategoryGuid = categoryGuid;
      ToolWindowPackageGuid = toolWindowPackageGuid;
      CategoryKey = "FontAndColors\\" + categoryKeyName;
    }

    public override void Register(RegistrationContext context)
    {
      using (var key = context.CreateKey(CategoryKey))
      {
        key.SetValue("Category", CategoryGuid);
        key.SetValue("Package", PackageGuid);
        key.SetValue("NameID", CategoryNameResourceID);
        key.SetValue("ToolWindowPackage", ToolWindowPackageGuid);

        // IMPORTANT: without calling Close() the values won't be persisted to registry.
        key.Close();
      }
    }

    public override void Unregister(RegistrationContext context) { context.RemoveKey(CategoryKey); }
  }
}