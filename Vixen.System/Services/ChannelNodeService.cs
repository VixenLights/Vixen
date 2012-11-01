using System.Linq;
using Vixen.Sys;

namespace Vixen.Services {
	public class ChannelNodeService {
		static private ChannelNodeService _instance;

		private ChannelNodeService() {
		}

		public static ChannelNodeService Instance {
			get { return _instance ?? (_instance = new ChannelNodeService()); }
		}

		public ChannelNode CreateSingle(ChannelNode parentNode, string name = null, bool createChannel = false, bool uniquifyName = true, int index = -1) {
			name = name ?? "Unnamed";

			ChannelNode channelNode = VixenSystem.Nodes.AddNode(name, parentNode, uniquifyName);

			Channel channel = createChannel ? _CreateChannel(name) : null;
			channelNode.Channel = channel;
			VixenSystem.Channels.AddChannel(channel);

			return channelNode;
		}

		public ChannelNode[] CreateMultiple(ChannelNode parentNode, int count, bool createChannel = false, bool uniquifyNames = true) {
			return Enumerable.Range(0, count).Select(x => CreateSingle(parentNode, null, createChannel, uniquifyNames)).ToArray();
		}

		public ChannelNode ImportTemplateOnce(string templateFileName, ChannelNode parentNode) {
			ChannelNodeTemplate channelNodeTemplate = FileService.Instance.LoadChannelNodeTemplateFile(templateFileName);
			if(channelNodeTemplate == null) return null;

			VixenSystem.Nodes.AddChildToParent(channelNodeTemplate.ChannelNode, parentNode);

			return channelNodeTemplate.ChannelNode;
		}

		public ChannelNode[] ImportTemplateMany(string templateFileName, ChannelNode parentNode, int count) {
			return Enumerable.Range(0, count).Select(x => ImportTemplateOnce(templateFileName, parentNode)).NotNull().ToArray();
		}

		public void Rename(ChannelNode channelNode, string name) {
			channelNode.Name = name;
		}

		private Channel _CreateChannel(string name) {
			return new Channel(name);
		}
	}
}
