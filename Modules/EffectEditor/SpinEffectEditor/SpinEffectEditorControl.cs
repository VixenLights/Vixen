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

namespace VixenModules.EffectEditor.SpinEffectEditor
{
	public partial class SpinEffectEditorControl : UserControl, IEffectEditorControl
	{
		public SpinEffectEditorControl()
		{
			InitializeComponent();
		}

		public object[] EffectParameterValues
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
	}
}
