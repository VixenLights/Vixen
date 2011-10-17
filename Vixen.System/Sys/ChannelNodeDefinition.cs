using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using Vixen.Sys;
using Vixen.IO;
using Vixen.IO.Xml;

namespace Vixen.Sys {
	public class ChannelNodeDefinition : IVersioned {
		private string _filePath;

		private const string DIRECTORY_NAME = "Template";
		private const string FILE_EXT = ".nod";
		private const int VERSION = 1;

		[DataPath]
		static private readonly string DefinitionDirectory = Path.Combine(Paths.DataRootPath, DIRECTORY_NAME);

		public ChannelNodeDefinition(string name = null, ChannelNode node = null) {
			if(name != null) {
				FilePath = name;
			}
			Node = node;
		}

		public ChannelNode Node { get; set; }

		public string FilePath {
			get { return _filePath; }
			set { _filePath = Path.Combine(DefinitionDirectory, Path.GetFileNameWithoutExtension(value) + FILE_EXT); }
		}

		public string DefinitionFileName {
			get { return Path.GetFileName(FilePath); }
		}

		public string Name {
			get { return Path.GetFileNameWithoutExtension(FilePath); }
		}

		public void Import(params string[] instanceNames) {
			foreach(string instanceName in instanceNames) {
				// Create a new tree instance.
				ChannelNode newNode = Node.Clone();
				newNode.Name = instanceName;
				foreach(ChannelNode channelNode in newNode.GetLeafEnumerator()) {
					// Create a channel and reference it in the node.
					channelNode.Channel = VixenSystem.Channels.AddChannel(channelNode.Name);

					// TODO: is this even needed? I changed the Vixen.Sys.Execution.AddChannel() call above to not
					// perform this following line, so explicitly added it here. However, it doesn't seem to make sense...
					// Vixen.Sys.Execution.Nodes.AddChannelLeaf(channelNode.Channel);
				}
				// Add the tree to the system trees.
				VixenSystem.Nodes.AddNode(newNode);
			}
		}

		static public IEnumerable<string> GetAllFileNames() {
			return Directory.GetFiles(DefinitionDirectory, "*" + FILE_EXT);
		}

		public void Save(string filePath) {
			if(string.IsNullOrWhiteSpace(filePath)) throw new InvalidOperationException("A name is required.");
			filePath = Path.Combine(DefinitionDirectory, Path.GetFileName(filePath));

			IWriter writer = new XmlChannelNodeTemplateWriter();
			writer.Write(filePath, this);
			this.FilePath = filePath;
		}

		public void Save() {
			Save(FilePath);
		}

		public void Delete() {
			// Delete this definition.
			if(File.Exists(DefinitionFileName)) {
				File.Delete(DefinitionFileName);
			}
		}

		public int Version {
			get { return VERSION; }
		}

		static public ChannelNodeDefinition Load(string filePath) {
			filePath = Path.Combine(DefinitionDirectory, Path.GetFileName(filePath));

			XmlChannelNodeTemplateReader reader = new XmlChannelNodeTemplateReader();
			ChannelNodeDefinition definition = reader.Read(filePath);

			return definition;
		}
	}
}
