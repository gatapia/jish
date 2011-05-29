﻿using System.Text.RegularExpressions;

namespace js.net.jish.Command
{
  public abstract class ParseInputCommand : EmptyCommand
  {    
    protected string ParseFileOrTypeName(string input)
    {
      input = input.Substring(input.IndexOf('('));
      return new Regex(@"([A-z0-9\.\\, '""])+").Match(input).Captures[0].Value.Trim();
    }
  }
}