﻿using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.ColorManagement.ColorPicker;
using Common.Controls.Theme;
using VixenModules.Property.Color;
using Vixen.Sys;
using Common.DiscreteColorPicker.Views;

namespace VixenModules.App.LipSyncApp
{
	public partial class LipSyncMapColorCtrl : UserControl
	{
		private bool _discreteColors;
		private IEnumerable<Color> _validDiscreteColors;

		public LipSyncMapColorCtrl()
		{
			InitializeComponent();

			intensityUpDown.BackColor = ThemeColorTable.NumericBackColor;
			intensityUpDown.ForeColor = ThemeColorTable.ForeColor;
			intensityUpDown.Items.AddRange(Enumerable.Range(0, 101).ToArray());
		}


		public List<IElementNode> ChosenNodes
		{
			//get { return _targetEffect; }
			set
			{
				List<IElementNode> nodeList = value;
				_discreteColors = false;
				if (value == null) return;

				HashSet<Color> validColors = new HashSet<Color>();

				// look for the color property of the target effect element, and restrict the gradient.
				// If it's a group, iterate through all children (and their children, etc.), finding as many color
				// properties as possible; then we can decide what to do based on that.
				validColors.AddRange(nodeList.SelectMany(x => ColorModule.getValidColorsForElementNode(x, true)));
				_discreteColors = validColors.Any();
				_validDiscreteColors = validColors;
			}
		}

		private Color _color;
		private HSV _hsvColor;
		private RGB _rgbColor;

		public RGB RGBColor
		{
			get
			{
				return _rgbColor;
			}

			set
			{
				_rgbColor = value;
				_hsvColor = HSV.FromRGB(_rgbColor);
				_color = _rgbColor.ToArgb();

				intensityTrackBar.Value = (int)(_hsvColor.V * 100);
				panelColor.BackColor = _color;
			}
		}

		public HSV HSVColor
		{
			get
			{
				return _hsvColor;
			}

			set
			{
				_hsvColor = value;
				_rgbColor = _hsvColor.ToRGB();
				_color = _rgbColor.ToArgb();

				intensityTrackBar.Value = (int)(_hsvColor.V * 100);
				panelColor.BackColor = _color;
			}
		}

		public Color Color
		{
			get { return _color; }
			set
			{
				_color = value;
				_rgbColor = new RGB(value);
				_hsvColor = HSV.FromRGB(_rgbColor);

				intensityTrackBar.Value = (int)(_hsvColor.V * 100);
				panelColor.BackColor = _color;
			}
		}

		public double Intensity
		{
			get
			{
				return _hsvColor.V;
			}

			set
			{
				_hsvColor.V = value;
				_rgbColor = _hsvColor.ToRGB();
				_color = _rgbColor.ToArgb();

				panelColor.BackColor = _color;
				intensityTrackBar.Value = (int)(Intensity * 100);
			}
		}

		private void intensityTrackBar_ValueChanged(object sender, EventArgs e)
		{
			intensityUpDown.SelectedIndex = intensityTrackBar.Value;
			Intensity = intensityUpDown.SelectedIndex / 100.0;
		}

		private void intensityUpDown_SelectedItemChanged(object sender, EventArgs e)
		{
			intensityTrackBar.Value = this.intensityUpDown.SelectedIndex;
			Intensity = intensityUpDown.SelectedIndex / 100.0;
		}

		private void panelColor_Click(object sender, EventArgs e)
		{
			if (_discreteColors)
			{
				// Create the discrete single color picker view
				SingleDiscreteColorPickerView colorPickerView = new SingleDiscreteColorPickerView(new HashSet<Color>(_validDiscreteColors), Color);

				// Show the single color picker window
				bool? colorResult = colorPickerView.ShowDialog();

				// If the user selected the OK button then...
				if (colorResult.HasValue &&
				    colorResult.Value)
				{
					// Get the selected color
					RGBColor= colorPickerView.GetSelectedColor();
				}
			}
			else
			{
				using (ColorPicker cp = new ColorPicker())
				{
					cp.LockValue_V = false;
					cp.Color = XYZ.FromRGB(Color);
					DialogResult result = cp.ShowDialog();
					if (result == DialogResult.OK)
					{
						RGBColor = cp.Color.ToRGB();
					}
				}
			}
		}
	}
}
