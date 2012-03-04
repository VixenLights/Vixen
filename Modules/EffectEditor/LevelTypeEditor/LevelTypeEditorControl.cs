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
			set
			{
				if (value.Length >= 1)
					LevelValue = (float)value[0];
			}
		}

		public float LevelValue
		{
			get { return (float)valueUpDown.Value / 100 * 255; }
			set { valueUpDown.Value = (int)(value / 255 * 100); }
		}
	}
}
