using System.IO;
using Vixen.Services;
using Vixen.Sys.Attribute;

namespace Vixen.Sys {
	class ElementNodeTemplate {
		private const string DIRECTORY_NAME = "Template\\Element";

		[DataPath]
		static public string Directory {
			get { return Path.Combine(Paths.DataRootPath, DIRECTORY_NAME); }
		}

		public const string Extension = ".cha";

		public void Save(string filePath) {
			FileService.Instance.SaveElementNodeTemplateFile(this, filePath);
		}

		public ElementNode ElementNode;
	}
}
