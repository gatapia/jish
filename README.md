# js.net - JavaScript the .Net way #

## Overview ##
js.net provides several tools for working with JavaScript in a '.Net' kind of 
way:

- A wrapper around V8:  Include js.net.dll in your project and you can run 
 JavaScript straight from your .Net programs
- A command line interface and JavaScript file interpreter for running 
 JavaScript straight from your console (Jish).
- A set of unit testing bindings for different JavaScript frameoworks that
 allows you tu run your JavaScript tests straight from Visual Studio or your
 favourite CI tools.

For full details see the [wiki](https://github.com/gatapia/js.net/wiki/)

## Using js.net in your code ##
To use js.net simply add a reference to the js.net.dll in your project and
initialise the Engine.

Example:
  
    using (IEngine engine = new new JSNetEngine()) {
      Console.WriteLine("Answer Is: " + engine.Run("1 + 1"));
    }

## Jish ##
Jish.exe is a command line shell interpreter to run JavaScript scripts that 
allows .Net framework integration.  Jish can also be used to run JavaScript 
scripts files.

## Running Jish
Running 'jish.exe' will yield the interactive shell.

    jish.exe
    > 1 + 1
    2
    > .exit

## Built-in Jish commands
All inbuilt Jish commands start with a '.' character.  Jish commands cannot be 
mixed on the same line with other JavaScript commands.  If Jish commands are run 
as part of an input file they are executed prior to execution of any other 
command.  Commands included in Jish are:

* .help - Shows a help description of all other built in Jish commands
* .process - Executes the command in a separate Process.
* .create - Creates an instance of an object (including static classes) and 
  stores it in the specified global name.
* .clear - Break, and also clear the local context.
* .break - Cancels the execution of a multi-line command.
* .exit - Exit Jish.
* .assembly - Loads a .Net assembly in preparation for .static calls.

## Extending Jish
There are 3 main extension points to Jish.  

### JavaScript Modules
The first way you can extend Jish is by creating a JavaScript extension files 
that will be available to all your Jish scripts. 

Simply create a 'modules' directory next to your 'jish.exe' file.  This 
directory will be parsed for all `.js` files and they will be loaded 
into your Jish environment.

### ICommand (Special Commands)
The special commands implement the ICommand interface.  ICommand(s) have 
certain charasteristics which may not be immediately obvious.

* ICommand(s) have access to IJishInterpreter which allows all ICommand(s) to 
  run additional JavaScript files, load and set globals and integrate into the 
  built in `.help` command.
* ICommand(s) get run before any other JavaScript command, regardless where they 
  appear on the JavaScript file.
* ICommand(s) only accept simple primitive inputs.
* ICommand(s) cannot return any values into the JavaScript environment
* ICommand(s) integrate into Jish's `.help` system
* ICommand(s) are invoked by calling the command prefixed by a `.`. 
  I.e. `.commandname`

### IInlineCommand 
The IInlineCommand(s) extend the JavsScript environment by adding a type to the 
  global namespace.

* IInlineCommand(s) must have a non-embedded namespace (cannot contain '.'s)
* IInlineCommand(s) cannot execute other scripts, or set/get globals
* IInlineCommand(s) can return any primative type to the JavaScript environment.
    
## Unit Testing
One of js.net's primary and most stable feature is JavaScript unit testing 
support (including tests that rely on the DOM).  Current supported frameworks 
are:
- [Closure](http://closure-library.googlecode.com/svn/docs/closure_goog_testing_jsunit.js.html)
- [qUnit](http://docs.jquery.com/Qunit)
- [jsUnit](http://www.jsunit.net/)
- [Jasmine](http://pivotal.github.com/jasmine/)
- [JSCoverage](http://siliconforks.com/jscoverage/)

To unit test your JavaScript simply follow these steps:

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

        using (ICoverageAdapter adapter = JSNet.JSCoverage(JSNet.ClosureLibrary(basejsfile)))
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