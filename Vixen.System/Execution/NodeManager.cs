using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Vixen.Sys;
using Vixen.Module.Timing;

namespace Vixen.Execution {
	public class NodeManager : IEnumerable<ChannelNode> {
		private ChannelNode _rootNode;

		public event EventHandler NodesChanged;

		public NodeManager() {
			_rootNode = new ChannelNode("Root");
			ChannelNode.Changed += new EventHandler(ChannelNode_Changed);
		}

		public NodeManager(IEnumerable<ChannelNode> nodes)
			: this() {
			AddNodes(nodes);
		}

		void ChannelNode_Changed(object sender, EventArgs e) {
			OnNodesChanged();
		}

		/// <summary>
		/// Creates a leaf node for a new channel.
		/// </summary>
		/// <param name="channel"></param>
		public void AddChannelLeaf(Channel channel) {
			_rootNode.AddChild(new ChannelNode(channel.Name, channel));
		}

		public void RemoveChannelLeaf(Channel channel) {
			// Find any leaf nodes that reference this channel.
			ChannelNode[] leafNodes = _rootNode.GetNodeEnumerator().Where(x => x.Channel == channel).ToArray();
			// Remove all instances.
			foreach(ChannelNode leafNode in leafNodes) {
				// since we're effectively trying to remove the channel, we'll be removing
				// ALL nodes with this channel, which means they will be removed from all parents.
				foreach (ChannelNode parent in leafNode.Parents.ToArray()) {
					leafNode.RemoveFromParent(parent);
				}
			}
		}

		public void CopyNode(ChannelNode node, ChannelNode target, int index = -1) {
			target = target ?? _rootNode;
			ChannelNode NewNode = node.Clone();
			NewNode.Name = _Uniquify(NewNode.Name);
			AddChildToParent(NewNode, target, index);
		}

		public void MoveNode(ChannelNode movingNode, ChannelNode newParent, ChannelNode oldParent, int index = -1) {
			// add the node to the root node if a target wasn't given.	
			newParent = newParent ?? _rootNode;
			AddChildToParent(movingNode, newParent, index);

			// remove the node from its old parent. This must be done last, otherwise it may have its children culled
			// if it was the last instance available (ie. was temorarily floating free while moving around)
			RemoveNode(movingNode, oldParent);
		}

		public void MirrorNode(ChannelNode node, ChannelNode target) {
			AddChildToParent(node, target);
		}

		public void AddNode(ChannelNode node) {
			if(!_rootNode.Children.Contains(node)) {
				_rootNode.AddChild(node);
			}
		}

		public void AddNodes(IEnumerable<ChannelNode> nodes) {
			foreach(ChannelNode node in nodes) {
				AddNode(node);
			}
		}

		public ChannelNode AddNewNode(string name) {
			name = _Uniquify(name);
			ChannelNode newNode = new ChannelNode(name);
			AddNode(newNode);
			return newNode;
		}

		public void RemoveNode(ChannelNode node, ChannelNode parent) {
			// if the given parent is null, it's most likely a root node (ie. with
			// a parent of our private _rootNode). Try to remove it from that instead.
			if (parent == null) {
				node.RemoveFromParent(_rootNode);
			} else {
				node.RemoveFromParent(parent);
			}
		}

		public void RenameNode(ChannelNode node, string newName) {
			node.Name = _Uniquify(newName);
		}

		public void AddChildToParent(ChannelNode child, ChannelNode parent, int index = -1) {
			// if an item is a group (or is becoming one), it can't have an output
			// channel anymore. Remove it.
			if (parent.Channel != null) {
				VixenSystem.Channels.RemoveChannel(parent.Channel);
				parent.Channel = null;
			}

			// if an index was specified, insert it in that position, otherwise just add it at the end
			if (index < 0)
				parent.AddChild(child);
			else
				parent.InsertChild(index, child);
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

		public IEnumerable<ChannelNode> InvalidRootNodes {
			get { return _rootNode.InvalidChildren(); }
		}

		public IEnumerable<ChannelNode> RootNodes {
			get { return _rootNode.Children; }
		}

		protected virtual void OnNodesChanged() {
			if(NodesChanged != null) {
				NodesChanged(this, EventArgs.Empty);
			}
		}

		public IEnumerable<ChannelNode> GetLeafNodes() {
			// Don't want to return the root node.
			// note: this may very well return duplicate nodes, if they are part of different groups.
			return _rootNode.Children.SelectMany(x => x.GetLeafEnumerator());
		}

		public IEnumerable<ChannelNode> GetNonLeafNodes() {
			// Don't want to return the root node.
			// note: this may very well return duplicate nodes, if they are part of different groups.
			return _rootNode.Children.SelectMany(x => x.GetNonLeafEnumerator());
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
