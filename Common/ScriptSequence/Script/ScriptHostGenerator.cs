using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vixen.Module.Effect;
using Vixen.Sys;
using System.Reflection;
using Vixen.Module.Script;

namespace Common.ScriptSequence.Script
{
	internal class ScriptHostGenerator
	{
		private List<string> _errors = new List<string>();

		private string[] _standardReferences
		{
			get
			{
				List<string> retval = new List<string>();
				retval.AddRange(new string[]{                                   	
												"System.dll",
		                                       	"System.Drawing.dll",
		                                       	"System.Core.dll",
		                                       	"Vixen.dll",
		                                       	"Common\\Controls.dll",
		                                       	"Modules\\App\\ColorGradients.dll",
		                                       	"Modules\\App\\Curves.dll",
		                                       	"Modules\\Effect\\Candle.dll",
		                                       	"Modules\\Effect\\Chase.dll",
		                                       	"Modules\\Effect\\Nutcracker.dll",
		                                       	"Modules\\Effect\\Spin.dll",
		                                       	"Modules\\Effect\\Twinkle.dll",
		                                       	//"Modules\\Effect\\SetPosition.dll",
		                                       	"Microsoft.CSharp.dll" // Required for dynamic.
							  });
				//Paths to look for Effects and other items to reference in the Script generator
				var paths = new string[] { 
					System.IO.Path.Combine(Environment.CurrentDirectory, "Modules", "Property"),
					System.IO.Path.Combine(Environment.CurrentDirectory, "Modules", "Effect") 
				};
				paths.ToList().ForEach(path => {
					var dir = new System.IO.DirectoryInfo(path);
					retval.AddRange(dir.GetFiles("*.dll").Select(f => f.FullName));
			
				});

				return retval.ToArray();
			}
		}

		public static readonly string UserScriptNamespace = "Vixen.User";

		public IUserScriptHost GenerateScript(ScriptSequence sequence)
		{
			List<string> files = new List<string>();

			Assembly assembly;
			string nameSpace = UserScriptNamespace;

			try {
				// Emit the T4 template to be compiled into the assembly.
				string fileName = Path.GetTempFileName();

				string fileContents = sequence.Language.FrameworkGenerator.Generate(nameSpace, sequence.ClassName);
				File.WriteAllText(fileName, fileContents);
				files.Add(fileName);

				// Add the user's source files.
				foreach (SourceFile sourceFile in sequence.SourceFiles) {
					fileName = Path.Combine(Path.GetTempPath(), sourceFile.Name);
					File.WriteAllText(fileName, sourceFile.Contents);
					files.Add(fileName);
				}

				// Compile the sources.
				assembly = _Compile(files.ToArray(), _GetReferencedAssemblies(sequence), sequence.Language);
			} finally {
				// Delete the temp files.
				foreach (string tempFile in files) {
					File.Delete(tempFile);
				}
			}

			if (assembly != null) {
				// Get the generated type.
				Type type = assembly.GetType(string.Format("{0}.{1}", nameSpace, sequence.ClassName));
				if (type != null) {
					// Create and return an instance.
					return (IUserScriptHost)Activator.CreateInstance(type);
				}
			}

			return null;
		}

		private IEnumerable<string> _GetReferencedAssemblies(ScriptSequence sequence)
		{
			List<string> assemblyReferences = new List<string>();

			assemblyReferences.AddRange(sequence.FrameworkAssemblies);
			assemblyReferences.AddRange(sequence.ExternalAssemblies);
			assemblyReferences.AddRange(_GetEffectParameterAssemblies());
			assemblyReferences.AddRange(_standardReferences);
			assemblyReferences.Add(VixenSystem.AssemblyFileName);

			return assemblyReferences;
		}

		private IEnumerable<string> _GetEffectParameterAssemblies()
		{
			var effectParameterTypes =
				Vixen.Services.ApplicationServices.GetAll<IEffectModuleInstance>().SelectMany(x => x.Parameters.Select(y => y.Type));
			return effectParameterTypes.Select(x => x.Assembly.GetFilePath());
		}

		public IEnumerable<string> Errors
		{
			get { return _errors; }
		}

		/// <returns>The file name of the compiled assembly.</returns>
		private Assembly _Compile(string[] files, IEnumerable<string> assemblyReferences, IScriptModuleInstance language)
		{
			// Assembly references come in two flavors:
			// 1. Framework assemblies -- need only the file name.
			// 2. Other assemblies -- need the qualified file name.
			_errors.Clear();

			HashSet<string> uniqueAssemblyReferences = new HashSet<string>(assemblyReferences);
			// Remove .NET framework and GAC assemblies.
			uniqueAssemblyReferences.RemoveWhere(
				x => x.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.Windows), StringComparison.OrdinalIgnoreCase));

			ICompilerParameters compilerParameters = new ScriptCompilerParameters {
				GenerateInMemory = true,
				//IncludeDebugInformation = true
			};
			compilerParameters.ReferencedAssemblies.AddRange(uniqueAssemblyReferences.ToArray());

			ICompilerResults results = language.CodeProvider.CompileAssemblyFromFile(compilerParameters, files);

			// Get any errors.
			foreach (ICompilerError error in results.Errors) {
				_errors.Add(string.Format("{0} [{1}]: {2}", Path.GetFileName(error.FileName), error.Line, error.ErrorText));
			}

			return results.HasErrors ? null : results.CompiledAssembly;
		}

		public static string Mangle(string str)
		{
			if (string.IsNullOrWhiteSpace(str)) throw new ArgumentException("Name cannot be empty.");

			List<char> chars = new List<char>();

			if (!char.IsLetter(str[0]) && str[0] != '_') {
				chars.Add('_');
			}

			foreach (char ch in str) {
				if (_IsValidSymbolChar(ch)) {
					chars.Add(ch);
				} else {
					chars.Add('_');
				}
			}

			return new string(chars.ToArray());
		}

		private static bool _IsValidSymbolChar(char ch)
		{
			return char.IsLetterOrDigit(ch) || ch == '_';
		}
	}
}