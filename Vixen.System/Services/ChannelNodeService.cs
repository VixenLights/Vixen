using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.IO;
using Vixen.Rule;
using Vixen.Sys;

namespace Vixen.Services {
	public class ChannelNodeService {
		static private ChannelNodeService _instance;

		private ChannelNodeService() {
		}

		public static ChannelNodeService Instance {
			get { return _instance ?? (_instance = new ChannelNodeService()); }
		}

		public ChannelNode CreateSingle(ChannelNode parentNode, string name = null, bool createChannel = false, int index = -1) {
			name = name ?? "Unnamed";

			ChannelNode channelNode = VixenSystem.Nodes.AddNode(name);
			VixenSystem.Nodes.AddChildToParent(channelNode, parentNode, index);

			Channel channel = createChannel ? _CreateChannel(name) : null;
			channelNode.Channel = channel;
			VixenSystem.Channels.AddChannel(channel);

			return channelNode;
		}

		public ChannelNode[] CreateMultiple(ChannelNode parentNode, int count, bool createChannel = false) {
			return Enumerable.Range(0, count).Select(x => CreateSingle(parentNode, null, createChannel)).ToArray();
		}

		public ChannelNode ImportTemplateOnce(string templateFileName, ChannelNode parentNode) {
			FileSerializer<ChannelNodeTemplate> serializer = SerializerFactory.Instance.CreateChannelNodeTemplateSerializer();
			var result = serializer.Read(templateFileName);
			if(!result.Success) return null;

			ChannelNode channelNode = result.Object.ChannelNode;
			VixenSystem.Nodes.AddChildToParent(channelNode, parentNode);
			return channelNode;
		}

		public ChannelNode[] ImportTemplateMany(string templateFileName, ChannelNode parentNode, int count) {
			return Enumerable.Range(0, count).Select(x => ImportTemplateOnce(templateFileName, parentNode)).NotNull().ToArray();
		}

		public void Rename(ChannelNode channelNode, string name) {
			channelNode.Name = name;
		}

		public void Rename(IEnumerable<ChannelNode> channelNodes, INamingRule namingRule) {
			ChannelNode[] channelNodeArray = channelNodes.ToArray();
			string[] names = namingRule.GenerateNames(channelNodeArray.Length);

			for(int i=0; i<channelNodeArray.Length; i++) {
				Rename(channelNodeArray[i], names[i]);
			}
		}

		public void Patch(ChannelNode channelNode, IPatchingRule patchingRule) {
			Patch(channelNode.AsEnumerable(), patchingRule);
		}

		public void Patch(IEnumerable<ChannelNode> channelNodes, IPatchingRule patchingRule) {
			Channel[] channelArray = channelNodes.Select(x => x.Channel).NotNull().ToArray();
			ControllerReferenceCollection[] destinations = patchingRule.GenerateControllerReferenceCollections(channelArray.Length).ToArray();

			for(int i=0; i<channelArray.Length; i++) {
				VixenSystem.ChannelPatching.AddPatches(channelArray[i].Id, destinations[i]);
			}
		}

		private Channel _CreateChannel(string name) {
			return new Channel(name);
		}
	}
}
