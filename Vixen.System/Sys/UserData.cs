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
//  ModuleData
//  Channels
//  RootNodes

namespace Vixen.Sys {
	class UserData : IVersioned {
		private string _alternateDataPath;

		private const string USER_DATA_FILE = "UserData.xml";
		private const int VERSION = 1;

		public UserData() {
			ModuleData = new ModuleStaticDataSet();
		}

		public string FilePath { get; private set; }

		public ModuleStaticDataSet ModuleData { get; set; }

		public IEnumerable<OutputChannel> Channels { get; set; }

		public IEnumerable<ChannelNode> Nodes { get; set; }

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
			// It is up to whatever uses this data to make sure to commit their data objects
			// to the data set.
			string filePath = Path.Combine(Paths.DataRootPath, USER_DATA_FILE);
			IWriter writer = new XmlUserDataWriter();
			writer.Write(filePath, this);
		}

		public int Version {
			get { return VERSION; }
		}
	}
}
