using System;
using System.Drawing;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Editor.TimedSequenceEditor {
	class EffectRasterizer {
		public void Rasterize(IEffectModuleInstance effect, Graphics g) {
			float width = g.VisibleClipBounds.Width;
			float height = g.VisibleClipBounds.Height;

			// As recommended by R#
			if(Math.Abs(width - 0) < float.Epsilon || Math.Abs(height - 0) < float.Epsilon) return;

			Channel[] channels = effect.TargetNodes.GetChannels();
			float heightPerChannel = height / channels.Length;

			EffectIntents effectIntents = effect.Render();

			IntentRasterizer intentRasterizer = new IntentRasterizer();
			float y = 0;
			foreach(Channel channel in channels) {
				var channelIntents = effectIntents.GetIntentNodesForChannel(channel.Id);
				foreach(IntentNode channelIntentNode in channelIntents) {
					float startPixelX = width * _GetPercentage(channelIntentNode.StartTime, effect.TimeSpan);
					float widthPixelX = width * _GetPercentage(channelIntentNode.TimeSpan, effect.TimeSpan);
					intentRasterizer.Rasterize(channelIntentNode.Intent, new RectangleF(startPixelX, y, widthPixelX, heightPerChannel), g);
				}
				y += heightPerChannel;
			}
		}

		private float _GetPercentage(TimeSpan offset, TimeSpan totalTimeSpan) {
			return (float)(offset.TotalMilliseconds / totalTimeSpan.TotalMilliseconds);
		}
	}
}
