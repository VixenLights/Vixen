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
using Vixen.Commands.KnownDataTypes;
using VixenModules.App.ColorGradients;

namespace VixenModules.EffectEditor.ColorGradientTypeEditor
{
	public partial class ColorGradientTypeEditorControl : UserControl, IEffectEditorControl
	{
		public ColorGradientTypeEditorControl()
		{
			InitializeComponent();
			ColorGradientValue = new ColorGradient();
		}

		public IEffect TargetEffect { get; set; }

		public object[] EffectParameterValues
		{
			get { return new object[] { ColorGradientValue }; }
			set
			{
				if (value.Length >= 1)
					ColorGradientValue = (ColorGradient)value[0];
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
			panelGradient.BackgroundImage = ColorGradientValue.GenerateColorGradientImage(panelGradient.Size);
		}

		private void panelGradient_Click(object sender, EventArgs e)
		{
			using (ColorGradientEditor cge = new ColorGradientEditor(ColorGradientValue)) {
				DialogResult result = cge.ShowDialog();
				if (result == DialogResult.OK) {
					ColorGradientValue = cge.Gradient;
				}
			}
		}

	}
}
