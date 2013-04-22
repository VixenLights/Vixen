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
    public partial class PreviewStarSetupControl : DisplayItemBaseControl
    {
        //private DisplayItem _displayItem;

        public PreviewStarSetupControl(DisplayItem displayItem)
            : base(displayItem)
        {
            InitializeComponent();
            _displayItem = displayItem;
            propertyGrid.SelectedObject = displayItem.Shape;
            displayItem.Shape.OnPropertiesChanged += OnPropertiesChanged;
        }

        ~PreviewStarSetupControl()
        {
            _displayItem.Shape.OnPropertiesChanged -= OnPropertiesChanged;
        }

        private void OnPropertiesChanged(object sender, PreviewBaseShape shape)
        {
            propertyGrid.Refresh();
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            Shapes.PreviewTools.ShowHelp(Properties.Settings.Default.Help_Star);
        }
    }
}
