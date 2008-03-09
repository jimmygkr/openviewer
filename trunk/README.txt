To build OpenViewer in Linux / OS X:

  ./runprebuild.sh
  nant

To build OpenViewer in Windows:

  ./runprebuild.bat
  Load OpenViewer.sln in Visual Studio and build the solution

The executable will be built in the `bin' directory.


To use the gtk# GUI (Windows):
  Install gtksharp-2.8.3-win32-0.0.exe from 
  http://forge.novell.com/modules/xfmod/project/?gtks-inst4win

To use the gtk# GUI (Linux):
  You have to install gtk-sharp (apt-get install gtk-sharp2-gapi)
  If OpenViewer.exe won't run you may have to add this line to
  /etc/mono/config:
    <dllmap dll="libglade-2.0-0.dll" target="libglade-2.0.so.0" />

To use the gtk# GUI (Mac OS X):
  You need to have the latest version of Mono installed
  (it comes with native GTK# in the install)
  If all the text in the UI comes up as squares, you may have to
  update your pango configuration, by running these commands in
  a terminal window:
    pango-querymodules > temp
    sudo mv temp /Library/Frameworks/Mono.framework/Versions/Current/etc/pango/pango.modules
