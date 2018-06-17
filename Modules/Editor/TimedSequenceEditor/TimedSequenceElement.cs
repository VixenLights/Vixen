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
		}

		// copy ctor
		public TimedSequenceElement(TimedSequenceElement other)
			: base(other)
		{
			//TODO: This needs to be a deep-copy of the effect node.
			EffectNode = other.EffectNode;
		}

		private bool ElementTimeHasChangedSinceDraw { get; set; }

		protected override void DrawCanvasContent(Graphics g, TimeSpan startTime, TimeSpan endTime, int overallWidth, bool redBorder)
		{
			EffectRasterizer.Rasterize(this, g, startTime, endTime, overallWidth);
			ElementTimeHasChangedSinceDraw = false;			
		}

		protected override void OnTimeChanged()
		{
			if (!SuspendEvents)
			{
				if (this.EffectNode != null)
				{
					this.EffectNode.StartTime = this.StartTime;
					this.EffectNode.Effect.TimeSpan = this.Duration;
					EffectNode.Effect.StartTime = StartTime;
				}

				ElementTimeHasChangedSinceDraw = true;

				base.OnTimeChanged();	
			}
			
		}

		protected override void OnTargetNodesChanged()
		{
			if (!SuspendEvents)
			{
				if (EffectNode != null)
				{
					EffectNode.Effect.TargetNodes = TargetNodes;
				}
				base.OnTargetNodesChanged();	
			}
			
		}
	}
}