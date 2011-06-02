// Use jish.exe to execute this file
var file = jish.create('System.IO.File');

function run() {
  updateNuGetBuildFiles();

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

run(); // Go!!!!

function updateNuGetBuildFiles() {
  // jish
  copyFile('js.net.jish\\bin\\Noesis.Javascript.dll', 
    'build\\jish\\tools\\Noesis.Javascript.dll');
  jish.process('build\\ILMerge.exe', '/targetplatform:v4 /target:exe ' + 
    '/out:build\\jish\\tools\\jish.exe js.net.jish\\bin\\jish.exe ' +
    'js.net.jish\\bin\\js.net.dll js.net.jish\\bin\\Ninject.dll');

  // js.net
  copyFile('js.net.jish\\bin\\Noesis.Javascript.dll', 
    'build\\js.net\\lib\\Noesis.Javascript.dll');
  copyFile('js.net.jish\\bin\\js.net.dll', 'build\\js.net\\lib\\js.net.dll');
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