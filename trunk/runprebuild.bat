@REM TODO: change "" in this .cs file to a string containing the SVN revision number
copy OpenViewer\Application\VersionInfo.cs.template OpenViewer\Application\VersionInfo.cs

bin\Prebuild.exe /target nant
bin\Prebuild.exe /target vs2005
