using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls.Theme;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	public partial class PreviewFloodSetupControl : DisplayItemBaseControl
	{
		public PreviewFloodSetupControl(PreviewBaseShape shape) : base(shape)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			ThemePropertyGridRenderer.PropertyGridRender(propertyGrid);
			propertyGrid.SelectedObject = Shape;
			Shape.OnPropertiesChanged += OnPropertiesChanged;
		}

		~PreviewFloodSetupControl()
		{
			Shape.OnPropertiesChanged -= OnPropertiesChanged;
		}

		private void OnPropertiesChanged(object sender, PreviewBaseShape shape)
		{
			propertyGrid.Refresh();
		}
	}
}