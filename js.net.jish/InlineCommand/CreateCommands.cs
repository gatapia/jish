using System.Collections.Generic;
using js.net.jish.Util;

namespace js.net.jish.InlineCommand
{
  public class CreateCommand : IInlineCommand
  {
    private readonly TypeCreator typeCreator;

    public CreateCommand(TypeCreator typeCreator)
    {
      this.typeCreator = typeCreator;
    }
    
    public string GetNameSpace()
    {
      return "jish";
    }
    
    public object create(string typeName, params object[] args) 
    {
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
