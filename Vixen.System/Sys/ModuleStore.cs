using System.IO;
using Vixen.Module;
using Vixen.IO;

namespace Vixen.Sys {
	class ModuleStore {
		//private const int VERSION = 1;

		static public readonly string Directory = SystemConfig.Directory;
		public const string FileName = "ModuleStore.xml";
		static public readonly string DefaultFilePath = Path.Combine(Directory, FileName);

		public ModuleStore() {
			Data = new ModuleStaticDataSet();
		}

		public string LoadedFilePath { get; set; }

		public ModuleStaticDataSet Data { get; set; }

		public void Save() {
			FileSerializer<ModuleStore> serializer = SerializerFactory.Instance.CreateModuleStoreSerializer();
			string filePath = LoadedFilePath ?? DefaultFilePath;
			serializer.Write(this, filePath);
		}

		//public int Version {
		//    get { return VERSION; }
		//}
	}
}
