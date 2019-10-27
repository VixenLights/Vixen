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

		public static void Rasterize(TimedSequenceElement tsElement, Graphics g, TimeSpan visibleStartOffset, TimeSpan visibleEndOffset, int overallWidth)
		{
			//var sw = new System.Diagnostics.Stopwatch(); sw.Start();
			IEffectModuleInstance effect = tsElement.EffectNode.Effect;
			if (effect.ForceGenerateVisualRepresentation || Vixen.Common.Graphics.DisableEffectsEditorRendering)
			{

				var startX = (int)((visibleStartOffset.Ticks / (float)tsElement.Duration.Ticks) * overallWidth);
				effect.GenerateVisualRepresentation(g, new Rectangle(-startX, 0, overallWidth, (int)g.VisibleClipBounds.Height));
			} else {
				double width = g.VisibleClipBounds.Width;
				double height = g.VisibleClipBounds.Height;

				// As recommended by R#
				if (Math.Abs(width - 0) < double.Epsilon || Math.Abs(height - 0) < double.Epsilon)
					return;
				
				// limit the number of 'rows' rasterized
				int tmpsiz = (int)(height / 2) + 1;

				EffectIntents effectIntents = effect.Render();

				int count = effectIntents.Count;

				int skipCount = count>tmpsiz ? count / tmpsiz: 1;

				double y = 0;
				int ctr = 0;

				var elements = effect.TargetNodes.GetElements();

				double heightPerElement = height/ ( elements.Length / skipCount);
				
				foreach (var element in elements)
				{
					if(ctr++ % skipCount != 0) continue;
					
					IntentNodeCollection elementIntents = effectIntents.GetIntentNodesForElement(element.Id);//effectIntents.GetIntentNodesForElement(element.Id);
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
							
							intentRasterizer.Rasterize(stack[0][0].Intent,
													   new RectangleF(0, (float)y, (float)width,
																	  (float)heightPerElement), g, visibleStartOffset,stack[0][0].TimeSpan);
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
								
								if(elementIntentNode.EndTime<visibleStartOffset || elementIntentNode.StartTime>visibleEndOffset) continue;

								TimeSpan visibleIntentStart = elementIntentNode.StartTime < visibleStartOffset
									? visibleStartOffset - elementIntentNode.StartTime
									: TimeSpan.Zero;

								TimeSpan visibleIntentEnd = elementIntentNode.EndTime > visibleEndOffset
									? visibleEndOffset - elementIntentNode.StartTime
									: elementIntentNode.TimeSpan;

								double startPixelX = overallWidth * _GetPercentage(elementIntentNode.StartTime, effect.TimeSpan);
								double widthPixelX = overallWidth * _GetPercentage(elementIntentNode.TimeSpan, effect.TimeSpan);

								widthPixelX = widthPixelX * ((visibleIntentEnd.TotalMilliseconds - visibleIntentStart.TotalMilliseconds) / elementIntentNode.TimeSpan.TotalMilliseconds);
								if (visibleIntentStart == TimeSpan.Zero)
								{
									startPixelX -= overallWidth*_GetPercentage(visibleStartOffset, effect.TimeSpan);
								}
								else
								{
									startPixelX = 0;
								}
								

								intentRasterizer.Rasterize(elementIntentNode.Intent,
														   new RectangleF((float)startPixelX, (float)y+h*stackCount , (float)widthPixelX,
																		  h), g, visibleIntentStart , visibleIntentEnd);
							}

							stackCount++;
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