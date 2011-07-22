using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace js.net.Util
{
  public class HtmlFileScriptExtractor
  {        
    public string GetScriptContents(string file)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(file));
      Trace.Assert(File.Exists(file));
      Trace.Assert(file.EndsWith(".html") || file.EndsWith(".htm"));

      string html = File.ReadAllText(file);
      string js = "";
      int startIndex;
      while ((startIndex = html.IndexOf("<script")) >= 0)
      {
        html = html.Substring(startIndex + 1); // Ensure we dont match again
        Match m = Regex.Match(html, @"src *= *['""](.*)['""]");
        string content;
        if (m.Success) { // Has Source
          string srcFile = m.Groups[1].Value;        
          if (srcFile.IndexOf("base.js") >= 0) { continue; }

          string parent = new FileInfo(file).Directory.FullName;
          content = File.ReadAllText(Path.Combine(parent, srcFile));
        } else { // Is Inline Script
          html = html.Substring(html.IndexOf(">") + 1);
          content = "\n" + html.Substring(0, html.IndexOf("</script>"));          
        }
        js += content;
      }
      // SaveDebugFile(file, js);      
      return js;
    }

    private void SaveDebugFile(string file, string jsContents) { 
      string debug = file.Replace(".html", ".debug.js").Replace(".htm", ".debug.js");
      if (File.Exists(debug)) File.Delete(debug);
      File.WriteAllText(debug, jsContents);      
    }
  }
}