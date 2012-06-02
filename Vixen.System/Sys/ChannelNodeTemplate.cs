using System.IO;
using Vixen.Sys.Attribute;

namespace Vixen.Sys {
	class ChannelNodeTemplate {
		private const string DIRECTORY_NAME = "Template\\Channel";

		[DataPath]
		static public string Directory {
			get { return Path.Combine(Paths.DataRootPath, DIRECTORY_NAME); }
		}

		public const string Extension = ".cha";

		public ChannelNode ChannelNode;
	}
}
