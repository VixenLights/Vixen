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
    public partial class VixenPreviewDisplay : Form
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

        public VixenPreviewDisplay()
        {
            InitializeComponent();
        }

        public VixenPreviewControl PreviewControl
        {
            get { return preview; }
        }

        public void Setup()
        {
            preview.LoadBackground(Data.BackgroundFileName);

            SetDesktopLocation(Data.Left, Data.Top);
            Size = new Size(Data.Width, Data.Height);
            //if (Data.Width > MinimumSize.Width)
            //    Width = Data.Width;
            //else
            //    Width = MinimumSize.Width;

            //if (Data.Height > MinimumSize.Height)
            //    Height = Data.Height;
            //else
            //    Height = MinimumSize.Height;
        }

        private void timerStatus_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = preview.PixelCount.ToString();
            //toolStripAverageUpdate.Text = "Average: " + Math.Round(VixenPreviewControl.averageUpdateTime).ToString() + "ms";
            toolStripStatusCurrentUpdate.Text = "Last: " + Math.Round(VixenPreviewControl.lastUpdateTime).ToString() + "ms";
            //toolStripStatusLastRenderTime.Text = "Render: " + Math.Round(lastRenderTime).ToString() + "ms";
            toolStripStatusLastRenderTime.Text = "Render: " + Math.Round(preview.lastRenderUpdateTime).ToString() + "ms";
        }

        private void VixenPreviewDisplay_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                MessageBox.Show("The preview can only be closed from the Preview Configuration dialog.", "Close", MessageBoxButtons.OKCancel);
                e.Cancel = true;
            }
        }

        private void VixenPreviewDisplay_Move(object sender, EventArgs e)
        {
            Data.Top = Top;
            Data.Left = Left;
        }

        private void VixenPreviewDisplay_Resize(object sender, EventArgs e)
        {
            Data.Width = Width;
            Data.Height = Height;
        }

        private void VixenPreviewDisplay_Load(object sender, EventArgs e)
        {
            preview.Reload();
        }


    }
}
