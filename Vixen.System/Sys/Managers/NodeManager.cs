using System;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.Sys.Managers {
	public class NodeManager : IEnumerable<ChannelNode> {
		private ChannelNode _rootNode;

		// a mapping of channel node GUIDs to channel node instances. Used for initial creation, to easily find nodes we have already created.
		// once they've been created, they're in the dictionary. The only way to 'delete' channelnodes is to make a new NodeManager,
		// which reinitializes this mapping and we can start fresh.
		private Dictionary<Guid, ChannelNode> _instances;

		static public event EventHandler NodesChanged;

		public NodeManager() {
			_instances = new Dictionary<Guid, ChannelNode>();
			ChannelNode.Changed += ChannelNode_Changed;
		}

		public NodeManager(IEnumerable<ChannelNode> nodes)
			: this() {
			AddNodes(nodes);
		}

		void ChannelNode_Changed(object sender, EventArgs e) {
			OnNodesChanged();
		}

		private ChannelNode RootNode
		{
			get
			{
				if (_rootNode == null)
					_rootNode = new ChannelNode("Root");

				return _rootNode;
			}
		}


		/// <summary>
		/// Creates a leaf node for a new channel.
		/// </summary>
		/// <param name="channel"></param>
		public void AddChannelLeaf(Channel channel) {
			RootNode.AddChild(new ChannelNode(channel.Name, channel));
		}

		public void RemoveChannelLeaf(Channel channel) {
			// Find any leaf nodes that reference this channel.
			ChannelNode[] leafNodes = _instances.Values.Where(x => x.Channel == channel).ToArray();
			// Remove all instances.
			foreach(ChannelNode leafNode in leafNodes) {
				// since we're effectively trying to remove the channel, we'll be removing
				// ALL nodes with this channel, which means they will be removed from all parents.
				foreach (ChannelNode parent in leafNode.Parents.ToArray()) {
					leafNode.RemoveFromParent(parent, true);
				}
			}
		}

		//public void CopyNode(ChannelNode node, ChannelNode target, int index = -1) {
		//    target = target ?? RootNode;
		//    ChannelNode NewNode = node.Clone();
		//    NewNode.Name = Uniquify(NewNode.Name);
		//    AddChildToParent(NewNode, target, index);
		//}

		public void MoveNode(ChannelNode movingNode, ChannelNode newParent, ChannelNode oldParent, int index = -1) {
			// if null nodes, default to the root node.
			newParent = newParent ?? RootNode;
			oldParent = oldParent ?? RootNode;

			// if we are going to be moving a node within its same group, but to a later position, we need to offset
			// the destination index by 1: once we remove a node, everything shuffles up one, and we need to account for it
			if (oldParent == newParent && index >= 0 && index > newParent.IndexOfChild(movingNode)) {
				index--;
			}

			// remove the node from its old parent, not culling any floating children (since it's about to be added
			// again somewhere else) and then move it to the new parent, in the desired position if set
			RemoveNode(movingNode, oldParent, false);
			AddChildToParent(movingNode, newParent, index);
		}

		public void AddNode(ChannelNode node) {
			AddChildToParent(node, null);
		}

		public void AddNodes(IEnumerable<ChannelNode> nodes) {
			foreach(ChannelNode node in nodes) {
				AddNode(node);
			}
		}

		public ChannelNode AddNode(string name, bool uniquifyName = true) {
			if(uniquifyName) {
				name = _Uniquify(name);
			}
			ChannelNode newNode = new ChannelNode(name);
			AddNode(newNode);
			return newNode;
		}

		public void RemoveNode(ChannelNode node, ChannelNode parent, bool removeChildrenIfFloating) {
			// if the given parent is null, it's most likely a root node (ie. with
			// a parent of our private RootNode). Try to remove it from that instead.
			if (parent == null) {
				node.RemoveFromParent(RootNode, removeChildrenIfFloating);
			} else {
				node.RemoveFromParent(parent, removeChildrenIfFloating);
			}
		}

		public void RenameNode(ChannelNode node, string newName) {
			node.Name = _Uniquify(newName);
			if (node.Channel != null)
				node.Channel.Name = node.Name;
		}

		public void AddChildToParent(ChannelNode child, ChannelNode parent, int index = -1) {
			// if no parent was specified, add to the root node.
			if (parent == null)
				parent = RootNode;

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
			if (_instances.Values.Any(x => x.Name == name)) {
				string originalName = name;
				bool unique;
				int counter = 2;
				do {
					name = originalName + "-" + counter++;
					unique = _instances.Values.All(x => x.Name != name);
				} while(!unique);
			}
			return name;
		}

		public IEnumerable<ChannelNode> InvalidRootNodes {
			get { return RootNode.InvalidChildren(); }
		}

		public bool SetChannelNode(Guid id, ChannelNode node)
		{
			bool rv = _instances.ContainsKey(id);

			_instances[id] = node;
			return rv;
		}

		public ChannelNode GetChannelNode(Guid id) {
			if (_instances.ContainsKey(id)) {
				return _instances[id];
			}
			return null;
		}

		public bool ChannelNodeExists(Guid id)
		{
			return _instances.ContainsKey(id);
		}

		protected virtual void OnNodesChanged()
		{
			if(NodesChanged != null) {
				NodesChanged(this, EventArgs.Empty);
			}
		}

		public IEnumerable<ChannelNode> GetLeafNodes() {
			// Don't want to return the root node.
			// note: this may very well return duplicate nodes, if they are part of different groups.
			return RootNode.Children.SelectMany(x => x.GetLeafEnumerator());
		}

		public IEnumerable<ChannelNode> GetNonLeafNodes() {
			// Don't want to return the root node.
			// note: this may very well return duplicate nodes, if they are part of different groups.
			return RootNode.Children.SelectMany(x => x.GetNonLeafEnumerator());
		}

		public IEnumerable<ChannelNode> GetRootNodes() {
			return RootNode.Children;
		}

		public IEnumerable<ChannelNode> GetAllNodes() {
			//return RootNode.Children.SelectMany(x => x.GetNodeEnumerator());
			return _instances.Values;
		}

		public IEnumerator<ChannelNode> GetEnumerator()
		{
			// Don't want to return the root node.
			return GetAllNodes().GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
