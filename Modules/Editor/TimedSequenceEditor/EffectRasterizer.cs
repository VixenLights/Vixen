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
	internal class EffectRasterizer
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public void Rasterize(IEffectModuleInstance effect, Graphics g)
		{
			double width = g.VisibleClipBounds.Width;
			double height = g.VisibleClipBounds.Height;

			// As recommended by R#
			if (Math.Abs(width - 0) < double.Epsilon || Math.Abs(height - 0) < double.Epsilon) return;

			Element[] elements = effect.TargetNodes.GetElements();
			double heightPerElement = height / elements.Length;

			//Stopwatch timer = new Stopwatch();
			//timer.Start();
			EffectIntents effectIntents = effect.Render();
			//timer.Stop();
			//Console.WriteLine("Effect Render:" + timer.ElapsedMilliseconds);

			// Is this a Nutcracker effect?
			//if (effect.TypeId.ToString().ToLower() == "82334cb3-9472-42fe-a221-8482f5c731db")
			//{
			//    g.FillRectangle(Brushes.Purple, new Rectangle(0, 0, (int)width, (int)height));
			//    //intentRasterizer.Rasterize(elementIntentNode.Intent, new RectangleF((float)startPixelX, (float)y, (float)widthPixelX, (float)heightPerElement), g);
			//}
			//else
			//{
			//timer.Reset();
			//timer.Start();
			using (IntentRasterizer intentRasterizer = new IntentRasterizer()) {
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

								// these were options to try and get the rasterization to 'overlap' slightly to remove vertical splits between intents.
								// However, with the change to doubles and more precision, the issue seems to have disappeared. Nevertheless, leave these here.
								//startPixelX -= 0.2;
								//widthPixelX += 0.4;
								//startPixelX = Math.Floor(startPixelX);
								//widthPixelX = Math.Ceiling(widthPixelX);

								intentRasterizer.Rasterize(elementIntentNode.Intent,
														   new RectangleF((float)startPixelX, (float)y, (float)widthPixelX,
																		  (float)heightPerElement), g);
							}
						}
					}
					y += heightPerElement;
				}
				//timer.Stop();
				//Console.WriteLine("Effect Draw:" + timer.ElapsedMilliseconds);
				//}
			}
		}

		private double _GetPercentage(TimeSpan offset, TimeSpan totalTimeSpan) {
			return (double)offset.Ticks / totalTimeSpan.Ticks;
		}

	}
}