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
using Vixen.Commands.KnownDataTypes;
using VixenModules.App.ColorGradients;

namespace VixenModules.EffectEditor.ColorGradientTypeEditor
{
	public partial class ColorGradientTypeEditorControl : UserControl, IEffectEditorControl
	{
		public ColorGradientTypeEditorControl()
		{
			InitializeComponent();
		}

		public object[] EffectParameterValues
		{
			get { return new object[] { ColorGradientValue }; }
			set
			{
				if (value.Length >= 1)
					ColorGradientValue = (ColorGradient)value[0];
			}
		}

		public ColorGradient ColorGradientValue { get; set; }

		private void buttonEditColorGradient_Click(object sender, EventArgs e)
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
