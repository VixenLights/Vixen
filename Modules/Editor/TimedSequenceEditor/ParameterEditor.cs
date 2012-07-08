using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class ParameterEditor : UserControl
	{
		public ParameterEditor()
		{
			InitializeComponent();
		}

		public Panel editorPanel { get { return tableLayoutPanel; } }

		public string labelText { get { return labelParameterType.Text; } set { labelParameterType.Text = value; } }
	}
}
