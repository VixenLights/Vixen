NSIS <project folder>\Installer\Vixen-installer.nsi installer script build notes:

Features in installer:
Detection of .net 4.0 framework, if target computer does not have .net 4.0 installed the installer will attempt to install it from MS.
Logging is enabled.  A log file is generated in the install target directory (vixen3-install.log)


Release notes dialog, and Run read me that launches at the end of install.

Release notes dialog source:
<project folder>\Release\releasenotes.txt

Read me popup at the end of install:
<project folder>\Release\changlog.txt

============================
How to build the setup.exe to install Vixen:

Installer is based on NSIS 2.46.

1) Download and install NSIS 2.46.
http://nsis.sourceforge.net/Download

2) Copy the NSIS prereq's which are located in:  
<project folder>\Installer\Contrib\Includes
<project folder>\Installer\Contrib\Plugins

To:
<project folder>\Installer\Contrib\Includes\*.* to %Program files%\NSIS\Include
<project folder>\Installer\Contrib\Plugins\*.* to %Program files%\NSIS\Plugins

The NSIS installer script (<project folder>\Installer\Vixen-installer.nsi ) is going to assume the release binaries are in: 

<project folder>\Installer
<project folder>\Release
<project folder>\Release\releasenotes.txt must exist.
<project folder>\Release\changlog.txt must exist.

3) Change and update the build number in <project folder>\Installer\Vixen-installer.nsi 
Change the:

	!define PRODUCT_VERSION "X.X.X.X" 

To the release build of your choice.  ie: !define PRODUCT_VERSION "3.1.1" 

4) Build the installer.

Run the %Program Files%\NSIS\makensis.exe <project folder>\installer\Vixen-installer.nsi to create the install setup.exe
Vixen-x.x.x-setup.exe will be built in the <project folder>\Release directory.

