using System;
using System.Collections.Generic;
using System.Drawing.Text;
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
using Element = Common.Controls.Timeline.Element;

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
			//BackColor = Color.FromArgb(50, 255, 255, 255);
			BackColor = Color.FromArgb(0, 0, 0, 0);
			TextColor = Color.FromArgb(60, 60, 60);
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

		private bool ElementTimeHasChangedSinceDraw { get; set; }

		protected override void DrawCanvasContent(Graphics graphics)
		{
			// if our cached copy is old or nonexistant, redraw it and save it
			// TODO: this may not be perfectly accurate, since it's possible for someone else to come along and render
			// the effect, which would make it non-dirty, but we don't know about it yet. Not ideal. Add a serial or GUID to effect rendering, to be able to track it.
			if (EffectNode.Effect.IsDirty || CachedCanvasContent == null ||
				CachedCanvasContent.Width != (int)graphics.VisibleClipBounds.Width ||
				CachedCanvasContent.Height != (int)graphics.VisibleClipBounds.Height)
			{
				CachedCanvasContent = new Bitmap((int)graphics.VisibleClipBounds.Width, (int)graphics.VisibleClipBounds.Height);
				EffectRasterizer effectRasterizer = new EffectRasterizer();
				using (Graphics g = Graphics.FromImage(CachedCanvasContent)) {
					effectRasterizer.Rasterize(EffectNode.Effect, g);
				}
			}

			graphics.DrawImage(CachedCanvasContent, 0, 0);

			// add text describing the effect
			using (Font f = new Font("Arial", 7))
			using (Brush b = new SolidBrush(TextColor)) {
				graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
				graphics.DrawString(EffectNode.Effect.EffectName, f, b, new RectangleF(5, 3, 50, 12));
				graphics.DrawString("Start: " + EffectNode.StartTime.ToString(@"m\:ss\.fff"), f, b, new PointF(60, 3));
				graphics.DrawString("Length: " + EffectNode.TimeSpan.ToString(@"m\:ss\.fff"), f, b, new PointF(60, 16));
			}

			ElementTimeHasChangedSinceDraw = false;
		}
		protected override bool IsCanvasContentCurrent(Size imageSize)
		{
			return base.IsCanvasContentCurrent(imageSize) && !EffectNode.Effect.IsDirty && !ElementTimeHasChangedSinceDraw && CachedCanvasContent != null;
		}

        protected override void OnTimeChanged()
        {
            if (this.EffectNode != null)
            {
                this.EffectNode.StartTime = this.StartTime;
                this.EffectNode.Effect.TimeSpan = this.Duration;
            }

        	ElementTimeHasChangedSinceDraw = true;

            base.OnTimeChanged();
        }
	}
}
