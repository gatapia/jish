# js.net #

## Overview ##
js.net provides a mechanism for running JavaScript from .net code.  It also 
allows for JavaScript unit testing to be performed directly from Visual Studio.
Currently js.net supports several JavaScript unit testing frameworks:

- [Closure](http://closure-library.googlecode.com/svn/docs/closure_goog_testing_jsunit.js.html)
- [qUnit](http://docs.jquery.com/Qunit)
- [jsUnit](http://www.jsunit.net/)
- [Jasmine](http://pivotal.github.com/jasmine/)
- [JsCoverage](http://siliconforks.com/jscoverage/)

## Unit Testing
One of js.net's primary and most stable feature is JavaScript unit testing 
support (including tests that rely on the DOM).  To unit test your JavaScript 
simply follow these steps:

- Change the target framework of your unit test project to x86
  (Project Properties -> Build -> Playform Target -> x86)
- Add a reference to the js.net.dll
- Add your unit test like this (This is using NUnit style code but you can use 
  anything you want):

Example:

    [TestFixture] public class JavaScriptTests {
      [Test] public void TestSingleFile()
      { 
        // Initialise your adapter using the helpers in JSNet utility class
        using (ITestAdapter adapter = JSNet.QUnit(pathToTheQunitJsFile)) 
        {
          // Run your test file
          ITestResults results = 
            adapter.RunTest(pathToTestJsOrHtmlFile); 
              
          // Assert no failures
          Assert.AreEqual(0, results.Failed.Count(), results.ToString());
          Assert.AreEqual(22, results.Passed.Count(), results.ToString());
        }            
      }
    }

## Unit Testing Multiple Files
Unit testing multiple files is just as simple as testing a single file.  So once
you have added the js.net.dll reference to your project simply write some code
like this:

Example:

    [TestFixture] public class JavaScriptTests {
      [Test] public void TestAllFiles()
      { 
        // Run all tests
        string[] files = GetTestSuiteFiles();
        TestSuiteRunner runner = 
          JSNet.ClosureLibraryTestSuiteRunner(baseJsFile);
        runner.AddGlobalSourceFile(depsJsFile);
        TestSuiteResults results = runner.TestFiles(files);

        // Assert no failures
        Assert.AreEqual(0, results.Failed.Count(), results.ToString());
        Assert.Greater(results.Passed.Count(), 0, results.ToString());
        }            
      }
    }

## Coverage
Running coverage on your tests is just as simple as running the tests 
themselves.

Example:

    [TestFixture] public class JavaScriptTests {
      [Test] public void TestRunCoverageWithProperAdapter()
      {
        // Code must be instrumented first (use the native instrumenter).
        Process p = Process.Start("jscoverage.exe", "src\ instrumented\").WaitForExit();

        using (ICoverageAdapter adapter = JSNet.JsCoverage(JSNet.ClosureLibrary(basejsfile)))
        {        
          adapter.LoadSourceFile(@"instrumented\instrumentedSourceFile.js"); 
          ICoverageResults results = adapter.RunCoverage(@"src\tests\sourceFileTests.js");         

          // Assert tests passes as per normal
          Assert.AreEqual(0, results.Failed.Count(), results.ToString());
          Assert.AreEqual(4, results.Passed.Count(), results.ToString());

          // Assert coverage is as expected
          Assert.AreEqual(1, results.FilesCount);
          Assert.AreEqual(5, results.Statements);
          Assert.AreEqual(5, results.Executed);
          // Assert we have 100% coverage
          Assert.AreEqual(100.0m, results.CoveragePercentage);

          // Assert coverage for individual files within the test
          IFileCoverageResults sourceCoverage = results.FileResults.First();
          Assert.AreEqual("jscoverage_source.js", sourceCoverage.FileName);
          Assert.AreEqual(5, sourceCoverage.Statements);
          Assert.AreEqual(5, sourceCoverage.Executed);
          Assert.AreEqual(100.0m, sourceCoverage.CoveragePercentage);                  
        } 
      }
    }

## Shout Outs
This project would not be possible without JavaScript.Net:
[http://javascriptdotnet.codeplex.com/](http://javascriptdotnet.codeplex.com/).

John Resig's awesome 
[Env.js](http://jqueryjs.googlecode.com/svn/trunk/jquery/build/runtest/env.js) 
provides the DOM support that all test adapters levarage.

## About [PicNet](http://www.picnet.com.au)
PicNet does software development for large companies in Australia.  We 
specialise in .Net development and serious JavaScript development.

## About Guido Tapia ##
[Guido Tapia](guido@tapia.com.au) is the software development manager for 
[PicNet](http://www.picnet.com.au) and he is a really awesome guy.

Feel free to email:
[guido@tapia.com.au](guido@tapia.com.au)

## License
BSD, see license.txt for full license