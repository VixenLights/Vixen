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
	class TimedSequenceElement : TimelineElement
	{
		public TimedSequenceElement(EffectNode effectNode)
			: base()
		{
			StartTime = effectNode.StartTime;
			Duration = effectNode.TimeSpan;
			Effect = effectNode;
			Random r = new Random();
			BackColor = Color.FromArgb(r.Next(256), r.Next(256), r.Next(256));
		}

		public EffectNode Effect { get; set; }

		public override System.Drawing.Bitmap Draw(System.Drawing.Size imageSize)
		{
			Bitmap result = base.Draw(imageSize);
			Graphics g = Graphics.FromImage(result);

			// add text describing the effect
			Font f = new Font("Arial", 7);
			Brush b = new SolidBrush(Color.Black);
			g.DrawString(Effect.Effect.EffectName, f, b, new PointF(5, 5));
			g.DrawString("Start: " + Effect.StartTime.ToString("g"), f, b, new PointF(5, 23));
			g.DrawString("Length: " + Effect.TimeSpan.ToString("g"), f, b, new PointF(5, 35));

			return result;
		}
	}
}
