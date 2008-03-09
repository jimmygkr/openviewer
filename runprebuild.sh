#!/bin/sh

set_versioninfo()
{
    if [ -n "`which svn`" -a -n "`which grep`" -a -n "`which perl`" ]; then
        revision=`svn info | grep ^Revision: | perl -p -i -e "s/Revision: //"`
        perl -p -i -e "s/\"\"/\" (revision ${revision})\"/" OpenViewer/Application/VersionInfo.cs
    fi
}

# copy from a template so we can svn:ignore the .cs file
cp OpenViewer/Application/VersionInfo.cs.template OpenViewer/Application/VersionInfo.cs

set_versioninfo

mono bin/Prebuild.exe /target nant
mono bin/Prebuild.exe /target vs2005

## TODO: Remove this section once nant can build MogrePlugin
if [ -n "`which perl`" ]; then
    perl -p -i -e 's/(.*MogrePlugin.dll.build.*)/<!-- $1 -->/' OpenViewer.build
else
    echo "Warning: Could not disable MogrePlugin in nant build. Compiling may fail." >& 2
    echo "         Install perl to disable building MogrePlugin" >& 2
fi
