using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Vixen.Module;
using Vixen.Module.Property;
using Vixen.Sys;

namespace VixenModules.Property.Position {
	public class PositionModule : PropertyModuleInstanceBase {
		private PositionData _data;
		// For a given node, the position of every node underneath it.
		static private Dictionary<Guid, PositionMap> _nodePositionMap = new Dictionary<Guid, PositionMap>();

		public override void SetDefaultValues() {
			float spanPerChild = 1f/Owner.Children.Count();
			float start = 0;

			// Make sure each child has a value.
			foreach(ChannelNode node in Owner.Children) {
				if(!_data.ChildrenPositions.ContainsKey(node.Id)) {
					_data.ChildrenPositions[node.Id] = new PositionValue(start, spanPerChild);
				}
				start += spanPerChild;
			}
		}

		public override bool HasSetup {
			get { return true; }
		}

		public override void Setup() {
			using(SetupForm setupForm = new SetupForm(this)) {
				if(setupForm.ShowDialog() == DialogResult.OK) {
					if(_HaveMap(Owner)) {
						_ParseNode(Owner);
					}
				}
			}
		}

		public override IModuleDataModel StaticModuleData {
			get { return _data; }
			set { _data = value as PositionData; }
		}

		//=====================================================//

		public enum PositionBehavior { Spanning, Resetting };

		public void BuildPositionMap(IEnumerable<ChannelNode> topLevelNodes) {
			_GetPositionMap(topLevelNodes);
		}

		public PositionValue GetPosition(Guid channelNodeId) {
			PositionValue childPosition;
			_data.ChildrenPositions.TryGetValue(channelNodeId, out childPosition);
			return childPosition;
		}

		static private PositionMap _GetPositionMap(IEnumerable<ChannelNode> topLevelNodes) {
			// If there are conflicts in the resulting absolute positioning, we're not going
			// to care how that plays out.  Shame on them for not doing an accurate modeling. ;)
			PositionMap positionMap = new PositionMap();
			foreach(ChannelNode node in topLevelNodes) {
				PositionMap nodeMap = _GetPositionMap(node);
				positionMap.AddRange(nodeMap);
			}
			return positionMap;
		}

		static private PositionMap _GetPositionMap(ChannelNode topLevelNode) {
			PositionMap map;
			if(!_nodePositionMap.TryGetValue(topLevelNode.Id, out map)) {
				_ParseNode(topLevelNode);
				map = _nodePositionMap[topLevelNode.Id];
			}
			return map;
		}

		static private bool _HaveMap(ChannelNode node) {
			return _nodePositionMap.ContainsKey(node.Id);
		}

		static private void _ParseNode(ChannelNode node) {
			// This will be the dictionary of node : position for every node within this branch.
			PositionValue positionValue = new PositionValue(0, 100);
			_nodePositionMap[node.Id] = _ParseChildren(node, positionValue);
		}

		static private PositionMap _ParseChildren(ChannelNode parentNode, PositionValue parentPosition) {
			// Traverse every node.  Nodes with the position property will affect the childrens'
			// positions.
			PositionMap childValues = new PositionMap();

			PositionModule positionProperty = parentNode.Properties.Get(PositionDescriptor._typeId) as PositionModule;

			foreach(ChannelNode childNode in parentNode.Children) {
				PositionValue childPosition = positionProperty.GetPosition(childNode.Id);
				if(childPosition != null) {
					// Parent has a modifying position for the child.
					childPosition = _Multiply(parentPosition, childPosition);
				} else {
					// Child inherits parent's position.
					childPosition = new PositionValue(parentPosition);
				}

				childValues[childNode.Id] = childPosition;
				childValues.AddRange(_ParseChildren(childNode, childPosition));
			}

			return childValues;
		}

		static private PositionValue _Multiply(PositionValue parentPosition, PositionValue childPosition) {
			return new PositionValue(parentPosition.Start + childPosition.Start * parentPosition.Width, childPosition.Width * parentPosition.Width);
		}
	}
}
