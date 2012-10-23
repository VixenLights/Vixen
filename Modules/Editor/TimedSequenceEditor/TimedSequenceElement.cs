using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls.Timeline;
using Vixen.Module.Editor;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Commands;
using System.Drawing;
using System.Drawing.Imaging;

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
			BorderColor = Color.Black;
			BackColor = Color.Transparent;
			TextColor = Color.FromArgb(220, 220, 220);
		}

		// copy ctor
		public TimedSequenceElement(TimedSequenceElement other)
			:base(other)
		{
			//TODO: This needs to be a deep-copy of the effect node.
			EffectNode = other.EffectNode;
		}

		[NonSerializedAttribute]
		private EffectNode _effectNode;
		public EffectNode EffectNode
		{
			get { return _effectNode; }
			set { _effectNode = value; }
		}

		public Color TextColor { get; set; }

		private Bitmap CachedCanvasContent { get; set; }

		protected override void DrawCanvasContent(Bitmap canvas)
		{
			// if our cached copy is old or nonexistant, redraw it and save it
			// TODO: this may not be perfectly accurate, since it's possible for someone else to come along and render
			// the effect, which would make it non-dirty, but we don't know about it yet. Not ideal. Add a serial or GUID to effect rendering, to be able to track it.
			if (EffectNode.Effect.IsDirty || CachedCanvasContent == null || CachedCanvasContent.Width != canvas.Width || CachedCanvasContent.Height != canvas.Height) {
				CachedCanvasContent = new Bitmap(canvas.Width, canvas.Height);
				EffectRasterizer effectRasterizer = new EffectRasterizer();
				using (Graphics g = Graphics.FromImage(CachedCanvasContent)) {
					effectRasterizer.Rasterize(EffectNode.Effect, g);
				}
			}

			using (Graphics g = Graphics.FromImage(canvas)) {
				g.DrawImage(CachedCanvasContent, 0, 0);

				// add text describing the effect
				using (Font f = new Font("Arial", 7))
				using (Brush b = new SolidBrush(TextColor)) {
					g.DrawString(EffectNode.Effect.EffectName, f, b, new PointF(5, 3));
					g.DrawString("Start: " + EffectNode.StartTime.ToString("g"), f, b, new PointF(60, 3));
					g.DrawString("Length: " + EffectNode.TimeSpan.ToString("g"), f, b, new PointF(60, 16));
				}
			}

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
