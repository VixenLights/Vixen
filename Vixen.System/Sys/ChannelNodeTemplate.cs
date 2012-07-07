using System.IO;
using Vixen.IO;
using Vixen.IO.Result;
using Vixen.Services;
using Vixen.Sys.Attribute;

namespace Vixen.Sys {
	class ChannelNodeTemplate {
		private const string DIRECTORY_NAME = "Template\\Channel";

		[DataPath]
		static public string Directory {
			get { return Path.Combine(Paths.DataRootPath, DIRECTORY_NAME); }
		}

		public const string Extension = ".cha";

		static public ChannelNodeTemplate Load(string filePath) {
			VersionedFileSerializer serializer = FileService.Instance.CreateChannelNodeTemplateSerializer();
			ISerializationResult result = serializer.Read(filePath);
			return (ChannelNodeTemplate)result.Object;
		}

		public ChannelNode ChannelNode;
	}
}
