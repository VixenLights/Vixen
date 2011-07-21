using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Common;
using Vixen.Module.Property;

//Having this be THE concrete class
//vs.
//Having this be an abstract base for ChannelBranch and ChannelLeaf with an
//external reader and writer that writes and checks for a channel id reference
//when reading and uses that to determine whether to create a leaf or branch.
namespace Vixen.Sys {
	public class ChannelNode : GroupNode<OutputChannel> {
		// Making this static so there doesn't have to be potentially thousands of
		// subscriptions from the node manager.
		static public event EventHandler Changed;

		#region Constructors
		private ChannelNode(Guid id, string name, OutputChannel channel, IEnumerable<ChannelNode> content)
			: base(name, content) {
			Id = id;
			Channel = channel;
			Properties = new PropertyManager(this);
		}

		public ChannelNode(string name, OutputChannel channel, IEnumerable<ChannelNode> content)
			: this(Guid.NewGuid(), name, channel, content) {
		}

		public ChannelNode(string name, IEnumerable<ChannelNode> content)
			: this(name, null, content) {
		}

		private ChannelNode(Guid id, string name, OutputChannel channel, params ChannelNode[] content)
			: this(id, name, channel, content as IEnumerable<ChannelNode>) {
		}

		public ChannelNode(string name, OutputChannel channel, params ChannelNode[] content)
			: this(name, channel, content as IEnumerable<ChannelNode>) {
		}

		public ChannelNode(string name, params ChannelNode[] content)
			: this(name, null, content) {
		}
		#endregion

		public OutputChannel Channel { get; internal set; }

		public Guid Id { get; private set; }

		new public ChannelNode Find(string childName) {
			return base.Find(childName) as ChannelNode;
		}

		new public IEnumerable<ChannelNode> Children {
			get { return base.Children.Cast<ChannelNode>(); }
		}

		public bool Masked {
			get { return this.All(x => x.Masked); }
			set {
				foreach(OutputChannel channel in this) {
					channel.Masked = value;
				}
			}
		}

		public ChannelNode Clone() {
			ChannelNode node = null;

			if(IsLeaf) {
				// We're cloning a node, not the channel.
				// Multiple nodes referencing a channel need to reference that same channel instance.
				node = new ChannelNode(Guid.NewGuid(), Name, this.Channel);
			} else {
				node = new ChannelNode(Guid.NewGuid(), Name, this.Channel, this.Children.Select(x => x.Clone()));
			}

			// Property data.
			node.Properties.PropertyData.Clone(this.Properties.PropertyData);

			// Properties
			foreach(IPropertyModuleInstance property in this.Properties) {
				node.Properties.Add(property.Descriptor.TypeId);
			}

			return node;
		}

		public bool IsLeaf {
			get { return base.Children.Count() == 0; }
		}

		public PropertyManager Properties { get; private set; }

		#region Overrides
		public override void AddChild(GroupNode<OutputChannel> node) {
			base.AddChild(node);
			OnChanged(this);
		}

		public override bool RemoveFromParent(GroupNode<OutputChannel> parent) {
			bool result = base.RemoveFromParent(parent);
			OnChanged(this);
			return result;
		}

		public override bool RemoveChild(GroupNode<OutputChannel> node) {
			bool result = base.RemoveChild(node);
			OnChanged(this);
			return result;
		}

		public override GroupNode<OutputChannel> Get(int index) {
			if(IsLeaf) throw new InvalidOperationException("Cannot get child nodes from a leaf.");
			return base.Get(index);
		}

		public override IEnumerator<OutputChannel> GetEnumerator() {
			return GetChannelEnumerator().GetEnumerator();
		}
		#endregion

		#region Enumerators
		public IEnumerable<OutputChannel> GetChannelEnumerator() {
			if(IsLeaf) {
				// OutputChannel is already an enumerable, so AsEnumerable<> won't work.
				return (new[] { Channel });
			} else {
				return this.Children.SelectMany(x => x.GetChannelEnumerator());
			}
		}

		public IEnumerable<ChannelNode> GetNodeEnumerator() {
			// "this" is already an enumerable, so AsEnumerable<> won't work.
			return (new[] { this }).Concat(Children.SelectMany(x => x.GetNodeEnumerator()));
		}

		public IEnumerable<ChannelNode> GetLeafEnumerator() {
			if(IsLeaf) {
				// OutputChannel is already an enumerable, so AsEnumerable<> won't work.
				return (new[] { this });
			} else {
				return Children.SelectMany(x => x.GetLeafEnumerator());
			}
		}

		public IEnumerable<ChannelNode> GetNonLeafEnumerator() {
			if (IsLeaf) {
				return Enumerable.Empty<ChannelNode>();
			} else {
				// "this" is already an enumerable, so AsEnumerable<> won't work.
				return (new[] { this }).Concat(Children.SelectMany(x => x.GetNonLeafEnumerator()));
			}
		}
		#endregion

		#region Static members
		static protected void OnChanged(ChannelNode value) {
			if(Changed != null) {
				Changed(value, EventArgs.Empty);
			}
		}

		static public XElement WriteXml(ChannelNode node) {
			return _WriteXml(node, true);
		}

		static public XElement WriteXmlTemplate(ChannelNode node) {
			return _WriteXml(node, false);
		}

		static private XElement _WriteXml(ChannelNode node, bool includeChannelReferences = true) {
			object channelElements;
			if(node.IsLeaf) {
				// Leaf - reference the single channel by id
				channelElements = includeChannelReferences ? new XAttribute("channelId", node.Channel.Id.ToString()) : null;
			} else {
				// Branch - include the child nodes inline
				channelElements = node.Children.Select(x => _WriteXml(x, includeChannelReferences));
			}
			return new XElement("Node",
				new XAttribute("name", node.Name),
				new XAttribute("id", node.Id),
				new XElement("Properties", node.Properties.Select(x => new XElement("Property", x.Descriptor.TypeId))),
				new XElement("PropertyData", XElement.Parse(node.Properties.PropertyData.Serialize())),
				channelElements);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="element"></param>
		/// <returns>May return null if the node references a non-existent channel.</returns>
		static public ChannelNode ReadXml(XElement element) {
			string name = element.Attribute("name").Value;
			Guid id = Guid.Parse(element.Attribute("id").Value);

			// Children or channel reference
			ChannelNode node = null;
			if(element.Attribute("channelId") == null) {
				// Branch
				node = new ChannelNode(id, name, null, element.Elements("Node").Select(x => ReadXml(x)));
			} else {
				// Leaf
				Guid channelId = Guid.Parse(element.Attribute("channelId").Value);
				OutputChannel channel = Vixen.Sys.Execution.Channels.FirstOrDefault(x => x.Id == channelId);
				if(channel != null) {
					node = new ChannelNode(id, name, channel, null);
				}
			}

			if(node != null) {
				// Property data
				// It's not necessary to load the data before the properties, but it will
				// save it from creating data for each module and then dumping it.
				string moduleDataString = element.Element("PropertyData").InnerXml();
				node.Properties.PropertyData.Deserialize(moduleDataString);

				// Properties
				foreach(XElement propertyElement in element.Element("Properties").Elements("Property")) {
					node.Properties.Add(new Guid(propertyElement.Value));
				}
			}

			return node;
		}
		#endregion
	}
}
