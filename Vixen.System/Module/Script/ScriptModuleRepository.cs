using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Script;

namespace Vixen.Module.Script {
	class ScriptModuleRepository : GenericModuleRepository<IScriptModuleInstance> {
		private Dictionary<string, Guid> _languageIndex = new Dictionary<string, Guid>();

		override public void Add(Guid id) {
			IScriptModuleDescriptor descriptor = Modules.GetDescriptorById<IScriptModuleDescriptor>(id);
			
			// Check the type in the descriptor to make sure they implement the correct
			// interfaces.
			List<string> invalidTypes = new List<string>();
			if(!descriptor.SkeletonGenerator.ImplementsInterface(typeof(IScriptSkeletonGenerator))) {
				invalidTypes.Add("Skeleton generator");
			}
			if(!descriptor.FrameworkGenerator.ImplementsInterface(typeof(IScriptFrameworkGenerator))) {
				invalidTypes.Add("Framework generator");
			}
			if(!descriptor.CodeProvider.ImplementsInterface(typeof(IScriptCodeProvider))) {
				invalidTypes.Add("Code provider");
			}

			if(invalidTypes.Count > 0) {
				VixenSystem.Logging.Error("Script module \"" + descriptor.TypeName + "\" has the following incorrect types: " + string.Join(", ", invalidTypes) + "." + Environment.NewLine + "The language will be unavailable.");
			} else {
				_languageIndex[descriptor.Language] = id;
			}
		}

		override public void Remove(Guid id) {
			IScriptModuleDescriptor descriptor = Modules.GetDescriptorById<IScriptModuleDescriptor>(id);
			_languageIndex.Remove(descriptor.Language);
		}

		//public IScriptModuleInstance Get(string language) {
		//    Guid typeId;
		//    if(_languageIndex.TryGetValue(language, out typeId)) {
		//        return Get(typeId);
		//    }
		//    return null;
		//}
		public IScriptSkeletonGenerator GetSkeletonGenerator(string language) {
			Guid typeId;
			if(_languageIndex.TryGetValue(language, out typeId)) {
				IScriptModuleDescriptor descriptor = Modules.GetDescriptorById<IScriptModuleDescriptor>(typeId);
				return Activator.CreateInstance(descriptor.SkeletonGenerator) as IScriptSkeletonGenerator;
			}
			return null;
		}

		public IScriptFrameworkGenerator GetFrameworkGenerator(string language) {
			Guid typeId;
			if(_languageIndex.TryGetValue(language, out typeId)) {
				IScriptModuleDescriptor descriptor = Modules.GetDescriptorById<IScriptModuleDescriptor>(typeId);
				return Activator.CreateInstance(descriptor.FrameworkGenerator) as IScriptFrameworkGenerator;
			}
			return null;
		}

		public IScriptCodeProvider GetCodeProvider(string language) {
			Guid typeId;
			if(_languageIndex.TryGetValue(language, out typeId)) {
				IScriptModuleDescriptor descriptor = Modules.GetDescriptorById<IScriptModuleDescriptor>(typeId);
				return Activator.CreateInstance(descriptor.CodeProvider) as IScriptCodeProvider;
			}
			return null;
		}

		public string GetFileExtension(string language) {
			Guid typeId;
			if(_languageIndex.TryGetValue(language, out typeId)) {
				IScriptModuleDescriptor descriptor = Modules.GetDescriptorById<IScriptModuleDescriptor>(typeId);
				return descriptor.FileExtension;
			}
			return null;
		}

		public string[] GetLanguages() {
			return _languageIndex.Keys.ToArray();
		}
	}
}
