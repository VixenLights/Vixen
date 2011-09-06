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

		private const string USER_DATA_FILE = "UserData.xml";
		private const int VERSION = 2;

		public UserData() {
			ModuleData = new ModuleStaticDataSet();
			Identity = Guid.NewGuid();
		}

		public string FilePath { get; set; }

		public Guid Identity { get; set; }

		public ModuleStaticDataSet ModuleData { get; set; }

		public IEnumerable<OutputChannel> Channels { get; set; }

		public IEnumerable<ChannelNode> Nodes { get; set; }

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
