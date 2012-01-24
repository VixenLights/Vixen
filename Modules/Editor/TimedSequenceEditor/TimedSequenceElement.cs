using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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


		public EffectNode EffectNode { get; set; }

		public Color TextColor { get; set; }

		private Bitmap RenderedInset { get; set; }

		private AsyncRefreshRenderedInsetDelegate dlgt = new AsyncRefreshRenderedInsetDelegate(new AsyncRefreshRenderedInset().RefreshRenderedInset);

		private Size currentSize;

		public override System.Drawing.Bitmap Draw(System.Drawing.Size imageSize)
		{
			Bitmap result = base.Draw(imageSize);
			Bitmap inset = RenderedInset;
			int newWidth = imageSize.Width - 4;
			int newHeight = imageSize.Height - 4;
			currentSize = imageSize;
			//if (inset == null) {
			//    inset = new Bitmap(newWidth, newHeight);
			//    dlgt.BeginInvoke(this, imageSize, new AsyncCallback(InsertNewRenderedInset), dlgt);
			//} else if (EffectNode.Effect.IsDirty || (inset.Width != newWidth || inset.Height != newHeight)) {
			//    dlgt.BeginInvoke(this, imageSize, new AsyncCallback(InsertNewRenderedInset), dlgt);
			//}

			//inset = new Bitmap(newWidth, newHeight);

			// if the inset image may have changed, then re-render it.
			// TODO: this may not be perfectly accurate, since it's possible for someone else to come along and render
			// the effect, which would make it non-dirty, but we don't know about it yet. Not ideal.
			// a big fat TODO: it's slow as shit, as soon as it gets bigger than a few pixels. We need to be able to:
			// (a) turn it off and just render text,
			// (b) do the visual rendering of effects in a separate thread so it can happen slowly and get updated later on.

			using (Graphics g = Graphics.FromImage(result)) {
				if (inset == null) {
					//VixenSystem.Logging.Debug("TimedSequenceElement: null inset!");
					using (Brush b = new SolidBrush(Color.Black)) {
						g.FillRectangle(b, 2, 2, imageSize.Width - 4, imageSize.Height - 4);
					}
				} else
					g.DrawImage(inset, 2, 2);

				// TODO: be able to turn this off somehow, maybe editor options or something?
				// add text describing the effect
				using (Font f = new Font("Arial", 7))
				using (Brush b = new SolidBrush(TextColor)) {
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

		private void InsertNewRenderedInset(IAsyncResult ar) {
			AsyncRefreshRenderedInsetDelegate dlgt = (AsyncRefreshRenderedInsetDelegate)ar.AsyncState;
			Bitmap inset = dlgt.EndInvoke(ar);
			if (inset != null) {
				lock (EffectNode) {
					RenderedInset = inset;
				}
				OnContentChanged();
			}
		}

		public void RenderInsetImage()
		{
			dlgt.BeginInvoke(this, currentSize, new AsyncCallback(InsertNewRenderedInset), dlgt);
		}

		delegate Bitmap AsyncRefreshRenderedInsetDelegate(TimedSequenceElement elem, Size imageSize);

		class AsyncRefreshRenderedInset {

			public Bitmap RefreshRenderedInset(TimedSequenceElement elem, Size imageSize)
			{
				Bitmap inset;
				//EffectNode renderingEffectNode = new EffectNode(elem.EffectNode);

				lock (elem.EffectNode) {

					if (imageSize != elem.currentSize)
						return null;
					if (elem.currentSize.Width <= 4 || elem.currentSize.Height <= 4)
						return null;

					inset = new Bitmap(elem.currentSize.Width - 4, elem.currentSize.Height - 4);
					List<ChannelNode> renderNodes = RGBModule.FindAllRenderableChildren(elem.EffectNode.Effect.TargetNodes);
					ChannelData effectData = elem.EffectNode.RenderEffectData();
					Dictionary<Channel, List<CommandNode>> renderedData = new Dictionary<Channel, List<CommandNode>>();
					foreach (KeyValuePair<Guid, CommandNode[]> kvp in effectData.ToArray()) {
						Channel channel = VixenSystem.Channels.GetChannel(kvp.Key);
						if (channel == null)
							continue;

						List<CommandNode> nodes = new List<CommandNode>(kvp.Value);
						nodes.Sort();
						renderedData.Add(channel, nodes);
					}

					if (renderNodes.Count > 0 && renderedData != null) {
						double nodeHeight = inset.Height / (double)renderNodes.Count;
						TimeSpan pixelWidth = TimeSpan.FromTicks(elem.EffectNode.TimeSpan.Ticks / inset.Width);

						int i = 0;
						for (TimeSpan currentTime = TimeSpan.Zero; currentTime < elem.EffectNode.TimeSpan; currentTime += pixelWidth) {

							//if (imageSize != elem.currentSize)
							//    return null;

							Dictionary<Channel, Command> requestData = new Dictionary<Channel, Command>();

							foreach (KeyValuePair<Channel, List<CommandNode>> kvp in renderedData) {
								IEnumerable<CommandNode> validNodes = kvp.Value.Where(x => x.StartTime <= currentTime && x.EndTime > currentTime);
								if (validNodes.Count() > 0) {
									requestData.Add(kvp.Key, Command.Combine(validNodes.Select(x => x.Command)));
								}
							}

							Dictionary<ChannelNode, Color> displayData = RGBModule.MapChannelCommandsToColors(requestData);

							foreach (KeyValuePair<ChannelNode, Color> kvp in displayData) {
								int drawRow = renderNodes.IndexOf(kvp.Key);
								if (drawRow < 0)
									continue;

								double drawTop = drawRow * nodeHeight;

								Pen p = new Pen(kvp.Value);

								using (Graphics g = Graphics.FromImage(inset)) {
									g.DrawLine(p, i, (float)drawTop, i, (float)(drawTop + nodeHeight));
								}
							}

							i++;

						}
					}
				}
				return inset;
			}
		}
	}
}
