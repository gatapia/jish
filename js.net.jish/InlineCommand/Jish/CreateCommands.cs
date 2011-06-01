using System.Collections.Generic;
using System.Linq;
using js.net.jish.Util;

namespace js.net.jish.InlineCommand.Jish
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

    // This uglyness is my inplementation of param object[] NULL_ARG is used 
    // so we can still pass null values to the tail end of create and the
    // correct constructor should be called
    private static readonly object NULL_ARG = new object();
    public object create(string typeName) { return create(typeName, NULL_ARG); }    
    public object create(string typeName, object a1) { return create(typeName, a1, NULL_ARG); }    
    public object create(string typeName, object a1, object a2) { return create(typeName, a1, a2, NULL_ARG); }    
    public object create(string typeName, object a1, object a2, object a3) { return create(typeName, a1, a2, a3, NULL_ARG); }    
    public object create(string typeName, object a1, object a2, object a3, object a4) { return create(typeName, a1, a2, a3, a4, NULL_ARG); }    
    public object create(string typeName, object a1, object a2, object a3, object a4, object a5) { return create(typeName, a1, a2, a3, a4, a5, NULL_ARG); }    
    public object create(string typeName, object a1, object a2, object a3, object a4, object a5, object a6) { return create(typeName, a1, a2, a3, a4, a5, a6, NULL_ARG); }    
    public object create(string typeName, object a1, object a2, object a3, object a4, object a5, object a6, object a7) { return create(typeName, a1, a2, a3, a4, a5, a6, a7, NULL_ARG); }    
    public object create(string typeName, object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8) { return create(typeName, a1, a2, a3, a4, a5, a6, a7, a8, NULL_ARG); }    
    public object create(string typeName, object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8, object a9) { return create(typeName, a1, a2, a3, a4, a5, a6, a7, a8, a9, NULL_ARG); }    
    public object create(string typeName, object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8, object a9, object a10) { return create(typeName, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, NULL_ARG); }    
    public object create(string typeName, object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8, object a9, object a10, object a11) { return create(typeName, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, NULL_ARG); }    
    public object create(string typeName, object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8, object a9, object a10, object a11, object a12) { return create(typeName, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, NULL_ARG); }    
    public object create(string typeName, object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8, object a9, object a10, object a11, object a12, object a13) { return create(typeName, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, NULL_ARG); }    
    public object create(string typeName, object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8, object a9, object a10, object a11, object a12, object a13, object a14) { return create(typeName, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, NULL_ARG); }    
    public object create(string typeName, object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8, object a9, object a10, object a11, object a12, object a13, object a14, object a15) { return create(typeName, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, NULL_ARG); }    
    public object create(string typeName, object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8, object a9, object a10, object a11, object a12, object a13, object a14, object a15, object a16)
    {
      object[] args = new[] {a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16}.Where(a => a != NULL_ARG).ToArray();
      return typeCreator.CreateType(typeName, args);
    }

    public string GetName()
    {
      return "create";
    }

    public string GetHelpDescription()
    {
      return "Creates and instance of any type (including static classes).  If the type's assembly is not loaded you must precede this call with a call to jish.assembly('assemblyFileName').";
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
