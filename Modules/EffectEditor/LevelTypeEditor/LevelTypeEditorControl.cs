using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Module.EffectEditor;
using Vixen.Module.Effect;

namespace VixenModules.EffectEditor.LevelTypeEditor
{
	public partial class LevelTypeEditorControl : UserControl, IEffectEditorControl
	{
		public LevelTypeEditorControl()
		{
			InitializeComponent();
			valueUpDown.TrackerOrientation = Orientation.Horizontal;
		}

		public IEffect TargetEffect { get; set; }

		public object[] EffectParameterValues
		{
			get { return new object[] { LevelValue }; }
			set { LevelValue = (double)value[0]; }
		}

		public double LevelValue
		{
			get { return (double)valueUpDown.Value / 100; }
			set { valueUpDown.Value = (int)(value * 100); }
		}
	}
}
