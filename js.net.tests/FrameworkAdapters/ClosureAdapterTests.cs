﻿using js.net.TestAdapters;
using NUnit.Framework;

namespace js.net.tests.FrameworkAdapters
{
  [TestFixture] public class ClosureAdapterTests
  {
    private ITestAdapter ctx;
    [SetUp] public void SetUp()
    {
      const string basejsfile = @"J:\dev\Projects\Misc\closure-library\closure\goog\base.js";

      ctx = JSNet.ClosureLibrary(basejsfile);
    }

    [TearDown] public void TearDown()
    {
      ctx.Dispose();
    }

    [Test] public void NonClosureCode()
    {      
      Assert.AreEqual(2, ctx.GetFrameworkAdapter().Run("1 + 1", "ClosureAdapterTests.NonClosureCode"));
    }

    [Test] public void SimpleBaseJSDependantScript()
    {
      Assert.AreEqual("function", ctx.GetFrameworkAdapter().Run("typeof (goog.bind)", "ClosureAdapterTests.SimpleBaseJSDependantScript"));
    }

    [Test] public void AccrossMultipleSessions()
    {
      ctx.GetFrameworkAdapter().Run("var x = 1", "ClosureAdapterTests.TestSetModifyAndGetGlobal");
      Assert.AreEqual(2, ctx.GetFrameworkAdapter().Run("x + 1", "ClosureAdapterTests.AccrossMultipleSessions"));
    }

    [Test] public void CallingArrayFunction()
    {
      Assert.AreEqual(10, ctx.GetFrameworkAdapter().Run(
@"
[0,1,2,3,4].reduce(function(previousValue, currentValue, index, array){
  return previousValue + currentValue;
});
", "ClosureAdapterTests.CallingArrayFunction"));
    }

    [Test] public void UsingAComplexNonClosureFunction()
    {      
      Assert.AreEqual(10, ctx.GetFrameworkAdapter().Run(
@"
  goog.require('goog.array');
  var arr = [0,1,2,3,4];
  goog.array.reduce(arr, function(previousValue, currentValue, index, array){
    return previousValue + currentValue;
  }, 0);
", "ClosureAdapterTests.UsingAComplexNonClosureFunction"));
    }

    [Test] public void GoogRequireString()
    {
      ctx.GetFrameworkAdapter().Run("console.log('typeof(goog): ' + typeof(goog));", ":");

      ctx.GetFrameworkAdapter().Run("goog.require('goog.string');", "ClosureAdapterTests.GoogRequireString");
    }    
  }
}
