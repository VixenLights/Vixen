This contains all the publicly available content for the Vixen 3 system. At present,
this includes:

- common components used by the application and modules, in /Common
- modules developed and maintained by the core team, and other contributors, in /Modules
- the main Vixen application, in /Application

You should be able to open the VixenModules.sln solution and have it 'magically work'.
If you have any problems, please let us know; there's quite a few small workarounds
and gotchas used to get the modules building in two different solutions.

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

  If you're unsure, look at another existing project, and copy the OutputPath for both
  Debug and Release, eg.:
  <OutputPath>$(SolutionDir)\Output\Modules\Controller\</OutputPath>
  <OutputPath>$(SolutionDir)\Release\Modules\Controller\</OutputPath>
  
 
- To reference the vixen project, don't add a reference to the DLL; instead, edit the
  .csproj file, and add this line somewhere:
  
  <Import Project="$(SolutionDir)\commonProject.props" />
  
  This includes a custom file which references the Vixen assembly as appropriate for
  the solution it's been opened in: either a DLL reference, or a project reference.
  

- When adding any other references, make sure you turn off the "Copy Local" option;
  with it on, we end up with copies of DLLs all over the place on output. :-)


- Tabs vs. Spaces: Tabs are preferred; although it's not a religious thing. Try to
  keep it consistent within a file, though: it's annoying if there are commits that
  change one or two lines of content, with 100 lines changed because of whitespace.

