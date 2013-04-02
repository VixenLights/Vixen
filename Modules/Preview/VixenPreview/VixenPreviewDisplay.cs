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

        private void preview_Load(object sender, EventArgs e)
        {
        }

        public void Setup()
        {
            preview.LoadBackground(Data.BackgroundFileName);

            Top = Data.Top;

            Left = Data.Left;

            if (Data.Width > MinimumSize.Width)
                Width = Data.Width;
            else
                Width = MinimumSize.Width;

            if (Data.Height > MinimumSize.Height)
                Height = Data.Height;
            else
                Height = MinimumSize.Height;

            //if (Data.BackgroundAlpha == 0)
            //    Data.BackgroundAlpha = 255;
            //scrollBackgroundAlpha.Value = Data.BackgroundAlpha;
        }

        public void Save()
        {
            Data.Top = Top;
            Data.Left = Left;
            Data.Width = Width;
            Data.Height = Height;
            //Data.BackgroundAlpha = scrollBackgroundAlpha.Value;
        }

        private void VixenPreviewDisplay_Load(object sender, EventArgs e)
        {
            Setup();
        }

        private void timerStatus_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = preview.PixelCount.ToString();
            //toolStripAverageUpdate.Text = "Average: " + Math.Round(VixenPreviewControl.averageUpdateTime).ToString() + "ms";
            toolStripStatusCurrentUpdate.Text = "Last: " + Math.Round(VixenPreviewControl.lastUpdateTime).ToString() + "ms";
            //toolStripStatusLastRenderTime.Text = "Render: " + Math.Round(lastRenderTime).ToString() + "ms";
            toolStripStatusLastRenderTime.Text = "Render: " + Math.Round(preview.lastRenderUpdateTime).ToString() + "ms";
        }

        private void timerRender_Tick(object sender, EventArgs e)
        {
            timerRender.Stop();
            preview.RenderInForeground();
            timerRender.Start();
        }

    }
}
