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
using VixenModules.App.Curves;

namespace VixenModules.EffectEditor.CurveTypeEditor
{
	public partial class CurveTypeEditorControl : UserControl, IEffectEditorControl
	{
		public CurveTypeEditorControl()
		{
			InitializeComponent();
			CurveValue = new Curve();
		}

		public IEffect TargetEffect { get; set; }

		public object[] EffectParameterValues
		{
			get { return new object[] {CurveValue}; }
			set
			{
				if (value.Length >= 1)
					CurveValue = (Curve) value[0];
			}
		}

		private Curve _curve;

		public Curve CurveValue
		{
			get { return _curve; }
			set
			{
				_curve = value;
				UpdateCurveImage();
			}
		}

		private void UpdateCurveImage()
		{
			panelCurve.BackgroundImage = _curve.GenerateCurveImage(panelCurve.Size);
		}

		private void panelCurve_Click(object sender, EventArgs e)
		{
			ShowEditor();
		}

		public void ShowEditor()
		{
			CurveEditor editor = new CurveEditor(CurveValue);
			if (editor.ShowDialog() == DialogResult.OK)
			{
				CurveValue = editor.Curve;
			}
		}
	}
}