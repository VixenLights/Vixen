using System;
using System.Drawing;
using Vixen.Module.Effect;
using Vixen.Sys;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
			//var sw = new System.Diagnostics.Stopwatch(); sw.Start();

			if (effect.ForceGenerateVisualRepresentation || Vixen.Common.Graphics.DisableEffectsEditorRendering) {
				effect.GenerateVisualRepresentation(g, new Rectangle(0, 0, (int)g.VisibleClipBounds.Width, (int)g.VisibleClipBounds.Height));
			} else {
				double width = g.VisibleClipBounds.Width;
				double height = g.VisibleClipBounds.Height;

				// As recommended by R#
				if (Math.Abs(width - 0) < double.Epsilon || Math.Abs(height - 0) < double.Epsilon)
					return;

				IEnumerable<Element> elements = effect.TargetNodes.GetElements();

				// limit the number of 'rows' rasterized
				int tmpsiz = (int)(height / 2) + 1;
				if (elements.Count() > tmpsiz)
				{
					int skip = elements.Count() / tmpsiz;
					elements = elements.Where((element, index) => (index + 1) % skip == 0);
					}

				double heightPerElement = height / elements.Count();

				//long tOh = sw.ElapsedMilliseconds;
				EffectIntents effectIntents = effect.Render();

				//long tRend = sw.ElapsedMilliseconds - tOh;
				double y = 0;
				foreach (Element element in elements)
				{
					//Getting exception on null elements here... A simple check to look for these null values and ignore them
					if (element != null) {
						IntentNodeCollection elementIntents = effectIntents.GetIntentNodesForElement(element.Id);
						if (elementIntents != null && elementIntents.Count > 0)
						{
							//Determine if we have parallel intents used on this element for this effect.
							var stack = new List<List<IIntentNode>> {new List<IIntentNode> {elementIntents[0]}};
							for (int i = 1; i < elementIntents.Count; i++)
							{
								bool add = true;
								foreach (List<IIntentNode> t in stack)
								{
									if (elementIntents[i].StartTime >= t.Last().EndTime)
									{
										t.Add(elementIntents[i]);
										add = false;
										break;
									}
								}
								if (add) stack.Add(new List<IIntentNode> { elementIntents[i] });
							}
							int skip = 0;
							//Check for base or minimum level intent.
							if (stack.Count > 1 && stack[0].Count == 1 && stack[0][0].TimeSpan.Equals(effect.TimeSpan) && stack[1][0].TimeSpan != effect.TimeSpan)
							{
								//this is most likely a underlying base intent like chase, spin and twinkle use to provide a minimum value
								//so render it full size as it is usually a lower intensity and the pulses can be drawn over the top and look nice.
								double startPixelX = width * _GetPercentage(stack[0][0].StartTime, effect.TimeSpan);
								double widthPixelX = width * _GetPercentage(stack[0][0].TimeSpan, effect.TimeSpan);
								intentRasterizer.Rasterize(stack[0][0].Intent,
														   new RectangleF((float)startPixelX, (float)y, (float)widthPixelX,
																		  (float)heightPerElement), g);
								skip=1;
							}

							float h = (float)heightPerElement / (stack.Count-skip);
							int stackCount = 0;
							//Now we have a good idea what our element should look like, lets draw it up
							foreach (List<IIntentNode> intentNodes in stack.Skip(skip))
							{
								foreach (IntentNode elementIntentNode in intentNodes)
								{
									if (elementIntentNode == null)
									{
										Logging.Error("Error: elementIntentNode was null when Rasterizing an effect (ID: " + effect.InstanceId + ")");
										continue;
									}
									double startPixelX = width * _GetPercentage(elementIntentNode.StartTime, effect.TimeSpan);
									double widthPixelX = width * _GetPercentage(elementIntentNode.TimeSpan, effect.TimeSpan);
									intentRasterizer.Rasterize(elementIntentNode.Intent,
															   new RectangleF((float)startPixelX, (float)y+h*stackCount , (float)widthPixelX,
																			  h), g);
								}

								stackCount++;
							}	
						}
					}
					y += heightPerElement;
				}
				//long tRast = sw.ElapsedMilliseconds - tRend;
				//if( tRast > 10)
				//	Logging.Debug(" oh: {0}, rend: {1}, rast: {2}, eff: {3}, node:{4}", tOh, tRend, tRast, effect.EffectName, effect.TargetNodes[0].Name);
			}
		}

		private static double _GetPercentage(TimeSpan offset, TimeSpan totalTimeSpan) {
			return (double)offset.Ticks / totalTimeSpan.Ticks;
		}

	}
}