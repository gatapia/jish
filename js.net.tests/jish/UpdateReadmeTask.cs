using System;
using System.IO;
using NUnit.Framework;

namespace js.net.tests.jish
{
  [TestFixture] public class UpdateReadmeTask : AbstractJishTest
  {
    const string readMeFile =  @"..\..\..\README.md";
    const string buildJsFile =  @"..\..\..\build.js";
    const string buildJsSearchTag = "[in github](https://github.com/gatapia/js.net/blob/master/build.js).";
    const string helpScreenSearchTagStart = "Commands included in Jish are:";
    const string helpScreenSearchTagEnd = "## Extending Jish";

    private const string sourceCommandDll = @"..\..\..\js.net.test.module\bin\js.net.test.module.dll";
    private const string targetCommandDll = @"modules\js.net.test.module.dll";

    public override void SetUp()
    {
      if (!Directory.Exists("modules")) { Directory.CreateDirectory("modules"); }
      File.Copy(sourceCommandDll, targetCommandDll, true);
      base.SetUp();
    }

    public void TearDown()
    {
      if (File.Exists(targetCommandDll)) File.Delete(targetCommandDll);
    }

    [Test] public void AddBuildJSToTailOfReadme()
    {
      ValidateReadmeIsStillValidFormat();

      string readmeContents = File.ReadAllText(readMeFile);      
      readmeContents = readmeContents.Substring(0, readmeContents.IndexOf(buildJsSearchTag) + buildJsSearchTag.Length);
      readmeContents += "\n";

      string buildJsContents = "\n\n    " + File.ReadAllText(buildJsFile);
      buildJsContents = buildJsContents.Replace("\n", "\n    "); // Markdown code block
      readmeContents += buildJsContents;

      File.WriteAllText(readMeFile, readmeContents);
    }

    [Test] public void AddHelpScreenToReadme()
    {
      ValidateReadmeIsStillValidFormat();

      string readmeContents = File.ReadAllText(readMeFile);
      string pre = readmeContents.Substring(0, readmeContents.IndexOf(helpScreenSearchTagStart) + helpScreenSearchTagStart.Length);
      string post = readmeContents.Substring(readmeContents.IndexOf(helpScreenSearchTagEnd));
      jish.ExecuteCommand(".help");
      string help = console.GetLastMessage();
      help = "\n\n    " + help.Replace("\n", "\n    ") + "\n\n";

      File.WriteAllText(readMeFile, pre + help + post);
    }

    [Test] public void ZipGitHubDownloads()
    {
      Directory.SetCurrentDirectory(@"..\..\..");

      jish.ExecuteCommand(@"
build.zip('build\\jish.exe.zip', ['build\\jish\\tools\\jish.exe', 'build\\jish\\tools\\Noesis.Javascript.dll']);
build.zip('build\\js.net.dll.zip', ['build\\js.net\\lib\\js.net.dll', 'build\\js.net\\lib\\Noesis.Javascript.dll']);
console.log('Both zip files updated to build\jish.exe.zip and build\js.net.dll.zip.  Please update github downloads');
");
      Assert.IsTrue(File.Exists(@"build\js.net.dll.zip"));
      Assert.IsTrue(File.Exists(@"build\js.net.dll.zip"));
    }

    private void ValidateReadmeIsStillValidFormat()
    {
      string readmeContents = File.ReadAllText(readMeFile);            
      // build.js
      int searchIdx = readmeContents.IndexOf(buildJsSearchTag);
      Assert.Greater(searchIdx, 0, "Could not find: " + buildJsSearchTag);
      Assert.AreEqual(-1, readmeContents.IndexOf('#', searchIdx + 4), "It appears that additional headings have been added after: " + buildJsSearchTag); // No more headings added after build.js

      // Help Screen
      Assert.Greater(readmeContents.IndexOf(helpScreenSearchTagStart), 0, "Could not find " + helpScreenSearchTagStart);
      Assert.Greater(readmeContents.IndexOf(helpScreenSearchTagEnd), 0, "Could not find " + helpScreenSearchTagEnd);
      Assert.Greater(readmeContents.IndexOf(helpScreenSearchTagEnd), readmeContents.IndexOf(helpScreenSearchTagStart));
    }
  }
}