using System;
using System.Linq;
using System.Windows.Forms;
using Vixen.Sys;

namespace VixenModules.Property.Position {
	public partial class SetupForm : Form {
		private ChannelNode[] _children;
		private PositionValue[] _positions;
		private PositionModule _positionProperty;

		public SetupForm(PositionModule positionProperty) {
			if(positionProperty.Owner == null) {
				throw new InvalidOperationException("Property must be attached to a node.");
			}

			InitializeComponent();

			_positionProperty = positionProperty;
			_children = _positionProperty.Owner.Children.ToArray();
			_positions = _children.Select(x => _positionProperty.GetPositionValues(x.Id)).ToArray();

			listBoxNodeChildren.DisplayMember = "Name";
			listBoxNodeChildren.ValueMember = "Id";
			listBoxNodeChildren.DataSource = _children;
		}

		private float _Start {
			get { return (float)numericUpDownStart.Value / 100; }
			set { numericUpDownStart.Value = (decimal)value * 100; }
		}

		private float _Width {
			get { return (float)numericUpDownWidth.Value / 100; }
			set { numericUpDownWidth.Value = (decimal)value * 100; }
		}

		private void listBoxNodeChildren_SelectedIndexChanged(object sender, EventArgs e) {
			if(listBoxNodeChildren.SelectedItem != null) {
				PositionValue position = _positions[listBoxNodeChildren.SelectedIndex];
				_Start = position.Start;
				_Width = position.Width;
			}
		}

		private void numericUpDownStart_Leave(object sender, EventArgs e) {
			_UpdatePosition();
		}

		private void numericUpDownWidth_Leave(object sender, EventArgs e) {
			_UpdatePosition();
		}

		private void _UpdatePosition() {
			PositionValue position = new PositionValue(_Start, _Width);
			_positions[listBoxNodeChildren.SelectedIndex] = position;
		}

		private void buttonOK_Click(object sender, EventArgs e) {
			PositionData data = _positionProperty.StaticModuleData as PositionData;
			for(int i=0; i<_positions.Length; i++) {
				ChannelNode child = _children[i];
				PositionValue childPosition = _positions[i];
				data.ChildrenPositions[child.Id] = childPosition;
			}
		}
	}
}
