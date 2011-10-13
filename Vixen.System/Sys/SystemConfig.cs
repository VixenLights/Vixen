using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Vixen.Module;
using Vixen.IO;
using Vixen.IO.Xml;

namespace Vixen.Sys {
	class SystemConfig : IVersioned {
		private string _alternateDataPath;
		private IEnumerable<Channel> _channels;
		private IEnumerable<ChannelNode> _nodes;

		private const int VERSION = 2;

		[DataPath]
		static public readonly string Directory = Path.Combine(Paths.DataRootPath, "SystemData");
		public const string FileName = "SystemConfig.xml";
		static public readonly string DefaultFilePath = Path.Combine(Directory, FileName);

		public SystemConfig() {
			Identity = Guid.NewGuid();
		}

		public string LoadedFilePath { get; set; }

		public Guid Identity { get; set; }

		// Doing it this way means that Channels and Nodes will never be null.
		public IEnumerable<Channel> Channels {
			get {
				if(_channels == null) {
					_channels = new Channel[0];
				}
				return _channels;
			}
			set { _channels = value; }
		}

		public IEnumerable<ChannelNode> Nodes {
			get {
				if(_nodes == null) {
					_nodes = new ChannelNode[0];
				}
				return _nodes;
			}
			set { _nodes = value; }
		}

		public bool IsContext { get; set; }

		public string AlternateDataPath {
			get { return _alternateDataPath; }
			set {
				Paths.DataRootPath = value;
				if(Paths.DataRootPath == value) {
					// Data root path is the path that we specified; the set
					// did not fail.
					_alternateDataPath = value;
				}
			}
		}

		public void Save() {
			string filePath = LoadedFilePath ?? Path.Combine(Paths.DataRootPath, FileName);
			IWriter writer = new XmlSystemConfigWriter();
			writer.Write(filePath, this);
			LoadedFilePath = filePath;
		}

		public int Version {
			get { return VERSION; }
		}
	}
}
