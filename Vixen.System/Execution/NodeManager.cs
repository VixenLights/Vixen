using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Module.Timing;

namespace Vixen.Execution {
	public class NodeManager : IEnumerable<ChannelNode> {
		private ChannelNode _rootNode;

		public event EventHandler NodesChanged;

		public NodeManager() {
			_rootNode = new ChannelNode("Root");
			ChannelNode.Changed += new EventHandler(ChannelNode_Changed);
		}

		void ChannelNode_Changed(object sender, EventArgs e) {
			OnNodesChanged();
		}

		/// <summary>
		/// Creates a leaf node for a new channel.
		/// </summary>
		/// <param name="channel"></param>
		public void AddChannelLeaf(OutputChannel channel) {
			_rootNode.Add(new ChannelNode(channel.Name, channel));
		}

		public void RemoveChannelLeaf(OutputChannel channel) {
			// Find any leaf nodes that reference this channel.
			ChannelNode[] leafNodes = _rootNode.GetNodeEnumerator().Where(x => x.Channel == channel).ToArray();
			// Remove all instances.
			foreach(ChannelNode leafNode in leafNodes) {
				leafNode.Remove();
			}
		}

		public void CopyNode(ChannelNode node, ChannelNode targetNode) {
			if(targetNode == null) {
				// When a node is copied to the root, we're going to assume to start a new branch.
				// This would be necessary anyway for leaf nodes because it will contain
				// a channel and there are already leaf nodes for every channel.
				//*** this situation needs to be handled internally
				ChannelNode newBranch = new ChannelNode(_Uniquify("Unnamed"), node.Clone());
				_rootNode.Add(newBranch);
			} else {
				MoveNode(node.Clone(), targetNode);
			}
		}

		public void MoveNode(ChannelNode branch, ChannelNode targetNode) {
			//Since no backlinks...
			// Find which node references to this so it can be removed from that parent.
			ChannelNode parentNode = _rootNode.GetNodeEnumerator().FirstOrDefault(x => x.Children.Contains(branch));
			if(parentNode != null) {
				parentNode.Remove(branch);
			}
			targetNode = targetNode ?? _rootNode;
			targetNode.Add(branch);
			//No backlinks to break
		}

		public void AddBranch(ChannelNode node) {
			if(!_rootNode.Children.Contains(node)) {
				node.Name = _Uniquify(node.Name);
				_rootNode.Add(node);
			}
		}

		private string _Uniquify(string name) {
			if(_rootNode.GetNodeEnumerator().Any(x => x.Name == name)) {
				string originalName = name;
				bool unique;
				int counter = 2;
				do {
					name = originalName + "-" + counter++;
					unique = !_rootNode.GetNodeEnumerator().Any(x => x.Name == name);
				} while(!unique);
			}
			return name;
		}

		public IEnumerable<ChannelNode> RootNodes {
			get { return _rootNode.Children; }
		}

		protected virtual void OnNodesChanged() {
			if(NodesChanged != null) {
				NodesChanged(this, EventArgs.Empty);
			}
		}

		public IEnumerator<ChannelNode> GetEnumerator() {
			// Don't want to return the root node.
			return _rootNode.Children.SelectMany(x => x.GetNodeEnumerator()).GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
