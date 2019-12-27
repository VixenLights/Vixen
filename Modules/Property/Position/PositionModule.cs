using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Vixen.Module;
using Vixen.Module.Property;
using Vixen.Sys;

namespace VixenModules.Property.Position
{
	public class PositionModule : PropertyModuleInstanceBase
	{
		private PositionData _data;
		// For a given node, the position of every node underneath it.
		private static PositionMap _nodePositionMap = new PositionMap();

		public override void SetDefaultValues()
		{
			float spanPerChild = 1f/Owner.Children.Count();
			float start = 0;

			// Make sure each child has a value.
			foreach (IElementNode node in Owner.Children) {
				if (!_data.ChildrenPositions.ContainsKey(node.Id)) {
					_data.ChildrenPositions[node.Id] = new PositionValue(start, spanPerChild);
				}
				start += spanPerChild;
			}
		}

		public override bool HasSetup
		{
			get { return true; }
		}

		public override bool Setup()
		{
			using (SetupForm setupForm = new SetupForm(this)) {
				if (setupForm.ShowDialog() == DialogResult.OK) {
					if (_HaveMap(Owner)) {
						_ParseNode(Owner);
					}
					return true;
				}
				return false;
			}
		}

		public override IModuleDataModel StaticModuleData
		{
			get { return _data; }
			set { _data = value as PositionData; }
		}

		//=====================================================//

		public enum PositionBehavior
		{
			Spanning,
			Resetting
		};

		public void BuildPositionMap(IEnumerable<ElementNode> topLevelNodes)
		{
			foreach (ElementNode node in topLevelNodes) {
				if (!_HaveMap(node)) {
					_ParseNode(node);
				}
			}
		}

		public PositionValue GetPositionValues(Guid elementNodeId)
		{
			PositionValue childPosition;
			_data.ChildrenPositions.TryGetValue(elementNodeId, out childPosition);
			return childPosition;
		}

		public PositionValue GetCalculatedPosition(Guid elementNodeId)
		{
			PositionValue childPosition;
			_nodePositionMap.TryGetValue(elementNodeId, out childPosition);
			return childPosition;
		}

		private static bool _HaveMap(ElementNode node)
		{
			return _nodePositionMap.ContainsKey(node.Id);
		}

		private static void _ParseNode(ElementNode node)
		{
			PositionValue positionValue = new PositionValue(0, 1);
			PositionMap positions = _ParseChildren(node, positionValue);
			_nodePositionMap.AddRange(positions);
		}

		private static PositionMap _ParseChildren(ElementNode parentNode, PositionValue parentPosition)
		{
			// Traverse every node.  Nodes with the position property will affect the childrens'
			// positions.
			PositionMap childValues = new PositionMap();

			PositionModule positionProperty = parentNode.Properties.Get(PositionDescriptor._typeId) as PositionModule;

			foreach (ElementNode childNode in parentNode.Children) {
				PositionValue childPosition = positionProperty.GetPositionValues(childNode.Id);
				if (childPosition != null) {
					// Parent has a modifying position for the child.
					childPosition = _Multiply(parentPosition, childPosition);
				}
				else {
					// Child inherits parent's position.
					childPosition = new PositionValue(parentPosition);
				}

				childValues[childNode.Id] = childPosition;
				childValues.AddRange(_ParseChildren(childNode, childPosition));
			}

			return childValues;
		}

		private static PositionValue _Multiply(PositionValue parentPosition, PositionValue childPosition)
		{
			return new PositionValue(parentPosition.Start + childPosition.Start*parentPosition.Width,
			                         childPosition.Width*parentPosition.Width);
		}
	}
}