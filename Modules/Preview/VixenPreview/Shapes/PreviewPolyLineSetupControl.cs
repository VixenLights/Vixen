using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	public partial class PreviewPolyLineSetupControl : DisplayItemBaseControl
	{
		public PreviewPolyLineSetupControl(PreviewBaseShape shape) : base(shape)
		{
			InitializeComponent();
			propertyGrid.SelectedObject = Shape;
			Shape.OnPropertiesChanged += OnPropertiesChanged;
		}

        ~PreviewPolyLineSetupControl()
		{
			Shape.OnPropertiesChanged -= OnPropertiesChanged;
		}

		private void OnPropertiesChanged(object sender, PreviewBaseShape shape)
		{
			propertyGrid.Refresh();
		}

		private void buttonHelp_Click(object sender, EventArgs e)
		{
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Preview_Icicle);
		}
	}
}