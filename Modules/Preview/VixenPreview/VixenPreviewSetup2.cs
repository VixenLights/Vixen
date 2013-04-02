using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.Preview.VixenPreview
{
    public partial class VixenPreviewSetup2 : Form
    {
        private VixenPreviewData _data;

        public VixenPreviewData Data
        {
            set
            {
                _data = value;
                if (!DesignMode)
                    preview.Data = _data;
            }
            get
            {
                return _data;
            }
        }

        public VixenPreviewSetup2()
        {
            InitializeComponent();
        }

        private void VixenPreviewSetup2_Load(object sender, EventArgs e)
        {
            Shapes.PreviewTools.PopulateElementTree(treeElements);
            preview.EditMode = true;
            preview.LoadBackground(Data.BackgroundFileName);
            trackBarBackgroundAlpha.Value = Data.BackgroundAlpha;
        }

        private void buttonSetBackground_Click(object sender, EventArgs e)
        {
            if (dialogSelectBackground.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Data.BackgroundFileName = dialogSelectBackground.FileName;
                preview.LoadBackground(dialogSelectBackground.FileName);
                trackBarBackgroundAlpha.Value = trackBarBackgroundAlpha.Maximum;
                //preview.BackgroundAlpha = scrollBackgroundAlpha.Value;
            }
        }

        private void timerRender_Tick(object sender, EventArgs e)
        {
            timerRender.Stop();
            preview.RenderInForeground();
            timerRender.Start();
        }

        private void preview_OnDeSelectDisplayItem(object sender, Shapes.DisplayItem displayItem)
        {
            //properties.SelectedObject = null;
        }

        private void preview_OnSelectDisplayItem(object sender, Shapes.DisplayItem displayItem)
        {
            Console.WriteLine("Item: " + displayItem.GetType());
            //properties.SelectedObject = displayItem.Shape;
            Shapes.DisplayItemBaseControl setupControl = new Shapes.PreviewLineSetupControl(displayItem);
            setupControl.Dock = DockStyle.Fill;
            splitContainerLeft.Panel2.Controls.Add(setupControl);
        }

        private void buttonLine_Click(object sender, EventArgs e)
        {
            preview.CurrentTool = VixenPreviewControl.Tools.String;
        }

        private void toolbarButton_Click(object sender, EventArgs e)
        {
            switch (sender.GetType().ToString())
            {
                case "buttonDrawPixel":
                    break;
                case "buttonLine":
                    preview.CurrentTool = VixenPreviewControl.Tools.String;
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
