using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Vixen.Module;
using Vixen.IO;
using Vixen.IO.Xml;

namespace Vixen.Sys {
	class ModuleStore : IVersioned {
		private const int VERSION = 1;

		static public readonly string Directory = SystemConfig.Directory;
		public const string FileName = "ModuleStore.xml";
		static public readonly string DefaultFilePath = Path.Combine(Directory, FileName);

		public ModuleStore() {
			Data = new ModuleStaticDataSet();
		}

		public string LoadedFilePath { get; set; }

		public ModuleStaticDataSet Data { get; set; }

		public void Save() {
			string filePath = LoadedFilePath ?? DefaultFilePath;
			IWriter writer = new XmlModuleStoreWriter();
			writer.Write(filePath, this);
			LoadedFilePath = filePath;
		}

		public int Version {
			get { return VERSION; }
		}
	}
}
