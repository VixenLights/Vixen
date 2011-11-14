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
using CommonElements.ColorManagement.ColorModels;
using CommonElements.ColorManagement.ColorPicker;

namespace VixenModules.EffectEditor.ColorTypeEditor
{
	public partial class ColorTypeEditorControl : UserControl, IEffectEditorControl
	{
		public ColorTypeEditorControl()
		{
			InitializeComponent();
		}

		public IEffect TargetEffect { get; set; }

		public object[] EffectParameterValues
		{
			get { return new object[] { ColorValue }; }
			set
			{
				if (value.Length >= 1)
					ColorValue = (Color)value[0];
			}
		}

		private Color _color;
		public Color ColorValue
		{
			get { return _color; }
			set { _color = value; panelColor.BackColor = value; }
		}

		private void panelColor_Click(object sender, EventArgs e)
		{
			using (ColorPicker cp = new ColorPicker()) {
				cp.LockValue_V = true;
				cp.Color = XYZ.FromRGB(ColorValue);
				DialogResult result = cp.ShowDialog();
				if (result == DialogResult.OK) {
					ColorValue = cp.Color.ToRGB().ToArgb();
				}
			}
		}
	}
}
