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
using VixenModules.App.Curves;

namespace VixenModules.EffectEditor.CurveTypeEditor
{
	public partial class CurveTypeEditorControl : UserControl, IEffectEditorControl
	{
		public CurveTypeEditorControl()
		{
			InitializeComponent();
		}

		public object[] EffectParameterValues
		{
			get { return new object[] { CurveValue }; }
			set
			{
				if (value.Length >= 1)
					CurveValue = (Curve)value[0];
			}
		}

		public Curve CurveValue { get; set; }

		private void buttonEditCurve_Click(object sender, EventArgs e)
		{
			CurveValue.EditCurve();
		}
	}
}
