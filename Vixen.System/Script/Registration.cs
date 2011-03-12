using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//*** This whole class would be replaced if scripts become a module

namespace Vixen.Script {
	class Registration {
		static private Dictionary<string, ScriptTypeImplementation> _registeredScriptTypes = new Dictionary<string, ScriptTypeImplementation>();

		static public void Add(ScriptTypeImplementation scriptTypeImplementation) {
			if(scriptTypeImplementation.SkeletonFileGenerator.GetInterface(typeof(IScriptSkeletonGenerator).FullName) == null) {
				throw new Exception("Cannot register a script skeleton type of " + scriptTypeImplementation.SkeletonFileGenerator.Name);
			}
			if(scriptTypeImplementation.SkeletonFileGenerator.GetConstructor(System.Type.EmptyTypes) == null) {
				throw new Exception("Skeleton type " + scriptTypeImplementation.SkeletonFileGenerator.Name + " must provide an empty constructor.");
			}
			if(scriptTypeImplementation.ScriptFrameworkGenerator.GetInterface(typeof(IScriptFrameworkGenerator).FullName) == null) {
				throw new Exception("Cannot register a script framework type of " + scriptTypeImplementation.ScriptFrameworkGenerator.Name);
			}
			if(scriptTypeImplementation.ScriptFrameworkGenerator.GetConstructor(System.Type.EmptyTypes) == null) {
				throw new Exception("Framework generator type " + scriptTypeImplementation.SkeletonFileGenerator.Name + " must provide an empty constructor.");
			}
			if(scriptTypeImplementation.CodeProvider.GetInterface(typeof(ICodeProvider).FullName) == null) {
				throw new Exception("Cannot register a code provider type of " + scriptTypeImplementation.CodeProvider.Name);
			}
			if(scriptTypeImplementation.CodeProvider.GetConstructor(System.Type.EmptyTypes) == null) {
				throw new Exception("Code provider type " + scriptTypeImplementation.SkeletonFileGenerator.Name + " must provide an empty constructor.");
			}

			_registeredScriptTypes[scriptTypeImplementation.Language] = scriptTypeImplementation;
		}

		static public IScriptSkeletonGenerator GetScriptSkeletonGenerator(string scriptTypeKey) {
			ScriptTypeImplementation scriptImplementation;
			if(_registeredScriptTypes.TryGetValue(scriptTypeKey, out scriptImplementation)) {
				return Activator.CreateInstance(scriptImplementation.SkeletonFileGenerator) as IScriptSkeletonGenerator;
			}
			return null;
		}

		static public IScriptFrameworkGenerator GetScriptFrameworkGenerator(string scriptTypeKey) {
			ScriptTypeImplementation scriptImplementation;
			if(_registeredScriptTypes.TryGetValue(scriptTypeKey, out scriptImplementation)) {
				return Activator.CreateInstance(scriptImplementation.ScriptFrameworkGenerator) as IScriptFrameworkGenerator;
			}
			return null;
		}

		static public string GetScriptFileExtension(string scriptTypeKey) {
			ScriptTypeImplementation scriptImplementation;
			if(_registeredScriptTypes.TryGetValue(scriptTypeKey, out scriptImplementation)) {
				return scriptImplementation.FileExtension;
			}
			return null;
		}

		static public ICodeProvider GetCodeProvider(string scriptTypeKey) {
			ScriptTypeImplementation scriptImplementation;
			if(_registeredScriptTypes.TryGetValue(scriptTypeKey, out scriptImplementation)) {
				return Activator.CreateInstance(scriptImplementation.CodeProvider) as ICodeProvider;
			}
			return null;
		}

		static public string[] GetLanguages() {
			return _registeredScriptTypes.Keys.ToArray();
		}
	}
}
