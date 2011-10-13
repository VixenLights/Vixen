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
using VixenModules.App.Curves;

namespace VixenModules.EffectEditor.PulseEffectEditor
{
	public partial class PulseEffectEditorControl : UserControl, IEffectEditorControl
	{
		public PulseEffectEditorControl()
		{
			InitializeComponent();
		}

		public object[] EffectParameterValues
		{
			get { return new object[] { CurveValue, ColorGradientValue }; }
			set
			{
				if (value.Length >= 1)
					CurveValue = (Curve)value[0];
				if (value.Length >= 2)
					ColorGradientValue = (ColorGradient)value[1];
			}
		}

		public Curve CurveValue
		{
			get { return curveTypeEditorControl.CurveValue; }
			set { curveTypeEditorControl.CurveValue = value; }
		}

		public ColorGradient ColorGradientValue
		{
			get { return colorGradientTypeEditorControl.ColorGradientValue; }
			set { colorGradientTypeEditorControl.ColorGradientValue = value; }
		}
	}
}
