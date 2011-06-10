using System;
using System.Diagnostics;
using System.IO;

namespace js.net
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
      while (html.IndexOf("<script") >= 0)
      {
        html = html.Substring(html.IndexOf(">", html.IndexOf("<script")) + 1);
        js += "\n" + html.Substring(0, html.IndexOf("</script>"));
      }
      // File.WriteAllText(file.Replace(".html", ".debug.js").Replace(".htm", ".debug.js"), js);      
      return js;
    }
  }
}