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
	public partial class PreviewFloodSetupControl : DisplayItemBaseControl
	{
		public PreviewFloodSetupControl(PreviewBaseShape shape) : base(shape)
		{
			InitializeComponent();
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