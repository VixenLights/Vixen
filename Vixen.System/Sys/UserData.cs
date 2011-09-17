using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Vixen.Sys;
using Vixen.Module;
using Vixen.IO;
using Vixen.IO.Xml;

//UserData
//  DataDirectory (may or may not exist, exists only in binary branch copy)
//  Identity
//  ModuleData
//  Channels
//  RootNodes

namespace Vixen.Sys {
	class UserData : IVersioned {
		private string _alternateDataPath;
		private IEnumerable<Channel> _channels;
		private IEnumerable<ChannelNode> _nodes;

		private const string USER_DATA_FILE = "UserData.xml";
		private const int VERSION = 2;

		public UserData() {
			Identity = Guid.NewGuid();
			ModuleData = new ModuleStaticDataSet();
		}

		public string FilePath { get; set; }

		public Guid Identity { get; set; }

		public ModuleStaticDataSet ModuleData { get; set; }

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
			string filePath = FilePath ?? Path.Combine(Paths.DataRootPath, USER_DATA_FILE);
			IWriter writer = new XmlUserDataWriter();
			writer.Write(filePath, this);
			FilePath = filePath;
		}

		public int Version {
			get { return VERSION; }
		}
	}
}
