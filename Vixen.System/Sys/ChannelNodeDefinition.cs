using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using Vixen.Common;
using Vixen.IO;

namespace Vixen.Sys {
	public class ChannelNodeDefinition : Definition {
		private ChannelNode _node;

		private const string DIRECTORY_NAME = "Template";
		private const string FILE_EXT = ".nod";

		[DataPath]
		static protected readonly string _channelNodeDefinitionDirectory = Path.Combine(Definition._definitionDirectory, DIRECTORY_NAME);

		private ChannelNodeDefinition() { }

		public ChannelNodeDefinition(string name, ChannelNode node) {
			_node = node;
			FilePath = Path.Combine(_channelNodeDefinitionDirectory, name + FILE_EXT);
		}

		public void Import(params string[] instanceNames) {
			foreach(string instanceName in instanceNames) {
				// Create a new tree instance.
				ChannelNode newNode = _node.Clone();
				newNode.Name = instanceName;
				foreach(ChannelNode channelNode in newNode.GetLeafEnumerator()) {
					// Create a channel and reference it in the node.
					channelNode.Channel = Vixen.Sys.Execution.AddChannel(channelNode.Name);

					// TODO: is this even needed? I changed the Vixen.Sys.Execution.AddChannel() call above to not
					// perform this following line, so explicitly added it here. However, it doesn't seem to make sense...
					// Vixen.Sys.Execution.Nodes.AddChannelLeaf(channelNode.Channel);
				}
				// Add the tree to the system trees.
				Vixen.Sys.Execution.Nodes.AddNode(newNode);
			}
		}

		static public IEnumerable<string> GetAllFileNames() {
			return Directory.GetFiles(_channelNodeDefinitionDirectory, "*" + FILE_EXT);
		}

		public void Save(string filePath) {
			if(string.IsNullOrWhiteSpace(filePath)) throw new InvalidOperationException("A name is required.");
			filePath = Path.Combine(_channelNodeDefinitionDirectory, Path.GetFileName(filePath));
			base._Save<ChannelNodeDefinitionWriter>(filePath);
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

		static public ChannelNodeDefinition Load(string filePath) {
			filePath = Path.Combine(_channelNodeDefinitionDirectory, Path.GetFileName(filePath));
			return Definition.Load<ChannelNodeDefinition, ChannelNodeDefinitionReader>(filePath);
		}

		static public XElement WriteXml(ChannelNodeDefinition value) {
			return ChannelNode.WriteXmlTemplate(value._node);
		}

		static public ChannelNodeDefinition ReadXml(XElement element) {
			ChannelNodeDefinition template = new ChannelNodeDefinition();
			template._node = ChannelNode.ReadXml(element);
			return template;
		}
	}
}
