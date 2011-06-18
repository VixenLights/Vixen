using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Vixen.Common;
using Vixen.Module;
using Vixen.IO;

//UserData
//  DataDirectory (may or may not exist, exists only in binary branch copy)
//  ModuleData
//  Channels
//  RootNodes

namespace Vixen.Sys {
	class UserData {
		private XElement _userData;

		private const string USER_DATA_FILE = "UserData.xml";
		private const string ELEMENT_ROOT = "UserData";
		private const string ELEMENT_MODULE_DATA = "ModuleData";
		private const string ELEMENT_DATA_DIRECTORY = "DataDirectory";
		private const string ELEMENT_CHANNELS = "Channels";
		private const string ELEMENT_NODES = "Nodes";

		public UserData() {
			string fileName;

			ModuleData = new ModuleDataSet();

			// Look for a user data file in the binary directory.
			fileName = Path.Combine(Paths.BinaryRootPath, USER_DATA_FILE);
			_userData = Helper.LoadXml(fileName);
			if(_userData != null) {
				XElement dataDirectory = _userData.Element(ELEMENT_DATA_DIRECTORY);
				if(dataDirectory != null) {
					if(Directory.Exists(dataDirectory.Value)) {
						// We have an alternate path and it does exist.
						Paths.DataRootPath = dataDirectory.Value;
					}
				}
			}

			// Setting DataRootPath is the way to get the data branch built, but we
			// don't want to do that until we have certainly determined the location
			// of their data branch.  Otherwise we would create multiple branches and
			// they will have an empty branch that is likely to raise questions.
			// SO...we may or may not have already set it above, so we are going to
			// set it to ensure the data branch exists.
			Paths.DataRootPath = Paths.DataRootPath;

			// Reset the expected location of the user data file.
			fileName = Path.Combine(Paths.DataRootPath, USER_DATA_FILE);

			// With the data directory settled, try to load data from it.
			if(File.Exists(fileName)) {
				_Load(fileName);
			} else {
				// Data file does not exist, create a new one.
				_userData = new XElement(ELEMENT_ROOT,
					new XElement(ELEMENT_MODULE_DATA),
					new XElement(ELEMENT_CHANNELS),
					new XElement(ELEMENT_NODES)
					);
				_userData.Save(fileName);
			}
		}

		public ModuleDataSet ModuleData { get; private set; }

		public void SetAlternateDataPath(string path) {
			Paths.DataRootPath = path;
			if(Paths.DataRootPath == path) {
				// Data root path is the path that we specified; the set
				// did not fail.
				_userData.SetElementValue(ELEMENT_DATA_DIRECTORY, path);
			}
		}

		public void Save() {
			// It is up to whatever uses this data to make sure to commit their data objects
			// to the data set.
			_SaveModuleData();
			_SaveChannels();
			_SaveBranchNodes();
			_userData.Save(Path.Combine(Paths.DataRootPath, USER_DATA_FILE));
		}

		private void _Load(string fileName) {
			if(File.Exists(fileName)) {
				// Load the content.
				_userData = Helper.LoadXml(fileName);
			}
		}

		public IEnumerable<OutputChannel> LoadChannels() {
			XElement parentNode = _userData.Element(ELEMENT_CHANNELS);
			IEnumerable<OutputChannel> channels = parentNode.Elements().Select(x => OutputChannel.ReadXml(x));
			return channels;
		}

		private void _SaveChannels() {
			XElement parentNode = _userData.Element(ELEMENT_CHANNELS);
			parentNode.RemoveAll();
			IEnumerable<XElement> elements = Vixen.Sys.Execution.Channels.Select(x => OutputChannel.WriteXml(x));
			parentNode.Add(elements);
		}

		public void LoadModuleData() {
			XElement moduleData = _userData.Element(ELEMENT_MODULE_DATA);
			if(moduleData != null) {
				ModuleData.Deserialize(moduleData.ToString());
			}
		}

		private void _SaveModuleData() {
			ModuleData.SaveToParent(_userData.Element(ELEMENT_MODULE_DATA));
		}

		public IEnumerable<ChannelNode> LoadBranchNodes() {
			// Any references to non-existent channels will be pruned by this operation.
			XElement parentNode = _userData.Element(ELEMENT_NODES);
			IEnumerable<ChannelNode> rootNodes = parentNode.Elements().Select(x => ChannelNode.ReadXml(x));
			return rootNodes;
		}

		private void _SaveBranchNodes() {
			XElement parentNode = _userData.Element(ELEMENT_NODES);
			parentNode.RemoveAll();
			IEnumerable<XElement> elements = Vixen.Sys.Execution.Nodes.RootNodes.Select(x => ChannelNode.WriteXml(x));
			parentNode.Add(elements);
		}
	}
}
