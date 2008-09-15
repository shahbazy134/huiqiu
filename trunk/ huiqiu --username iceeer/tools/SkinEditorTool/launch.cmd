@echo off

set EMPTYDRV=J:
set THISDIR=d:\shimmer\skintool

set DOMINO=q:/notes/bin/w32/bin
set JAVAKIT=D:/Progra~1/Java/j2re1.4.2/jdk

set PATH=.;%JAVAKIT%/bin;%DOMINO%;c:/winnt/system32;c:/winnt
set CLASSPATH=.;%DOMINO%/Notes.jar


REM 	Make sure we are at the root
if not exist %EMPTYDRV%\SkinEditor.java subst %EMPTYDRV% %THISDIR%
%EMPTYDRV%


REM	Run the demo
java SkinEditor D:/Domino/Data6/iNotes/Forms6.nsf
if not %ERRORLEVEL%==0 pause
