
# Installer Build

## Overview

WIX is the current installer platform used to build an install package that can install the core dependencies and the msi with the Vixen application files. There are 2 projects that manage the creation of the installer. Vixen.Installer is the core project that builds the msi installer for the core Vixen application files. Vixen.DeployBundle is a project that creates a WIX Burn bundle to install the required dependencies of the .NET Runtimes and the C++ libraries. This has logic to determine if those need to be installed on the users machine and will install them if they are missing.

To build a setup installer for Vixen, msbuild can be utilized to automate the tasks. There are 3 distinct release types that the installer projects support.

* Test - These builds are used for off the cuff test builds that a developer may want to share with a user to try out a fix or some other new functionality before forammly commiting the changes to the tree and entering a development build.
* Development - This is the historical "DevBuild" build that is created after each change is merged to the tree.
* Production - This is a formal release build for general consumption.

The solution will only build the installer projects when the Configuration is Deploy.

## MSBUILD Parameters

* Configuration - This can be one of 3 values. Debug, Release, Deploy. Debug and Release targets will primarily be used for development in Visual Studio and will not target the setup projects. For the purposes of building a setup file, Deploy will be used and will follow Release, but include the rwo setup projects in the build.
* Platform - The build supports x86 and x64. We will primarily use x64.
* App_Version - This is the formal application build number for install purposes. It will be of the form 3.9.4 for 3.9u4 Release or 0.0.2300 for DevBuild 2300 or 0.0.0 for Test builds which is the default. It will be the msi version number reported in Add/Remove Programs. It is currently only used in the Vixen.Installer and Vixen.BundleDeploy projects.
* Assembly_Version - This gets mapped to the the Version property in the msbuild projects and will become the assembly version of the output. This should be of the form 0.0.0.0 major.minor.build.revision i.e 2.9.2300.2 for Release 3.9u4 build 2300 and for dev builds it would just be 0.0.2300.0. For test builds it should just be 0.0.0.0 which is the default.
* Environment - This has one of 3 values. Test, Development, and Production. This allows for 3 types of builds to be supported and installed side by side. See above for usage.
* Platform - This will be the target architecture of the build. x64 or x86.

## Outputs

Setup files will be produced in the Release\Setup\{Environment} folder under the root of the project. The setup files will have naming conventions matching the patterns below. The Environment and App_Version parameters will drive the naming. The output will also have some other files like the core msi that will be in a localized folder along with some debugging files. The main setup exe is all that is needed for the end user.

* Test - Vixen-Test-0.0.0.0-Setup-64bit.exe
* Development - Vixen-DevBuild-0.0.2300.0-Setup-64bit.exe
* Production - Vixen-3.9.4-Setup-64bit.exe

## Examples

Test Build

msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Deploy -p:Platform=x64

Development Build

msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Deploy -p:Platform=x64 -p:App_Version="0.0.2456" -p:Assembly_Version="0.0.2456.0" -p:Environment=Development

Production Build

msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Deploy -p:Platform=x64 -p:App_Version="3.9.4" -p:Assembly_Version="3.9.2314.4" -p:Environment=Production
