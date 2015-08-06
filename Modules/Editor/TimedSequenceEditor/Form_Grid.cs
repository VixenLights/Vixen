using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls.Theme;
using WeifenLuo.WinFormsUI.Docking;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class Form_Grid : DockContent
	{
		public Form_Grid()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.ComboBoxBackColor;
			ThemeUpdateControls.UpdateControls(this);

		}

		public Common.Controls.Timeline.TimelineControl TimelineControl 
		{ 
			get 
			{
				return timelineControl;
			}
		}
	}
}
