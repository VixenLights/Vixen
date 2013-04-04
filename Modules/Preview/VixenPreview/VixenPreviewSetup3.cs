using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Execution.Context;
using Vixen.Module.Preview;
using Vixen.Data.Value;
using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview
{
    public partial class VixenPreviewSetup3 : Form
    {
        private VixenPreviewData _data;
        private VixenPreviewSetupDocument previewForm;
        private VixenPreviewSetupElementsDocument elementsForm;
        private VixenPreviewSetupPropertiesDocument propertiesForm;

        public VixenPreviewData Data
        {
            set
            {
                _data = value;
                if (!DesignMode && previewForm != null)
                    previewForm.Preview.Data = _data;
            }
            get
            {
                return _data;
            }
        }

        public VixenPreviewSetup3()
        {
            InitializeComponent();
        }

        private void VixenPreviewSetup3_Load(object sender, EventArgs e)
        {
            previewForm = new VixenPreviewSetupDocument();
            if (!DesignMode && previewForm != null)
                previewForm.Preview.Data = _data;
            previewForm.Preview.OnSelectDisplayItem += OnSelectDisplayItem;
            previewForm.Preview.OnDeSelectDisplayItem += OnDeSelectDisplayItem;

            elementsForm = new VixenPreviewSetupElementsDocument();
            propertiesForm = new VixenPreviewSetupPropertiesDocument();
            previewForm.Show(dockPanel);
            elementsForm.Show(dockPanel, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
            propertiesForm.Show(elementsForm.Pane, WeifenLuo.WinFormsUI.Docking.DockAlignment.Bottom, 0.5);

            Setup();
        }

        private void buttonSetBackground_Click(object sender, EventArgs e)
        {
            if (dialogSelectBackground.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Data.BackgroundFileName = dialogSelectBackground.FileName;
                previewForm.Preview.LoadBackground(dialogSelectBackground.FileName);
                trackBarBackgroundAlpha.Value = trackBarBackgroundAlpha.Maximum;
                previewForm.Preview.BackgroundAlpha = trackBarBackgroundAlpha.Value;
            }
        }

        private void OnDeSelectDisplayItem(object sender, Shapes.DisplayItem displayItem)
        {
            propertiesForm.ShowSetupControl(null);
        }

        private void OnSelectDisplayItem(object sender, Shapes.DisplayItem displayItem)
        {
            Console.WriteLine("Item: " + displayItem.Shape.ToString());
            Shapes.DisplayItemBaseControl setupControl = null;

            if (displayItem.Shape.GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewSingle")
            {
                setupControl = new Shapes.PreviewSingleSetupControl(displayItem);
            }
            else if (displayItem.Shape.GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewLine")
            {
                setupControl = new Shapes.PreviewLineSetupControl(displayItem);
            }
            else if (displayItem.Shape.GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewRectangle")
            {
                setupControl = new Shapes.PreviewRectangleSetupControl(displayItem);
            }
            else if (displayItem.Shape.GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewEllipse")
            {
                setupControl = new Shapes.PreviewEllipseSetupControl(displayItem);
            }
            else if (displayItem.Shape.GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewArch")
            {
                setupControl = new Shapes.PreviewArchSetupControl(displayItem);
            }
            else if (displayItem.Shape.GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewMegaTree")
            {
                setupControl = new Shapes.PreviewMegaTreeSetupControl(displayItem);
            }
            else if (displayItem.Shape.GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewTriangle")
            {
                setupControl = new Shapes.PreviewTriangleSetupControl(displayItem);
            }

            if (setupControl != null)
            {
                propertiesForm.ShowSetupControl(setupControl);
            }
        }

        private void toolbarButton_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            Console.WriteLine(button.Name);
            // Select Button
            if (button == buttonSelect)
                previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.Select;
            // Standard Buttons
            else if (button == buttonDrawPixel)
                previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.Single;
            else if (button == buttonLine)
                previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.String;
            else if (button == buttonSemiCircle)
                previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.Arch;
            else if (button == buttonRectangle)
                previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.Rectangle;
            else if (button == buttonEllipse)
                previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.Ellipse;
            else if (button == buttonTriangle)
                previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.Triangle;
            // Smart Shape Buttons
            else if (button == buttonMegaTree)
                previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.MegaTree;
        }

        private void trackBarBackgroundAlpha_ValueChanged(object sender, EventArgs e)
        {
            previewForm.Preview.BackgroundAlpha = trackBarBackgroundAlpha.Value;
        }

        public void Setup()
        {
            previewForm.Preview.LoadBackground(Data.BackgroundFileName);

            Top = Data.SetupTop;
            Left = Data.SetupLeft;
            Width = Data.SetupWidth;
            Height = Data.SetupHeight;

            //if (Data.Width > MinimumSize.Width)
            //    Width = Data.Width;
            //else
            //    Width = MinimumSize.Width;

            //if (Data.Height > MinimumSize.Height)
            //    Height = Data.Height;
            //else
            //    Height = MinimumSize.Height;
        }

        public void Save()
        {
            Data.SetupTop = Top;
            Data.SetupLeft = Left;
            Data.SetupWidth = Width;
            Data.SetupHeight = Height;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Save();
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }
    }
}
