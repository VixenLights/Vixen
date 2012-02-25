using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Vixen.Sys;
using System.Reflection;
using System.CodeDom.Compiler;
using Vixen.Module.Sequence;
using Vixen.Module.Script;

namespace Vixen.Script {
	class ScriptHostGenerator {
		private List<string> _errors = new List<string>();
		private string[] _standardReferences = {
            "System.dll",
			"System.Core.dll",
			"Microsoft.CSharp.dll", // Required for dynamic.
            "Vixen.dll"
        };

		static public readonly string UserScriptNamespace = "Vixen.User";

		public IUserScriptHost GenerateScript(ScriptSequence sequence) {
			List<string> files = new List<string>();
			string fileName;

			Assembly assembly = null;
			string nameSpace = UserScriptNamespace;

			try {
				// Emit the T4 template to be compiled into the assembly.
				fileName = Path.GetTempFileName();
				ScriptModuleManagement manager = Modules.GetManager<IScriptModuleInstance, ScriptModuleManagement>();
				IScriptFrameworkGenerator frameworkGenerator = manager.GetFrameworkGenerator(sequence.Language);

				string fileContents = frameworkGenerator.Generate(nameSpace, sequence.ClassName);
				File.WriteAllText(fileName, fileContents);
				files.Add(fileName);

				// Add the user's source files.
				foreach(SourceFile sourceFile in sequence.GetAllSourceFiles()) {
					fileName = Path.Combine(Path.GetTempPath(), sourceFile.Name);
					File.WriteAllText(fileName, sourceFile.Contents);
					files.Add(fileName);
				}

				// Compile the sources.
				assembly =_Compile(files.ToArray(), sequence);
			} finally {
				// Delete the temp files.
				foreach(string tempFile in files) {
					File.Delete(tempFile);
				}
			}

			if(assembly != null) {
				// Get the generated type.
				Type type = assembly.GetType(string.Format("{0}.{1}", nameSpace, sequence.ClassName));
				if(type != null) {
					// Create and return an instance.
					IUserScriptHost scriptHost = Activator.CreateInstance(type) as IUserScriptHost;
					scriptHost.Sequence = sequence;
					return scriptHost;
				}
			}

			return null;
		}

		public IEnumerable<string> Errors {
			get { return _errors; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="files"></param>
		/// <returns>The file name of the compiled assembly.</returns>
		private Assembly _Compile(string[] files, ScriptSequence sequence) {
			// Assembly references come in two flavors:
			// 1. Framework assemblies -- need only the file name.
			// 2. Other assemblies -- need the qualified file name.
			List<string> assemblyReferences = new List<string>();
			_errors.Clear();

			assemblyReferences.Add(VixenSystem.AssemblyFileName);
			assemblyReferences.AddRange(sequence.FrameworkAssemblies);
			assemblyReferences.AddRange(sequence.ExternalAssemblies);
			assemblyReferences.AddRange(_standardReferences);

			ScriptModuleManagement manager = Modules.GetManager<IScriptModuleInstance, ScriptModuleManagement>();
			IScriptCodeProvider codeProvider = manager.GetCodeProvider(sequence.Language);

			CompilerResults results = null;
			using(codeProvider) {
				CompilerParameters compilerParameters = new CompilerParameters() {
					GenerateInMemory = true
					//IncludeDebugInformation = true //FOR TESTING ONLY! Leaves a file behind.
				};
				compilerParameters.ReferencedAssemblies.AddRange(assemblyReferences.ToArray());

				results = codeProvider.CompileAssemblyFromFile(compilerParameters, files);
			}

			// Get any errors.
			foreach(CompilerError error in results.Errors) {
				_errors.Add(string.Format("{0} [{1}]: {2}", Path.GetFileName(error.FileName), error.Line, error.ErrorText));
			}

			return results.Errors.HasErrors ? null : results.CompiledAssembly;
		}

		static public string Mangle(string str) {
			if(string.IsNullOrWhiteSpace(str)) throw new ArgumentException("Name cannot be empty.");

			List<char> chars = new List<char>();

			if(!char.IsLetter(str[0]) && str[0] != '_') {
				chars.Add('_');
			}

			foreach(char ch in str) {
				if(_IsValidSymbolChar(ch)) {
					chars.Add(ch);
				} else {
					chars.Add('_');
				}
			}

			return new string(chars.ToArray());
		}

		static private bool _IsValidSymbolChar(char ch) {
			return char.IsLetterOrDigit(ch) || ch == '_';
		}
	}
}
