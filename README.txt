This contains all the publicly available content for the Vixen 3 system. At present,
this includes:

- common components used by the application and modules, in /Common
- modules developed and maintained by the core team, in /Modules


Conventions for module development:

- The assembly name should be the name of the module (eg. TimedSequenceEditor), and
  the default namespace should be "VixenModules.<ModuleType>.<ModuleName>". For
  example, VixenModules.Editor.TimedSequenceEditor.
  
- The build output directory should be relative to the solution directory, in an
  'Output' directory for debug builds, and a 'Release' directory for release builds.
  It will also depend on the type of module. For example:
  
  Vixen Modules (Debug):              $(SolutionDir)\Output\Modules\<ModuleType>\
  Vixen Modules (Release):            $(SolutionDir)\Release\Modules\<ModuleType>\
  Vixen Common assemblies (Debug):    $(SolutionDir)\Output\Common\
  Vixen Common assemblies (Release):  $(SolutionDir)\Release\Common\
  Vixen Applications (Debug):         $(SolutionDir)\Output\
  Vixen Applications (Release):       $(SolutionDir)\Release\

  (this allows the VixenModules solution to be used within another solution -- ie.
  the main Vixen solution -- and still output build data appropriately.)
  
  Note: the $(SolutionDir) text will need to be edited in the .csproj file directly
        (ie. outside of Visual Studio), as VS escapes the '$', '(', and ')' characters.



