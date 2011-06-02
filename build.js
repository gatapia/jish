// Use jish.exe to execute this file
var file = jish.create('System.IO.File');

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
  jish.process('build\\ILMerge.exe', '/targetplatform:v4 /target:exe /out:build\\jish\\tools\\jish.exe js.net.jish\\bin\\jish.exe js.net.jish\\bin\\js.net.dll js.net.jish\\bin\\Ninject.dll');

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
  jish.process('build\\NuGet.exe', 'Pack -OutputDirectory build\\js.net build\\js.net\\js.net.nuspec');
  jish.process('build\\NuGet.exe', 'Pack -OutputDirectory build\\jish build\\jish\\jish.nuspec');
};

function pushNuGetPackages() {
  runProcess('build\\NuGet.exe', 'Push build\\js.net\\js.net.0.0.1.nupkg');
  runProcess('build\\NuGet.exe', 'Push build\\jish\\jish.0.0.1.nupkg');
};