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
		private Object lockObject = new Object();
		public TimedSequenceElement(EffectNode effectNode)
			: base()
		{
			StartTime = effectNode.StartTime;
			Duration = effectNode.TimeSpan;
			EffectNode = effectNode;
			BorderColor = Color.Black;
			BackColor = Color.FromArgb(0, 0, 0, 0);
		}

		// copy ctor
		public TimedSequenceElement(TimedSequenceElement other)
			: base(other)
		{
			//TODO: This needs to be a deep-copy of the effect node.
			EffectNode = other.EffectNode;
		}

		private bool ElementTimeHasChangedSinceDraw { get; set; }

		protected override void DrawCanvasContent(Graphics graphics)
		{
			EffectRasterizer.Rasterize(EffectNode.Effect, graphics);
			ElementTimeHasChangedSinceDraw = false;			
		}

		protected override void OnTimeChanged()
		{
			if (this.EffectNode != null) {
				this.EffectNode.StartTime = this.StartTime;
				this.EffectNode.Effect.TimeSpan = this.Duration;
			}

			ElementTimeHasChangedSinceDraw = true;

			base.OnTimeChanged();
		}
	}
}