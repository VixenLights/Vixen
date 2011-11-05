using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommonElements.Timeline;
using Vixen.Module.Editor;
using Vixen.Module.Effect;
using VixenModules.Property.RGB;
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
			BackColor = Color.White;
		}

		// copy ctor
		public TimedSequenceElement(TimedSequenceElement other)
			:base(other)
		{
			//TODO: This needs to be a deep-copy of the effect node.
			EffectNode = other.EffectNode;
		}


		public EffectNode EffectNode { get; set; }


		private Bitmap RenderedInset { get; set; }

		public override System.Drawing.Bitmap Draw(System.Drawing.Size imageSize)
		{
			Bitmap result = base.Draw(imageSize);
			Bitmap inset = new Bitmap(imageSize.Width - 4, imageSize.Height - 4);

			// if the inset image may have changed, then re-render it.
			// TODO: this may not be perfectly accurate, since it's possible for someone else to come along and render
			// the effect, which would make it non-dirty, but we don't know about it yet. Not ideal.
			// a big fat TODO: it's slow as shit, as soon as it gets bigger than a few pixels. We need to be able to:
			// (a) turn it off and just render text,
			// (b) do the visual rendering of effects in a separate thread so it can happen slowly and get updated later on.
			if (EffectNode.Effect.IsDirty || (RenderedInset != null && RenderedInset.Width != inset.Width && RenderedInset.Height != inset.Height)) {

				List<ChannelNode> renderNodes = RGBModule.GetVisuallyRenderableChildNodes(EffectNode.Effect.TargetNodes);
				ChannelData effectData = EffectNode.RenderEffectData();
				Dictionary<Channel, List<CommandNode>> renderedData = new Dictionary<Channel, List<CommandNode>>();
				foreach (KeyValuePair<Guid, CommandNode[]> kvp in effectData) {
					Channel channel = VixenSystem.Channels.GetChannel(kvp.Key);
					if (channel == null)
						continue;

					List<CommandNode> nodes = new List<CommandNode>(kvp.Value);
					nodes.Sort();
					renderedData.Add(channel, nodes);
				}


				if (renderNodes.Count > 0 && renderedData != null) {

					using (Graphics g = Graphics.FromImage(inset)) {

						double nodeHeight = inset.Height / (double)renderNodes.Count;
						TimeSpan pixelWidth = TimeSpan.FromTicks(EffectNode.TimeSpan.Ticks / inset.Width);

						int i = 0;
						for (TimeSpan currentTime = TimeSpan.Zero; currentTime < EffectNode.TimeSpan; currentTime += pixelWidth) {

							Dictionary<Channel, Command> requestData = new Dictionary<Channel, Command>();

							foreach (KeyValuePair<Channel, List<CommandNode>> kvp in renderedData) {
								CommandNode cn = kvp.Value.FirstOrDefault(x => x.StartTime <= currentTime && x.EndTime > currentTime);
								if (cn != null)
									requestData.Add(kvp.Key, cn.Command);
							}

							Dictionary<ChannelNode, Color> displayData = RGBModule.MapChannelCommandsToColors(requestData);

							foreach (KeyValuePair<ChannelNode, Color> kvp in displayData) {
								int drawRow = renderNodes.IndexOf(kvp.Key);
								if (drawRow < 0)
									continue;

								double drawTop = drawRow * nodeHeight;

								Pen p = new Pen(kvp.Value);
								g.DrawLine(p, i, (float)drawTop, i, (float)(drawTop + nodeHeight));
							}

							i++;
						}
					}
				}
				// phew! cache what we just rendered, for future use if it's going to be the same size. (Hopefully.)
				RenderedInset = inset;
			} else {
				inset = RenderedInset;
			}

			using (Graphics g = Graphics.FromImage(result)) {
				g.DrawImage(inset, 2, 2);

				// TODO: be able to turn this off somehow, maybe editor options or something?
				// add text describing the effect
				using (Font f = new Font("Arial", 7))
				using (Brush b = new SolidBrush(Color.Gray)) {
					g.DrawString(EffectNode.Effect.EffectName, f, b, new PointF(5, 3));
					g.DrawString("Start: " + EffectNode.StartTime.ToString("g"), f, b, new PointF(60, 3));
					g.DrawString("Length: " + EffectNode.TimeSpan.ToString("g"), f, b, new PointF(60, 16));
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
