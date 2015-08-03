using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace VixenModules.EffectEditor.AudioMeterEffectEditor
{
    public partial class VerticalMeterControl : System.Windows.Forms.ProgressBar
    {

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                base.SetStyle(ControlStyles.UserPaint, true);
                cp.Style |= 0x04;
                return cp;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // None... Helps control the flicker.
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            const int inset = 2; // A single inset value to control teh sizing of the inner rect.

            using (Image offscreenImage = new Bitmap(this.Width, this.Height))
            {
                using (Graphics offscreen = Graphics.FromImage(offscreenImage))
                {
                    Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);

                    if (ProgressBarRenderer.IsSupported)
                        ProgressBarRenderer.DrawVerticalBar(offscreen, rect);

                    rect.Inflate(new Size(-inset, -inset)); // Deflate inner rect.
                    rect.Height = (int)(rect.Height * ((double)this.Value / this.Maximum));
                    if (rect.Height == 0) rect.Height = 1; // Can't draw rec with width of 0.               

                    LinearGradientBrush brush = new LinearGradientBrush(rect, this.ForeColor, this.ForeColor, LinearGradientMode.Horizontal);
                    offscreen.FillRectangle(brush, inset, this.Height - inset - rect.Height, rect.Width, rect.Height);

                    e.Graphics.DrawImage(offscreenImage, 0, 0);
                    offscreenImage.Dispose();
                }
            }
        }

        public VerticalMeterControl()
        {
            InitializeComponent();
        }

        private void VerticalMeter_Load(object sender, EventArgs e)
        {

        }
    }
}
