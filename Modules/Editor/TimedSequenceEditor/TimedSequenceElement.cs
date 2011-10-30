using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommonElements.Timeline;
using Vixen.Module.Editor;
using Vixen.Sys;
using System.Drawing;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public class TimedSequenceElement : Element
	{
		public TimedSequenceElement(EffectNode effectNode)
			: base()
		{
			StartTime = effectNode.StartTime;
			Duration = effectNode.TimeSpan;
			EffectNode = effectNode;
			Random r = new Random();
			BackColor = Color.FromArgb(r.Next(256), r.Next(256), r.Next(256));
		}


		public EffectNode EffectNode { get; set; }

		public override System.Drawing.Bitmap Draw(System.Drawing.Size imageSize)
		{
			Bitmap result = base.Draw(imageSize);

            using (Graphics g = Graphics.FromImage(result))
            {

                // add text describing the effect
                using (Font f = new Font("Arial", 7))
                using (Brush b = new SolidBrush(Color.Black))
                {
                    g.DrawString(EffectNode.Effect.EffectName, f, b, new PointF(5, 3));
                    g.DrawString("Start: " + EffectNode.StartTime.ToString("g"), f, b, new PointF(50, 3));
                    g.DrawString("Length: " + EffectNode.TimeSpan.ToString("g"), f, b, new PointF(50, 16));
                }
            }
            
			return result;
		}


        protected override void OnTimeChanged()
        {
            if (this.EffectNode != null)
            {
                this.EffectNode.StartTime = this.StartTime;
                this.EffectNode.Effect.TimeSpan = this.Duration;
            }

            base.OnTimeChanged();
        }
	}
}
