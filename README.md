# jish - JavaScript the .Net way #

## Overview ##
jish / js.net provides several tools for working with JavaScript in a '.Net' 
kind of way:

- A wrapper around V8:  Include js.net.dll in your project and you can run 
 JavaScript straight from your .Net programs
- A command line interface and JavaScript file interpreter for running 
 JavaScript straight from your console (Jish).
- A set of unit testing bindings for different JavaScript frameoworks that
 allows you tu run your JavaScript tests straight from Visual Studio or your
 favourite CI tools.

For full details see the [wiki](https://github.com/gatapia/js.net/wiki/)

## Getting Started
The best way to get started is to download the source:

    git clone git://github.com/gatapia/js.net.git

That way you can send me fixes ;)

Otherwise just download one of the following:

* [Jish - JavaScript Interactive SHell](https://github.com/gatapia/js.net/raw/master/build/jish.exe.zip)
* [js.net - Embed JavaScript in Your .Net Projects](https://github.com/gatapia/js.net/raw/master/build/js.net.dll.zip)
* [Both](https://github.com/gatapia/js.net/raw/master/build/both.zip)
* [Installer](https://github.com/gatapia/js.net/raw/master/build/jish.msi)

Note: The installer just 'unzips' the 'Both' package into a specified folder.

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
There are two type of built in commands available to Jish Inline Commands and
Console Commands.  The main difference is that Console command are there
to help you using the shell console, commands like .help, .exit, etc.  

Inline commands can be run inboth console and interpreted mode (running 
script files). And are your window into the .Net framework.

All Console Jish commands start with a '.' character and are only 
available if running in console mode.  Jish commands cannot be 
mixed on the same line with other JavaScript commands.  Commands included in 
Jish are:

    Jish Help
    =========
    
    Console Commands
    
    .break:
    	Cancels the execution of a multi-line command.
    	Arguments: ()
    
    .clear:
    	Break, and also clear the local context.
    	Arguments: ()
    
    .exit:
    	Exit Jish.
    	Arguments: ()
    
    
    Inline Commands
    
    jish.assembly:
    	Loads an assembly into the Jish 'context'. You can now
    		jish.create(<typeNames>) types from this assembly.
    	Arguments: (assemblyFileNameOrAssemblyName)
    
    jish.closure:
    	Loads google closure library environment.
    	Arguments: (baseJsPath)
    
    jish.create:
    	Creates and instance of any type (including static classes).  If the
    		type's assembly is not loaded you must precede this call with a
    		call to jish.assembly('assemblyFileName').
    	Arguments: (typeName, param object[] args)
    
    jish.process:
    	Executes the command in a separate Process.
    	Arguments: (command, arguments?)
    
    

## Extending Jish
There are 3 main extension points to Jish.  

### JavaScript Modules
The first way you can extend Jish is by creating a JavaScript extension files 
that will be available to all your Jish scripts. 

Simply create a 'modules' directory next to your 'jish.exe' file.  This 
directory will parse all `.js` files and they will be loaded into your Jish 
environment.  This directory is also where you can drop any additional dll's
that you want loaded into the context.  They will be parsed for implementations
if IInlineCommands and ICommands also.

### js.net.jish.InlineCommand.IInlineCommand 
The IInlineCommand(s) extend the JavsScript environment by adding a type to the 
  global namespace.  Inline commands are the main way of providing .Net framwork
  capabilities to your JavaScript scripts.

* IInlineCommand(s) must have a non-embedded namespace (cannot contain '.'s)
* IInlineCommand(s) cannot execute other scripts, or set/get globals
* IInlineCommand(s) can return any type to the JavaScript environment.
* IInlineCommand(s) intgrate into the built in `.help` command.

An example IInlineCommand follows, this is the jish.process command and is
used to spawn a process from your JavaScript environment.  Note: this is the
actual implementation code and is available every time you use jish 
by `jish.process('commandName', 'arguments_string')`:

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using js.net.Util;
    
    namespace js.net.jish.Command.InlineCommand
    {
      public class ProcessCommand : IInlineCommand
      {
        private readonly JSConsole console;
    
        public ProcessCommand(JSConsole console)
        {
          Trace.Assert(console != null);
    
          this.console = console;
        }
    
        public string GetName()
        {
          return "process";
        }
    
        public string GetHelpDescription()
        {
          return "Executes the command in a separate Process.";
        }
    
        public IEnumerable<CommandParam> GetParameters()
        {
          CommandParam a1 = new CommandParam { Name = "command" };
          CommandParam a2 = new CommandParam { Name = "arguments", Null = true};
          return new[] { a1, a2 };
        }
    
        public string GetNameSpace()
        {
          return "jish";
        }
    
        public int process(string command, string arguments = null) 
        {
          Trace.Assert(!String.IsNullOrWhiteSpace(command));
    
          using (var process = new Process
                          {
                            StartInfo =
                              {
                                FileName = command,
                                Arguments = arguments,
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true
                              }
                          })
          {
            process.Start();
            string err = process.StandardError.ReadToEnd();
            string output = process.StandardOutput.ReadToEnd();
            
            if (!String.IsNullOrWhiteSpace(err)) console.error(err);
            if (!String.IsNullOrWhiteSpace(output)) console.log(output);
            
            process.WaitForExit();
            
            return process.ExitCode;
          }
        }
      }
    }

### js.net.jish.Command.IConsoleCommand (Console Commands)
The console commands implement the ICommand interface.  ICommand(s) have 
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


## Running multi-file scripts / Google Closure Support
jish comes with out of the box support for Google Closure Library.  This means
that if you want to levarage the huge utility library in closure or use
multi file projects then this is easy.

Download the [closure library] http://code.google.com/closure/library/docs/gettingstarted.html) if you have not already done so and then Jish away:

    jish.closure('path/to/your/closure/base.js');      
    jish.addClosureDeps('path/to/any/additional/deps.js');    
    // Require your files or any closure library file
    goog.require('jish.test.main');    
    // Go!!
    var main = new jish.test.main();
    console.log(main.callUtilMethod());

    
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
        Process p = 
          Process.Start("jscoverage.exe", "src\ instrumented\").WaitForExit();

        using (ICoverageAdapter adapter = 
          JSNet.JSCoverage(JSNet.ClosureLibrary(basejsfile)))
        {        
          adapter.LoadSourceFile(@"instrumented\instrumentedSourceFile.js"); 
          ICoverageResults results = 
            adapter.RunCoverage(@"src\tests\sourceFileTests.js");

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

## An example jish JavaScript file (1) - Windows Forms
This example is a very simple winforms app.


    jish.assembly('js.net.jish/bin/System.Drawing.dll')
    jish.assembly('js.net.jish/bin/System.Windows.Forms.dll')

    var app = jish.create('System.Windows.Forms.Application');
    var form = jish.create('System.Windows.Forms.Form');
    var lbl = jish.create('System.Windows.Forms.Label');
    form.Text = lbl.Text = 'Hello Jish!!!';
    lbl.Location = jish.create('System.Drawing.Point', 50, 50);
    form.Controls.Add(lbl);

    app.Run(form);

## An example jish JavaScript file (2) - Build Script
This is Jish's very own build file.  You can find the latest version of this 
file [in github](https://github.com/gatapia/js.net/blob/master/build.js).

    
        
    // Use jish.exe to execute this file. Takes two optional command line 
    // instructions: 
    //    updatever:  Increments the build numbers on the NuGet files
    //    push:       Publishes NuGet packages
    
    // Load additional assemblies into the context.  This dll includes the 
    // build.zip command used in createZipBundles() below;
    jish.assembly('js.net.test.module/bin/js.net.test.module.dll');
    
    // Create a handle on the File static class.  Yes, jish.create even creates
    // static handles.
    var file = jish.create('System.IO.File');
    
    run(); // Go!!!!
    
    function run() {
      createZipBundles();
    
      if (args.indexOf('updatever') >= 0) {
        updateVersionNumberInNuGetConfigs();
      } else {
        console.log('Not updating version numbers. To update versions please ' +
          'execute with "updatever" argument');
      }
      packNuGetPacakges();
      if (args.indexOf('push') >= 0) {
        pushNuGetPackages();
      } else {
        console.log('Not "pushing". To push please execute with "push" argmuent');
      }  
    };
    
    function createZipBundles() {
      build.zip('build\\jish.exe.zip', ['build\\jish\\tools\\jish.exe']);
      build.zip('build\\js.net.dll.zip', ['build\\js.net\\lib\\js.net.dll']);  
      build.zip('build\\both.zip', ['build\\js.net.dll.zip', 'build\\jish.exe.zip']);  
      console.log('Successfully created the zip bundles');
    };
    
    function copyFile(from, to) {  
      file.Copy(from, to, true);
    };
    
    function updateVersionNumberInNuGetConfigs() {
      updateVersionOnConfig('build\\js.net\\js.net.nuspec');
      updateVersionOnConfig('build\\jish\\jish.nuspec');
    };
    
    function packNuGetPacakges() {  
      jish.process('build\\NuGet.exe', 
        'Pack -OutputDirectory build\\js.net build\\js.net\\js.net.nuspec');
      jish.process('build\\NuGet.exe', 
        'Pack -OutputDirectory build\\jish build\\jish\\jish.nuspec');
    };
    
    function pushNuGetPackages() {  
      var name = 'build\\js.net\\js.net.' + 
        getVersionNumberFromConfig('build\\js.net\\js.net.nuspec') + '.nupkg';
      
      console.log('Publishing ' + name);
      jish.process('build\\NuGet.exe', 'Push ' + name);
    
      name = 'build\\jish\\jish.' + 
        getVersionNumberFromConfig('build\\jish\\jish.nuspec') + '.nupkg';
      
      console.log('Publishing ' + name);
      jish.process('build\\NuGet.exe', 'Push ' + name);
    };
    
    function getVersionNumberFromConfig(configFile) {
      var contents = file.ReadAllText(configFile);
      var version = contents.substring(contents.indexOf('<version>') + 9);
      version = version.substring(0, version.indexOf('<'));
      return version;
    };
    
    function updateVersionOnConfig(configFile) {
      var version = getVersionNumberFromConfig(configFile);
      version = updateVersionNumber(version);
      setVersionNumberOnConfig(configFile, version);
    };
    
    function updateVersionNumber(oldVersion) {
      var pre = oldVersion.substring(0, oldVersion.lastIndexOf('.') + 1);
      var buildNum = 
        parseInt(oldVersion.substring(oldVersion.lastIndexOf('.') + 1), 10);
      buildNum++;
      return pre + buildNum.toString();
    };
    
    function setVersionNumberOnConfig(file, newv) {
      var contents = file.ReadAllText(file);
      var newContents = contents.substring(0, contents.indexOf('<version>') + 9);
      newContents += newv;
      newContents += contents.substring(contents.indexOf('</version>'));
      file.WriteAllText(file, newContents);
    
      console.log('Updated the version on [' + file + '] to [' + newv + ']');
    };