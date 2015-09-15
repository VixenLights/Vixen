This contains all the publicly available content for the Vixen 3 system. At present,
this includes:

- common components used by the application and modules, in /Common
- modules developed and maintained by the core team, and other contributors, in /Modules
- the main Vixen application, in /Application
- the vixen framework, in /Vixen.System
- files to build the installer, in /Installer

You should be able to open the Vixen.sln solution and have it 'magically work'.
If you have any problems, please let us know; there's quite a few small workarounds
and gotchas used to get the modules building in two different solutions.

Conventions for module development:

- The assembly name should be the name of the module (eg. TimedSequenceEditor), and
  the default namespace should be "VixenModules.<ModuleType>.<ModuleName>". For
  example, VixenModules.Editor.TimedSequenceEditor.
 
  
- The build output directory should be relative to the solution directory, in an
  'Release' directory for x86 release builds, and a 'Release64' directory for x64 release builds.
  It will also depend on the type of module. For example:
  
  Vixen Modules (Release):              $(SolutionDir)\Release\Modules\<ModuleType>\
  Vixen Modules (Release64):            $(SolutionDir)\Release64\Modules\<ModuleType>\
  Vixen Common assemblies (Release):    $(SolutionDir)\Release\Common\
  Vixen Common assemblies (Release64):  $(SolutionDir)\Release64\Common\
  Vixen Applications (Release):         $(SolutionDir)\Release\
  Vixen Applications (Release64):       $(SolutionDir)\Release64\

  Note: the $(SolutionDir) text will need to be edited in the .csproj file directly
        (ie. outside of Visual Studio), as VS escapes the '$()' macros.

  If you're unsure, look at another existing project, and copy the OutputPath for both
  Release and Release64, eg.:
  <OutputPath>$(SolutionDir)\Release\Modules\Controller\</OutputPath>
  <OutputPath>$(SolutionDir)\Release64\Modules\Controller\</OutputPath>
  
 
- To reference the vixen project (or any other projects that are needed), make sure you
  add a 'project reference', and not a "normal" reference (to the binary DLL). This will
  help compatibility for other developers when used in different locations.
  

- When adding any other references, make sure you turn off the "Copy Local" option;
  with it on, we end up with copies of DLLs all over the place on output. :-)  If you're
  unsure, after building the solution, do a 'clean solution', and everything should be
  removed from the build output directory.


- Tabs vs. Spaces, and other formatting: Tabs are preferred, any other preferred formatting
  can be added here. However, please try to review all your own changes before committing,
  to ensure you are not making large changes to unrelated sections of code (eg. changing
  formatting in a file, or whitespace, etc.). Commits like this may be rejected.

