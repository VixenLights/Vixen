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
        public PreviewFloodSetupControl(DisplayItem displayItem)
            : base(displayItem)
        {
            InitializeComponent();
            _displayItem = displayItem;
            propertyGrid.SelectedObject = displayItem.Shape;
            displayItem.Shape.OnPropertiesChanged += OnPropertiesChanged;
        }

        ~PreviewFloodSetupControl()
        {
            _displayItem.Shape.OnPropertiesChanged -= OnPropertiesChanged;
        }

        private void OnPropertiesChanged(object sender, PreviewBaseShape shape)
        {
            propertyGrid.Refresh();
        }
    }
}
