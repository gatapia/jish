// Use jish.exe to execute this file

// Load required .Net statics and objects
.create('System.IO.File', 'file');
// We are creating and reusing an instance of process because using .process
// means that it will be executed at the start of the file which is not 
// acceptable here
.create('System.Diagnostics.Process, System', 'process');

// Run!
updateNuGetBuildFiles();
if (args.indexOf('updatever') >= 0) {
  updateVersionNumberInNuGetConfigs();
} else {
  console.log('Not updating version numbers. To update versions please execute with "updatever" argument');
}
packNuGetPacakges();
if (args.indexOf('push') >= 0) {
  pushNuGetPackages();
} else {
  console.log('Not "pushing". To push please execute with "push" argmuent');
}

function updateNuGetBuildFiles() {
  // jish
  copyFile('js.net.jish\\bin\\Noesis.Javascript.dll', 'build\\jish\\tools\\Noesis.Javascript.dll');
  runProcess('build\\ILMerge.exe', '/internalize /v4 /out:build\\jish\\tools\\jish.exe js.net.jish\\bin\\jish.exe js.net.jish\\bin\\js.net.dll js.net.jish\\bin\\Ninject.dll');

  // js.net
  copyFile('js.net.jish\\bin\\Noesis.Javascript.dll', 'build\\js.net\\lib\\Noesis.Javascript.dll');
  copyFile('js.net.jish\\bin\\js.net.dll', 'build\\js.net\\lib\\js.net.dll');
};

function copyFile(from, to) {  
  file.Copy(from, to, true);
};

function updateVersionNumberInNuGetConfigs() {
  // TODO.  Needs also to update version in pushNuGetPackages
};

function packNuGetPacakges() {  
  runProcess('build\\NuGet.exe', 'Pack -OutputDirectory build\\js.net build\\js.net\\js.net.nuspec');
  runProcess('build\\NuGet.exe', 'Pack -OutputDirectory build\\jish build\\jish\\jish.nuspec');
};

function pushNuGetPackages() {
  runProcess('build\\NuGet.exe', 'Push build\\js.net\\js.net.0.0.1.nupkg');
  runProcess('build\\NuGet.exe', 'Push build\\jish\\jish.0.0.1.nupkg');
};

function runProcess(command, args) {
  process.StartInfo.FileName = command;
  process.StartInfo.Arguments = args;
  process.StartInfo.UseShellExecute = false;
  process.StartInfo.RedirectStandardOutput = true;
  process.StartInfo.RedirectStandardError = true;
  process.Start();
  var err = process.StandardError.ReadToEnd();
  var output = process.StandardOutput.ReadToEnd();
  if (err) console.log(err);
  if (output) console.log(output);
  process.WaitForExit();
  if (process.ExitCode != 0)
  {
    throw new Error('Process ' + commandAndArgs + ' exited with code: ' + process.ExitCode);
  }
}