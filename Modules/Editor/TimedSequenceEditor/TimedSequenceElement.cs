using System;
using Vixen.Sys;
using System.Drawing;
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