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
			_rootNode.AddChild(new ChannelNode(channel.Name, channel));
		}

		public void RemoveChannelLeaf(OutputChannel channel) {
			// Find any leaf nodes that reference this channel.
			ChannelNode[] leafNodes = _rootNode.GetNodeEnumerator().Where(x => x.Channel == channel).ToArray();
			// Remove all instances.
			foreach(ChannelNode leafNode in leafNodes) {
				// since we're effectively trying to remove the channel, we'll be removing
				// ALL nodes with this channel, which means they will be removed from all parents.
				foreach (ChannelNode parent in leafNode.Parents) {
					leafNode.RemoveFromParent(parent);
				}
			}
		}

		public void CopyNode(ChannelNode node, ChannelNode target) {
			target = target ?? _rootNode;
			target.AddChild(node.Clone());
		}

		public void MoveNode(ChannelNode node, ChannelNode target, ChannelNode parent) {
			// remove the node from its current parent first
			RemoveNode(node, parent);

			// add the node to the root node if a target wasn't given.	
			target = target ?? _rootNode;
			target.AddChild(node);
		}

		public void MirrorNode(ChannelNode node, ChannelNode target) {
			target.AddChild(node);
		}

		public void AddNode(ChannelNode node) {
			if(!_rootNode.Children.Contains(node)) {
				node.Name = _Uniquify(node.Name);
				_rootNode.AddChild(node);
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
