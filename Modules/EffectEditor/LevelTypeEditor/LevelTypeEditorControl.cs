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

namespace VixenModules.EffectEditor.LevelTypeEditor
{
	public partial class LevelTypeEditorControl : UserControl, IEffectEditorControl
	{
		public LevelTypeEditorControl()
		{
			InitializeComponent();
		}

		public object[] EffectParameterValues
		{
			get { return new object[] { LevelValue }; }
			set
			{
				if (value.Length >= 1)
					LevelValue = (Level)value[0];
			}
		}

		public Level LevelValue
		{
			get { return valueUpDown.Value; }
			set { valueUpDown.Value = (int)value; }
		}
	}
}
