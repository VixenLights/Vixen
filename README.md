![GitHub release (with filter)](https://img.shields.io/github/v/release/Vixenlights/Vixen?logo=GitHub)
![GitHub release (with filter)](https://img.shields.io/github/v/release/Vixenlights/Vixen?filter=DevBuild*&logo=GitHub&label=pre-release)
[![download](https://img.shields.io/badge/download-release-blue.svg)](https://www.vixenlights.com/download/release-build/)
[![download](https://img.shields.io/badge/download-development-green.svg)](https://www.vixenlights.com/download/develop-build/)
[![download](https://img.shields.io/badge/bugs-tracker-blue.svg)](http://bugs.vixenlights.com)


<img src="Assets/Vixen3-Logo.png" width=600 />

Vixen is a full featured sequencer for producing animated light shows. It provides all the software tools necessary to create and run an animated light show sequenced to music. It supports typical DIY controllers as well as DMX, traditional and pixel lighting. It runs on the Windows platform under .NET Core.

<img src="Assets/Editor.png" width=600 />

## User and Developer Documentation

[Vixen Documentation](https://www.vixenlights.com/docs/)

## Installation
You can download and install Vixen via the self contained installer:

[Download Vixen 3](http://www.vixenlights.com/downloads/vixen-3-downloads/)

[Latest Release Notes](https://github.com/VixenLights/Vixen/blob/master/Release%20Notes.txt)

#### Bugs / Feature requests

[Bug Tracker](https://bugs.vixenlights.com)

## License

Vixen 3 is unlicensed and free to use.  
[License](https://github.com/VixenLights/Vixen/blob/master/License.txt)

#### WARRANTY DISCLAIMER

This software is provided 'as-is', without any express or implied warranty. 

#### Source Code

Vixen 3 is Open Source and source code is available on GitHub at https://github.com/VixenLights/Vixen/ under the licensing outlined above. 
#### Developer Info

This repository contains all the public source available for to build the Vixen 3 system. 

Vixen 3 is a modular application that allows for pluggable modules to be developed for it. It is written in C# and some small parts in C++. The code structure is as follows under the src folder: 

* /Vixen.Common - Common components used by the application and modules
* /Vixen.Modules - Modules developed and maintained by the core team, and other contributors
* /Vixen.Application - The main Vixen application
* /Vixen.Core - The Vixen core framework 
* /Vixen.Installer - Installer projects

For current developer setup information, see the [Developer](https://www.vixenlights.com/developer/) pages on the Vixen website.

Conventions for development:

- The assembly name should be the name of the module (eg. TimedSequenceEditor), and
  the default namespace should be "Module.\<ModuleType>.\<ModuleName>". For
  example, Modules.Editor.TimedSequenceEditor.
 
  
- The build output directory should be relative to the solution directory, in a
  'Release' directory release builds and a Debug folder for Debug builds. We no longer actively support x86 builds.
  It will also depend on the type of module. For example:
  
  Vixen Modules (Release):              $(SolutionDir)\Release\Output\Module.ModuleType.ModuleName\
  Vixen Common assemblies (Release):    $(SolutionDir)\Release\Output\
  Vixen Applications (Release):         $(SolutionDir)\Release\Output

- Assembly names are handled by the Directory.Build.Props file for each module type.
  
- To reference the Vixen project (or any other projects that are needed), make sure you
  add a 'project reference', and not a "normal" reference (to the binary DLL). This will help compatibility for other developers when used in different locations. References to projects should be set so they do not copy local. This avoids assembly loader issues with multiple copies. Under Properties of the reference.
  
  * Copy Local : No
  * Include Assets: None

- NuGet packages follow the same principle as Project References. You should include the package in the Common area and allow the libraries to be deployed in that path. Then in the local project the NuGet package is added but is set not to copy the assets locally by setting the following in the properties of the library.

    * Exclude Assets : None
      
- Tabs vs. Spaces, and other formatting: Tabs are preferred, and general formatting standards are followed. An editorconfig file in the src root controls the code styles. However, please try to review all your own changes before committing, to ensure you are not making large changes to unrelated sections of code (eg. changing formatting in a file, or whitespace, etc.). Commits like this may be rejected. If larger reformats are desired, include them in their own commits noted as such to distingush them from logic changes. See [Visual Studio Setup](https://www.vixenlights.com/developer/visual-studio/) for more info.

- ### Powered by
[![JetBrains logo.](https://resources.jetbrains.com/storage/products/company/brand/logos/jetbrains.svg)](https://jb.gg/OpenSource) 

