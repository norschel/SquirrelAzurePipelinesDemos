$ErrorActionPreference = 'Stop';
$toolsDir     = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$fileLocation = Join-Path $toolsDir 'Setup.msi'

#Based on Msi
$packageArgs = @{
  packageName   = $env:ChocolateyPackageName
  softwareName  = 'squirreldemoapp Machine-Wide*'
  file          = $fileLocation
  fileType      = 'msi'
  silentArgs    = "/qn /norestart /l*v `"$env:TEMP\$env:ChocolateyPackageName.$env:ChocolateyPackageVersion.log`""
  validExitCodes= @(0,1641,3010)
  url           = ""
  checksum      = '__ChocoPackageChecksum__'
  checksumType  = 'sha256'
  url64bit      = ""
  checksum64    = ''
  checksumType64= 'sha256'
  destination   = $toolsDir
  #installDir   = "" # passed when you want to override install directory - requires licensed editions 1.9.0+
}

Install-ChocolateyInstallPackage @packageArgs

<#
== MSI Properties ==
These are the PROPERTIES of the MSI, some of which you can add or change to the silent args or add as package parameters
Note: This only captures what ends up in the MSI Property/AppSearch tables and is not guaranteed to cover all properties.


NETFRAMEWORK45=**Property found in SecureCustomProperties**
#>

