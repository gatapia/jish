﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using js.net.jish.Util;

namespace js.net.jish.Command.InlineCommand
{
  public class CreateCommand : IInlineCommand
  {
    private readonly TypeCreator typeCreator;

    public CreateCommand(TypeCreator typeCreator)
    {
      Trace.Assert(typeCreator != null);

      this.typeCreator = typeCreator;
    }
    
    public string GetNameSpace()
    {
      return "jish";
    }
    
    public object create(string typeName, params object[] args) 
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(typeName));      
      return typeCreator.CreateType(typeName, args);
    }

    public string GetName()
    {
      return "create";
    }

    public string GetHelpDescription()
    {
      return "Creates and instance of any type (including static classes).  If the\n\t\ttype's assembly is not loaded you must precede this call with a\n\t\tcall to jish.assembly('assemblyFileName').";
    }

    public IEnumerable<CommandParam> GetParameters()
    {
      return new[]
      {
        new CommandParam {Name = "typeName"},
        new CommandParam {Name = "args", IsParams = true }
      };
    }
  }
}
