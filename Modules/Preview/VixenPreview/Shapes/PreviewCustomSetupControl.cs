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
    public partial class PreviewCustomSetupControl : DisplayItemBaseControl
    {
        public PreviewCustomSetupControl(PreviewBaseShape shape): base(shape)
        {
            InitializeComponent();
            foreach (PreviewBaseShape stringShape in Shape._strings)
            {
                stringShape.OnPropertiesChanged += OnPropertiesChanged;
            }
        }

        ~PreviewCustomSetupControl()
        {
            foreach (PreviewBaseShape stringShape in Shape._strings)
            {
                stringShape.OnPropertiesChanged -= OnPropertiesChanged;
            }
        }

        private void OnPropertiesChanged(object sender, PreviewBaseShape shape)
        {
            PopulatePropList((comboBoxStringToEdit.SelectedItem as ComboBoxItem).Value as PreviewBaseShape);
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            Shapes.PreviewTools.ShowHelp(Properties.Settings.Default.Help_CustomShape);
        }

        private void PreviewCustomSetupControl_Load(object sender, EventArgs e)
        {
            PopulatePropList();
        }

        private void PopulatePropList(PreviewBaseShape selectedShape = null)
        {
            comboBoxStringToEdit.Items.Clear();
            foreach (PreviewBaseShape shape in Shape._strings) 
            {
                ComboBoxItem item = new ComboBoxItem(shape.Name, shape);
                if (item.Text == null)
                {
                    item.Text = shape.GetType().ToString();
                    item.Text = item.Text.Substring(item.Text.LastIndexOf('.') + 1);
                }
                comboBoxStringToEdit.Items.Add(item);
            }
            if (comboBoxStringToEdit.Items.Count > 0)
            {
                if (selectedShape != null)
                {
                    foreach (ComboBoxItem item in comboBoxStringToEdit.Items) 
                    {
                        if ((item.Value as PreviewBaseShape) == selectedShape)
                        {
                            comboBoxStringToEdit.SelectedItem = item;
                            return;
                        }
                    }
                }
                else
                {
                    comboBoxStringToEdit.SelectedIndex = 0;
                }
            }
        }

        public void ShowSetupControl(PreviewBaseShape shape)
        {
            panelProperties.Controls.Clear();
            Shapes.DisplayItemBaseControl setupControl = shape.GetSetupControl();
            if (setupControl != null)
            {
                panelProperties.Controls.Add(setupControl);
                setupControl.Dock = DockStyle.Fill;
            }
        }

        private void comboBoxStringToEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxItem item = comboBoxStringToEdit.SelectedItem as ComboBoxItem;
            if (item != null)
            {
                PreviewBaseShape shape = item.Value as PreviewBaseShape;
                if (shape != null)
                {
                    ShowSetupControl(shape);
                }
            }
        }
    }
}
