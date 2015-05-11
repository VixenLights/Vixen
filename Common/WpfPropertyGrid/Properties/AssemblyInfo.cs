/*
 * Copyright © 2010, Denys Vuika
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;
using System.Runtime.CompilerServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("WPF PropertyEditorGrid")]
[assembly: AssemblyDescription("WPF PropertyEditorGrid Control")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("WPF PropertyEditorGrid")]
[assembly: AssemblyCopyright("Copyright © 2010-2011 Denys Vuika")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]
[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.MainAssembly)]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
  //(used if a resource is not found in the page, 
  // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
  //(used if a resource is not found in the page, 
  // app, or any theme specific resource dictionaries)
)]


// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("2010.5.*")]
[assembly: AssemblyVersion("2010.11.10")]
[assembly: AssemblyFileVersion("2010.11.10")]
[assembly: CLSCompliant(true)]

[assembly: XmlnsDefinition("http://schemas.denisvuyka.wordpress.com/wpfpropertygrid", "System.Windows.Controls.WpfPropertyGrid")]
[assembly: XmlnsDefinition("http://schemas.denisvuyka.wordpress.com/wpfpropertygrid", "System.Windows.Controls.WpfPropertyGrid.Controls")]
[assembly: XmlnsDefinition("http://schemas.denisvuyka.wordpress.com/wpfpropertygrid", "System.Windows.Controls.WpfPropertyGrid.Design")]

// http://msdn.microsoft.com/en-us/library/system.runtime.compilerservices.internalsvisibletoattribute.aspx
[assembly: InternalsVisibleTo("WpfPropertyGrid.UnitTests, PublicKey=00240000048000009400000006020000002400005253413100040000010001004da09578b80bee870f7d2419e782b0e9b817e2c2cd8864f642212da0efafd446cea165f9bd4714b8e1dcaa7420b9f2ad0efa0b41d0e6e8a5928d8b8c97c5734aab7c7e810573646cc34e3b0ab74d62e9adb57da175f9bca9bdc13658b0536fed45fc96dd284e789ce06ccf7746f2298d8e04c791cfd334ebb0bf9e31f7b5ced9")]