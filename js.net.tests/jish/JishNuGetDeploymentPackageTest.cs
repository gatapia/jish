using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace js.net.tests.jish
{
  [TestFixture] public class JishNuGetDeploymentPackageTest
  {
    private const string rootDir = @"..\..\..\";
    private const string jishBuildDir = @"build\jish\tools\";
    private const string testFile = "test.js";
    private const string testContent = 
@"
var file = jish.create('System.IO.File');
if (file.Exists('test.js')) {
  console.log('success');
} else {
  console.log('fail');
}
";

    [SetUp] public void SetUp()
    {
      Directory.SetCurrentDirectory(rootDir);
      if (File.Exists(testFile))  File.Delete(testFile);
      File.WriteAllText(testFile, testContent);
    }

    [TearDown] public void TearDown()
    {
      if (File.Exists(testFile))  File.Delete(testFile);
    }

    [Test] public void TestRunSimpleScriptInOwnProcess()
    {
      AssertBuildDirectoryExistsAndAppearsCorrect();      

      const string command = jishBuildDir + "jish.exe";
      const string arguments = testFile;
      using (var process = new Process
                             {
                               StartInfo =
                                 {
                                   FileName = command,
                                   Arguments = arguments,
                                   UseShellExecute = false,
                                   RedirectStandardOutput = true,
                                   RedirectStandardError = true
                                 }
                             })
      {
        process.Start();
        string err = process.StandardError.ReadToEnd();
        Console.WriteLine(err);
        string output = process.StandardOutput.ReadToEnd();
        Assert.IsEmpty(err);
        Assert.AreEqual("Created instance of System.IO.File.\r\nsuccess\r\n", output);
        process.WaitForExit();
        Assert.AreEqual(0, process.ExitCode);
      }
    }

    private void AssertBuildDirectoryExistsAndAppearsCorrect()
    {
      Assert.IsTrue(Directory.Exists(jishBuildDir));
      string[] files = Directory.GetFiles(jishBuildDir, "*.exe");
      Assert.IsTrue(files.Single(f => f.EndsWith("jish.exe")) != null);
      files = Directory.GetFiles(jishBuildDir, "*.exe");
      Assert.IsTrue(files.Single(f => f.EndsWith("jish.exe")) != null);
    }
  }
}
