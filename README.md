# js.net #

## Overview ##
js.net provides a mechanism for running JavaScript from .net code.  It also 
allows for JavaScript unit testing to be performed directly from Visual Studio.
Currently js.net supports several JavaScript unit testing frameworks:

- [Closure](http://closure-library.googlecode.com/svn/docs/closure_goog_testing_jsunit.js.html)
- [qUnit](http://docs.jquery.com/Qunit)
- [jsUnit](http://www.jsunit.net/)
- [Jasmine](http://pivotal.github.com/jasmine/)

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
<<<<<<< HEAD
          TestResults results = 
            adapter.RunTest("pathToYourTestingJsOrHtmlFile.js"); 
=======
          TestResults results = adapter.RunTest(@"pathToYourTestingJsOrHtmlFile.js"); 
>>>>>>> 5ea0e433e4e7cb85124b40cf9d2f0ff4f89f0ce4
              
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