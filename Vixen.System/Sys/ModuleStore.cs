using System.IO;
using Vixen.IO.Result;
using Vixen.Module;
using Vixen.IO;
using Vixen.Services;

namespace Vixen.Sys {
	class ModuleStore {
		static public readonly string Directory = SystemConfig.Directory;
		public const string FileName = "ModuleStore.xml";
		static public readonly string DefaultFilePath = Path.Combine(Directory, FileName);

		public ModuleStore() {
			Data = new ModuleStaticDataSet();
		}

		public string LoadedFilePath { get; set; }

		public ModuleStaticDataSet Data { get; set; }

		public void Save() {
			VersionedFileSerializer serializer = FileService.Instance.CreateModuleStoreSerializer();
			string filePath = LoadedFilePath ?? DefaultFilePath;
			serializer.Write(this, filePath);
		}

		static public ModuleStore Load(string filePath) {
			VersionedFileSerializer serializer = FileService.Instance.CreateModuleStoreSerializer();
			ISerializationResult result = serializer.Read(filePath);
			return (ModuleStore)result.Object;
		}
	}
}
