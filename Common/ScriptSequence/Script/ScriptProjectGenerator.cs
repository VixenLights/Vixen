using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Script;
using System.Linq;
using System.Xml.Linq;

namespace Common.ScriptSequence.Script
{
    public class ScriptProjectGenerator
    {
        bool isCSharp = true;
        ScriptSequence sequence;
        List<string> files = new List<string>();
        List<string> references = new List<string>();
        string path = string.Empty;
        public ScriptProjectGenerator(string[] _files, HashSet<string> assemblyReferences, ScriptSequence _sequence)
        {
            isCSharp = _sequence.Language.GetType().Name.Contains("CSharp");
            sequence = _sequence;
            files.AddRange(_files);
            references.AddRange(assemblyReferences.ToArray());

        }
        public void Generate()
        {
            XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";

            XElement project = new XElement(ns + "Project", new XAttribute("DefaultTargets", "Build"), new XAttribute("ToolsVersion", "4.0"));
            var pg1 = new XElement(ns + "PropertyGroup");
            pg1.Add(new XElement(ns + "Configuration", "Debug", new XAttribute("Condition", " '$(Configuration)' == '' ")));
            pg1.Add(new XElement(ns + "Platform", "x86", new XAttribute("Condition", " '$(Platform)' == '' ")));
            pg1.Add(new XElement(ns + "ProductVersion", "8.0.30703"));
            pg1.Add(new XElement(ns + "SchemaVersion", "2.0"));
            pg1.Add(new XElement(ns + "ProjectGuid", Guid.NewGuid()));
            pg1.Add(new XElement(ns + "OutputType", "Library"));
            pg1.Add(new XElement(ns + "AppDesignerFolder", "Properties"));
            pg1.Add(new XElement(ns + "RootNamespace", "Vixen.User"));
            pg1.Add(new XElement(ns + "AssemblyName", "VixenScriptTest"));
            pg1.Add(new XElement(ns + "TargetFrameworkVersion", "v4.0"));
            pg1.Add(new XElement(ns + "TargetFrameworkProfile", ""));
            pg1.Add(new XElement(ns + "FileAlignment", 512));
            project.Add(pg1);
            var pg2 = new XElement(ns + "PropertyGroup", new XAttribute("Condition", " '$(Configuration)|$(Platform)' == 'Debug|x86' "));
            pg2.Add(new XElement(ns + "PlatformTarget", "x86"));
            pg2.Add(new XElement(ns + "DebugSymbols", "true"));
            pg2.Add(new XElement(ns + "DebugType", "full"));
            pg2.Add(new XElement(ns + "Optimize", "false"));
            pg2.Add(new XElement(ns + "OutputPath", "bin\\Debug\\"));
            pg2.Add(new XElement(ns + "DefineConstants", "DEBUG;TRACE"));
            pg2.Add(new XElement(ns + "ErrorReport", "prompt"));
            pg2.Add(new XElement(ns + "WarningLevel", 4));
            project.Add(pg2);
            var pg3 = new XElement(ns + "PropertyGroup", new XAttribute("Condition", " '$(Configuration)|$(Platform)' == 'Debug|x86' "));
            pg3.Add(new XElement(ns + "PlatformTarget", "x86"));
            pg3.Add(new XElement(ns + "DebugType", "pdbonly"));
            pg3.Add(new XElement(ns + "Optimize", "true"));
            pg3.Add(new XElement(ns + "OutputPath", "bin\\Release\\"));
            pg3.Add(new XElement(ns + "DefineConstants", "TRACE"));
            pg3.Add(new XElement(ns + "ErrorReport", "prompt"));
            pg3.Add(new XElement(ns + "WarningLevel", 4));
            project.Add(pg3);
            project.Add(new XElement(ns + "PropertyGroup", new XElement(ns + "StartupObject")));
            var refs = new XElement(ns + "ItemGroup");
            foreach (var r in references)
            {
                var fi = new System.IO.FileInfo(r);
                var name = r;
                XElement reference = new XElement(ns + "Reference");
                if (fi.Exists)
                {
                    name = fi.Name.Replace(fi.Extension, "");
                    reference.Add(new XAttribute("Include", name));
                    reference.Add(new XElement(ns + "HintPath", fi.FullName));
                }
                else
                {
                    switch (r)
                    {
                        case "System.dll":
                        case "System.Core.dll":
                        case "System.Xml.dll":
                        case "System.Data.dll":
                        case "System.Drawing.dll":
                        case "Microsoft.CSharp.dll":
                            reference.Add(new XAttribute("Include", r.Replace(".dll", "")));
                            break;
                        default:
                            reference.Add(new XAttribute("Include", r));
                            break;
                    }


                }

                refs.Add(reference);
            }
            path = sequence.SourceFileDirectory;

            project.Add(refs);
            var items = new XElement(ns + "ItemGroup");
            foreach (var a in files)
            {
                var fi = new FileInfo(a);
                var newName = fi.Name;
                var newPath = fi.FullName;
                if (fi.Extension.ToLower().Equals(".tmp"))
                {
                    newName = "Main_Extended.cs";
                    newPath = Path.Combine(path, newName);
                    fi.CopyTo(newPath, true);
                }

                items.Add(new XElement(ns + "Compile", new XAttribute("Include", newName)));

            }

            project.Add(items);
            project.Add(new XElement(ns + "Import", new XAttribute("Project", "$(MSBuildToolsPath)\\Microsoft.CSharp.targets")));
            project.Save(System.IO.Path.Combine(path, "generated.csproj"));

        }
    }
}
