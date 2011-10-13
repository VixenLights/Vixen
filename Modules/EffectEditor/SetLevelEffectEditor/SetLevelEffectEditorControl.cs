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

namespace VixenModules.EffectEditor.SetLevelEffectEditor
{
	public partial class SetLevelEffectEditorControl : UserControl, IEffectEditorControl
	{
		public SetLevelEffectEditorControl()
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
				if (value.Length >= 2)
					ColorValue = (Color)value[1];
			}
		}

		public Level LevelValue
		{
			get { return levelTypeEditorControl.LevelValue; }
			set { levelTypeEditorControl.LevelValue = value; }
		}

		public Color ColorValue
		{
			get { return colorTypeEditorControl.ColorValue; }
			set { colorTypeEditorControl.ColorValue = value; }
		}
	}
}
