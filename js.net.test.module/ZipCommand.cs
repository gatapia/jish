using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using js.net.jish;
using js.net.jish.Command;
using js.net.jish.Command.InlineCommand;

namespace js.net.test.module
{
  public class ZipCommand : IInlineCommand
  {
    public string GetNameSpace()
    {
      return "build";
    }

    public void zip(string to, string[] files)
    {
      using (ZipOutputStream s = new ZipOutputStream(File.Create(to)))
      {
        s.SetLevel(9); 

        byte[] buffer = new byte[4096];

        foreach (string file in files)
        {
          ZipEntry entry = new ZipEntry(Path.GetFileName(file)) { DateTime = DateTime.Now };
          s.PutNextEntry(entry);

          using (FileStream fs = File.OpenRead(file))
          {
            int sourceBytes;
            do
            {
              sourceBytes = fs.Read(buffer, 0, buffer.Length);
              s.Write(buffer, 0, sourceBytes);
            } while (sourceBytes > 0);
          }
        }
        s.Finish();
        s.Close();
      }
    }

    public string GetName()
    {
      return "zip";
    }

    public string GetHelpDescription()
    {
      return "zips input files into single output.";
    }

    public IEnumerable<CommandParam> GetParameters()
    {
      return new[] { new CommandParam { Name = "to" }, new CommandParam { Name = "files" } };
    }
  }
}