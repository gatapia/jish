using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace js.net.tests.jish
{
  [TestFixture] public class UpdateReadmeTask : AbstractJishTest
  {
    const string readMeFile =  @"..\..\..\README.md";
    const string buildJsFile =  @"..\..\..\build.js";
    const string buildJsSearchTag = "[in github](https://github.com/gatapia/js.net/blob/master/build.js).";
    const string helpScreenSearchTagStart = "Jish are:";
    const string helpScreenSearchTagEnd = "## Extending Jish";
    const string inlineCommandSearchTagStart = "`jish.process('commandName', 'arguments_string')`:";
    const string inlineCommandSearchTagEnd = "### js.net.jish.Command.IConsoleCommand (Console Commands)";
    const string jSNetNUnitFixtureSearchTagStart = "do the following:";
    const string jSNetNUnitFixtureSearchTagEnd = "## Coverage";


    public override void SetUp()
    {
      ValidateReadmeIsStillValidFormat();
      base.SetUp();
    }

    [Test] public void AddBuildJSToTailOfReadme()
    {
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
      jish.ExecuteCommand(".help");
      string help = console.GetLastMessage();
      AddPrePostTagTextToReadme(helpScreenSearchTagStart, helpScreenSearchTagEnd, MakeCodeBlock(help));
    }    

    [Test] public void AddIInlineCommandDescriptionToReadme()
    {
      string processCommandContents = File.ReadAllText(@"..\..\..\js.net.jish\Command\InlineCommand\ProcessCommand.cs");
      AddPrePostTagTextToReadme(inlineCommandSearchTagStart, inlineCommandSearchTagEnd, MakeCodeBlock(processCommandContents));
    }

    [Test] public void AddJSNetNUnitFixtureDescription() {
      string[] fixtureCodeLines = File.ReadAllLines(@"..\..\..\js.net\TestAdapters\JSNetNUnitFixture.cs");
      IList<string> neededLines = new List<string>();
      bool inCodeBlock = false;
      foreach (var line in fixtureCodeLines)
      {
        if (line.IndexOf("<code>") >= 0) inCodeBlock = true;
        else if (line.IndexOf("</code>") >= 0) inCodeBlock = false;
        else if (inCodeBlock) neededLines.Add(line.Replace("  /// ", string.Empty));
      }      
      string code = String.Join("\n", neededLines);
      AddPrePostTagTextToReadme(jSNetNUnitFixtureSearchTagStart, jSNetNUnitFixtureSearchTagEnd, MakeCodeBlock(code));
    }

    private string MakeCodeBlock(string block) {
      block = "\n\n    " + block.Replace("\n", "\n    ") + "\n\n";
      return block;
    }

    private void AddPrePostTagTextToReadme(string startTag, string endTag, string contents) {
      string readmeContents = File.ReadAllText(readMeFile);
      string pre = readmeContents.Substring(0, readmeContents.IndexOf(startTag) + startTag.Length);
      string post = readmeContents.Substring(readmeContents.IndexOf(endTag));      

      File.WriteAllText(readMeFile, pre + contents + post);
    }

    private void ValidateReadmeIsStillValidFormat()
    {
      string readmeContents = File.ReadAllText(readMeFile);            
      
      // build.js
      int searchIdx = readmeContents.IndexOf(buildJsSearchTag);
      Assert.Greater(searchIdx, 0, "Could not find: " + buildJsSearchTag);
      Assert.AreEqual(-1, readmeContents.IndexOf('#', searchIdx + 4), "It appears that additional headings have been added after: " + buildJsSearchTag); // No more headings added after build.js

      // Help Screen
      ValidateStarAndEndIndexes(readmeContents, helpScreenSearchTagStart, helpScreenSearchTagEnd);      

      // IInlineCommand
      ValidateStarAndEndIndexes(readmeContents, inlineCommandSearchTagStart, inlineCommandSearchTagEnd);      

      // JSNetNUnitFixture
      ValidateStarAndEndIndexes(readmeContents, jSNetNUnitFixtureSearchTagStart, jSNetNUnitFixtureSearchTagEnd);      
    }

    private void ValidateStarAndEndIndexes(string readmeContents, string startTag, string endTag)
    {
      Assert.Greater(readmeContents.IndexOf(startTag), 0, "Could not find " + startTag);
      Assert.Greater(readmeContents.IndexOf(endTag), 0, "Could not find " + endTag);
      Assert.Greater(readmeContents.IndexOf(endTag), readmeContents.IndexOf(startTag));
    }
  }
}