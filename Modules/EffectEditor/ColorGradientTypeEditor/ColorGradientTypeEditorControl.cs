using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Module.EffectEditor;
using Vixen.Module.Effect;
using VixenModules.App.ColorGradients;
using VixenModules.Property.Color;

namespace VixenModules.EffectEditor.ColorGradientTypeEditor
{
	public partial class ColorGradientTypeEditorControl : UserControl, IEffectEditorControl
	{
		private bool _discreteColors;
		private IEnumerable<Color> _validDiscreteColors;

		public ColorGradientTypeEditorControl()
		{
			InitializeComponent();
			ColorGradientValue = new ColorGradient();
			_discreteColors = false;
		}

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

				UpdateGradientImage();
			}
		}

		public object[] EffectParameterValues
		{
			get { return new object[] {ColorGradientValue}; }
			set
			{
				if (value.Length >= 1)
					ColorGradientValue = (ColorGradient) value[0];
			}
		}

		private ColorGradient _gradient;

		public ColorGradient ColorGradientValue
		{
			get { return _gradient; }
			set
			{
				_gradient = value;
				UpdateGradientImage();
			}
		}

		private void UpdateGradientImage()
		{
            if (ColorGradientValue == null) 
            {

                ColorGradientValue = new ColorGradient();
            
            }
			panelGradient.BackgroundImage = ColorGradientValue.GenerateColorGradientImage(panelGradient.Size, _discreteColors);
		}

		private void panelGradient_Click(object sender, EventArgs e)
		{
			ShowEditor();
		}

		public DialogResult ShowEditor()
		{
			using (ColorGradientEditor cge = new ColorGradientEditor(ColorGradientValue, _discreteColors, _validDiscreteColors))
			{
				DialogResult result = cge.ShowDialog();
				if (result == DialogResult.OK)
				{
					ColorGradientValue = cge.Gradient;
				}

				return result;

			}
		}
	}
}