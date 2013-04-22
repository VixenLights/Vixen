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
using System.IO;

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

            elementsForm = new VixenPreviewSetupElementsDocument(previewForm.Preview);
            propertiesForm = new VixenPreviewSetupPropertiesDocument();
            previewForm.Show(dockPanel);
            elementsForm.Show(dockPanel, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
            propertiesForm.Show(elementsForm.Pane, WeifenLuo.WinFormsUI.Docking.DockAlignment.Bottom, 0.5);

            previewForm.Preview.elementsForm = elementsForm;
            previewForm.Preview.propertiesForm = propertiesForm;

            previewForm.Preview.LoadBackground(Data.BackgroundFileName);
            trackBarBackgroundAlpha.Value = Data.BackgroundAlpha;
            previewForm.Preview.Reload();
            Setup();
        }

        private void buttonSetBackground_Click(object sender, EventArgs e)
        {
            if (dialogSelectBackground.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Copy the file to the Vixen folder
                var imageFile = new System.IO.FileInfo(dialogSelectBackground.FileName);
                var destFileName = Path.Combine(VixenPreviewDescriptor.ModulePath, imageFile.Name);
                var sourceFileName = imageFile.FullName;
                if (sourceFileName != destFileName)
                {
                    File.Copy(sourceFileName, destFileName, true);
                }

                // Set the backgrounds
                Data.BackgroundFileName = destFileName;
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
            else if (displayItem.Shape.GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewFlood")
            {
                setupControl = new Shapes.PreviewFloodSetupControl(displayItem);
            }
            else if (displayItem.Shape.GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewCane")
            {
                setupControl = new Shapes.PreviewCaneSetupControl(displayItem);
            }
            else if (displayItem.Shape.GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewStar")
            {
                setupControl = new Shapes.PreviewStarSetupControl(displayItem);
            }
            else if (displayItem.Shape.GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewNet")
            {
                setupControl = new Shapes.PreviewNetSetupControl(displayItem);
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
            else if (button == buttonNet)
                previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.Net;
            else if (button == buttonFlood)
                previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.Flood;
            else if (button == buttonCane)
                previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.Cane;
            else if (button == buttonStar)
                previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.Star;
            else if (button == buttonHelp)
                Shapes.PreviewTools.ShowHelp(Properties.Settings.Default.Help_Main);
            else if (button == buttonMegaTree)
                previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.MegaTree;
        }

        private void trackBarBackgroundAlpha_ValueChanged(object sender, EventArgs e)
        {
            previewForm.Preview.BackgroundAlpha = trackBarBackgroundAlpha.Value;
        }

        public void Setup()
        {
            SetDesktopLocation(Data.SetupLeft, Data.SetupTop);
            Size = new Size(Data.SetupWidth, Data.SetupHeight);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            previewForm.Close();
            Console.WriteLine("Closed");
            Close();
        }

        private void VixenPreviewSetup3_Move(object sender, EventArgs e)
        {
            Data.SetupTop = Top;
            Data.SetupLeft = Left;
        }

        private void VixenPreviewSetup3_Resize(object sender, EventArgs e)
        {
            Data.SetupWidth = Width;
            Data.SetupHeight = Height;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            previewForm.Preview.Cut();
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            previewForm.Preview.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            previewForm.Preview.Paste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            previewForm.Preview.Delete();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void backgroundPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResizePreviewForm resizeForm = new ResizePreviewForm(previewForm.Preview.Background.Width, previewForm.Preview.Background.Height);
            if (resizeForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                previewForm.Preview.ResizeBackground(resizeForm.Width, resizeForm.Height);
            }
        }

    }
}
