Getting Started on Windows

# Introduction #

A short explanation on how to get OpenViewer compiled and running on a windows machine.


# Details #

  1. You'll want a compiler something like  [Visual C# Express 2008](http://www.microsoft.com/express/vcsharp/) which is free , Visual Studio 2008 or Visual Studio 2005.
  1. If not installed. Install the compiler of your choice. Beware, The project files that will be automatically created are only tested with Visual Studio.
  1. You'll want to get a [GTK# runtime](http://sourceforge.net/project/showfiles.php?group_id=74626&package_id=223067) as well. 2.10.3 works.
  1. If not installed, install the GTK# runtime now.
  1. You'll also want to get the most recent source from the svn at http://openviewer.org/svn/openviewer/trunk. If you don't have it already, a good client for [Subversion](http://subversion.tigris.org/) is [TortoiseSVN](http://tortoisesvn.net/downloads).
  1. After that is downloaded, if you are using Visual C# Express Edition 2008 or Visual Studio 2008, run the runprebuild2008.bat file. If you are using Visual Studio 2005 use runprebuild.bat.
  1. In the bin directory open the OpenViewer.ini.example file. Comment out the `rendering_engine = ogredotnet` line with a semicolon (;) and uncomment the `rendering_engine = mogre` line by removing the semicolon. Save it as OpenViewer.ini
  1. Load the Openviewer.sln and build the project.