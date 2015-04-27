using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Vixen.Sys;
using Vixen.Module.EffectEditor;
using Vixen.Module.Effect;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.ColorManagement.ColorPicker;
using VixenModules.Property.Color;

namespace VixenModules.EffectEditor.ColorTypeEditor
{
	public partial class ColorTypeEditorControl : UserControl, IEffectEditorControl
	{
		public ColorTypeEditorControl()
		{
			InitializeComponent();
		}

		private bool _discreteColors;
		private IEnumerable<Color> _validDiscreteColors;
		private IEffect _targetEffect;

		public IEffect TargetEffect
		{
			get { return _targetEffect; }
			set
			{
				_targetEffect = value;
				_discreteColors = false;
				if (_targetEffect == null) return;

				HashSet<Color> validColors = new HashSet<Color>();

				// look for the color property of the target effect element, and restrict the gradient.
				// If it's a group, iterate through all children (and their children, etc.), finding as many color
				// properties as possible; then we can decide what to do based on that.
				validColors.AddRange(_targetEffect.TargetNodes.SelectMany(x => ColorModule.getValidColorsForElementNode(x, true)));
				_discreteColors = validColors.Any();
				_validDiscreteColors = validColors;
			}
		}

		public object[] EffectParameterValues
		{
			get { return new object[] {ColorValue}; }
			set
			{
				if (value.Length >= 1)
					ColorValue = (Color) value[0];
			}
		}

		private Color _color;

		public Color ColorValue
		{
			get { return _color; }
			set
			{
				_color = value;
				panelColor.BackColor = value;
			}
		}

		private void panelColor_Click(object sender, EventArgs e)
		{
			ShowEditor();
		}

		public DialogResult ShowEditor()
		{
			DialogResult result = DialogResult.Cancel;
			if (_discreteColors)
			{
				using (DiscreteColorPicker dcp = new DiscreteColorPicker())
				{
					dcp.ValidColors = _validDiscreteColors;
					dcp.SingleColorOnly = true;
					dcp.SelectedColors = new List<Color> {ColorValue};
					result = dcp.ShowDialog();
					if (result == DialogResult.OK)
					{
						if (dcp.SelectedColors.Count() == 0)
						{
							ColorValue = Color.White;
						}
						else
						{
							ColorValue = dcp.SelectedColors.First();
						}
					}

				}
			}
			else
			{
				using (ColorPicker cp = new ColorPicker())
				{
					cp.LockValue_V = true;
					cp.Color = XYZ.FromRGB(ColorValue);
					result = cp.ShowDialog();
					if (result == DialogResult.OK)
					{
						ColorValue = cp.Color.ToRGB().ToArgb();
					}
				}
			}

			return result;
		}
	}
}