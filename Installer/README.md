
# Installer Build

## Overview

WIX is the current installer platform used to build an install package that can install the core dependencies and the msi with the Vixen application files. There are 2 projects that manage the creation of the installer. Vixen.Installer is the core project that builds the msi installer for the core Vixen application files. Vixen.DeployBundle is a project that creates a WIX Burn bundle to install the required dependencies of the .NET Runtimes and the C++ libraries. This has logic to determine if those need to be installed on the users machine and will install them if they are missing.

To build a setup installer for Vixen, msbuild can be utilized to automate the tasks. There are 3 distinct release types that the installer projects support.

* Test - These builds are used for off the cuff test builds that a developer may want to share with a user to try out a fix or some other new functionality before forammly commiting the changes to the tree and entering a development build.
* Dev - This is the conventional development build that is created after each change is merged to the tree.
* Prod - This is a formal release build for general consumption.

The solution will only build the installer projects when the Configuration is Deploy.

## MSBUILD Parameters

* Configuration - This can be one of 3 values. Debug, Release, Deploy. For the purposes of building a setup file, Deploy will be used.
* Platform - The build supports x86 and x64. We will primarily use x64.
* VIX_VERSION - This is the formal build number for install purposes. This will be of the form 3.9.4 for 3.9u4 Release or 0.0.2300 for dev build 2300 or 0.0.0 for Test builds which is the default. This will be the msi version number reported in Add/Remove Programs.
* ASSEMBLY_VERSION - This gets mapped to the the Version property in the build projects and will become the assembly version of the output. This should be of the form 0.0.0.0 major.minor.build.revision i.e 2.9.2300.2 for Release 3.9u4 build 2300 and for dev builds it would just be 0.0.2300.0. For test builds it should just be 0.0.0.0 which is the default.
* VIX_RELEASE_TYPE - This has one of 3 values. Test, Dev, and Prod. This allows for 3 types of builds to be supported and installed side by side. See above for usage.

## Examples

Test Build

msbuild -Restore Vixen.sln -m -t:Rebuild -p:Configuration=Deploy -p:Platform=x64 -p:VIX_RELEASE_TYPE=Test

Dev Build

msbuild -Restore Vixen.sln -m -t:Rebuild -p:Configuration=Deploy -p:Platform=x64 -p:VIX_VERSION="0.0.2456" -p:ASSEMBLY_VERSION="0.0.2456.0" -p:VIX_RELEASE_TYPE=Dev

Prod Build

msbuild -Restore Vixen.sln -m -t:Rebuild -p:Configuration=Deploy -p:Platform=x64 -p:VIX_VERSION="3.9.4" -p:ASSEMBLY_VERSION="3.9.2314.4" -p:VIX_RELEASE_TYPE=Prod
