using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using js.net.jish.IL;
using js.net.Util;

namespace js.net.tests.jish.TypeILWrapper
{
  public abstract class AbstractTypeILWrapperTest {
    
    static AbstractTypeILWrapperTest()
    {
      Trace.Listeners.Clear();
      Trace.Listeners.Add(new ExceptionTraceListener());
    }

    private readonly net.jish.IL.TypeILWrapper wrapper = new net.jish.IL.TypeILWrapper();
    private readonly Type typeToWrap;

    protected AbstractTypeILWrapperTest(Type typeToWrap) { this.typeToWrap = typeToWrap; }

    protected IEnumerable<MethodInfo> GetExpandedMethods(string realMethodName, string wrappedMethodName = null, object instace = null)
    {
      object wrapped = wrapper.CreateWrapperFromType(typeToWrap, 
        new [] {new MethodToProxify(typeToWrap.GetMethod(realMethodName), instace)});
      return wrapped.GetType().GetMethods().Where(mi => mi.Name.Equals(wrappedMethodName ?? realMethodName)).OrderBy(mi => mi.GetParameters().Length);
    }
  }
}