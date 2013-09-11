using System;
using System.Drawing;
using Vixen.Module.Effect;
using Vixen.Sys;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace VixenModules.Editor.TimedSequenceEditor
{
	internal static class EffectRasterizer
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private static IntentRasterizer intentRasterizer = new IntentRasterizer();

		public static void Rasterize(IEffectModuleInstance effect, Graphics g)
		{

			if (effect.EffectName.Equals("RDS") || Vixen.Common.Graphics.DisableEffectsEditorRendering) {
				effect.GenerateVisualRepresentation(g, new Rectangle(0, 0, (int)g.VisibleClipBounds.Width, (int)g.VisibleClipBounds.Height));
			} else {
				double width = g.VisibleClipBounds.Width;
				double height = g.VisibleClipBounds.Height;

				// As recommended by R#
				if (Math.Abs(width - 0) < double.Epsilon || Math.Abs(height - 0) < double.Epsilon)
					return;

				Element[] elements = effect.TargetNodes.GetElements();
				double heightPerElement = height / elements.Length;


				EffectIntents effectIntents = effect.Render();

				double y = 0;
				foreach (Element element in elements) {
					//Getting exception on null elements here... A simple check to look for these null values and ignore them
					if (element != null) {
						IntentNodeCollection elementIntents = effectIntents.GetIntentNodesForElement(element.Id);
						if (elementIntents != null) {
							foreach (IntentNode elementIntentNode in elementIntents) {
								if (elementIntentNode == null) {
									Logging.Error("Error: elementIntentNode was null when Rasterizing an effect (ID: " + effect.InstanceId + ")");
									continue;
								}
								double startPixelX = width * _GetPercentage(elementIntentNode.StartTime, effect.TimeSpan);
								double widthPixelX = width * _GetPercentage(elementIntentNode.TimeSpan, effect.TimeSpan);


								intentRasterizer.Rasterize(elementIntentNode.Intent,
														   new RectangleF((float)startPixelX, (float)y, (float)widthPixelX,
																		  (float)heightPerElement), g);
							}
						}
					}
					y += heightPerElement;

				}
			}
		}

		private static double _GetPercentage(TimeSpan offset, TimeSpan totalTimeSpan) {
			return (double)offset.Ticks / totalTimeSpan.Ticks;
		}

	}
}