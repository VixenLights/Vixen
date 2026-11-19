using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Editor.TimedSequenceEditor
{
	internal static class EffectRasterizer
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private static IntentRasterizer intentRasterizer = new IntentRasterizer();

		/// <summary>
		/// Draws the visible portion of an effect thumbnail, sampling target rows and overlapping intent lanes to fit the available pixels.
		/// </summary>
		/// <param name="tsElement">The timeline element whose effect is drawn.</param>
		/// <param name="g">The graphics surface for the visible portion of the thumbnail.</param>
		/// <param name="visibleStartOffset">The start of the visible interval relative to the effect.</param>
		/// <param name="visibleEndOffset">The end of the visible interval relative to the effect.</param>
		/// <param name="overallWidth">The pixel width of the full effect thumbnail.</param>
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
				
				EffectIntents effectIntents = effect.Render();
				if (effectIntents.Count == 0)
					return;

				var elements = effect.TargetNodes.GetElements();
				if (elements.Length == 0)
					return;

				// Partition the target list itself. It can contain many more entries than the rendered intents.
				int rowCount = Math.Min(elements.Length, (int)(height / 2) + 1);
				double heightPerElement = height / rowCount;
				for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
				{
					double y = rowIndex * heightPerElement;
					int firstElement = (int)((long)rowIndex * elements.Length / rowCount);
					int endElement = (int)((long)(rowIndex + 1) * elements.Length / rowCount);
					IntentNodeCollection elementIntents = null;
					for (int i = firstElement; i < endElement; i++)
					{
						var candidate = effectIntents.GetIntentNodesForElement(elements[i].Id);
						if (candidate != null && HasVisibleIntent(candidate, visibleStartOffset, visibleEndOffset))
						{
							elementIntents = candidate;
							break;
						}
					}

					if (elementIntents != null)
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

						// Each displayed lane needs its own pixel band. Preserve the base layer drawn above.
						int laneCount = stack.Count - skip;
						int displayedLanes = Math.Min(laneCount, Math.Max(1, (int)heightPerElement));
						float h = (float)heightPerElement / displayedLanes;
						for (int laneIndex = 0; laneIndex < displayedLanes; laneIndex++)
						{
							int firstLane = skip + (int)((long)laneIndex * laneCount / displayedLanes);
							int endLane = skip + (int)((long)(laneIndex + 1) * laneCount / displayedLanes);
							var intentNodes = stack[firstLane];
							for (int i = firstLane; i + 1 < endLane && !HasVisibleIntent(intentNodes, visibleStartOffset, visibleEndOffset); i++)
								intentNodes = stack[i + 1];

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
														   new RectangleF((float)startPixelX, (float)y + h * laneIndex, (float)widthPixelX,
																		  h), g, visibleIntentStart , visibleIntentEnd);
							}

						}	
					}
					
				}
				//long tRast = sw.ElapsedMilliseconds - tRend;
				//if( tRast > 10)
				//	Logging.Debug(" oh: {0}, rend: {1}, rast: {2}, eff: {3}, node:{4}", tOh, tRend, tRast, effect.EffectName, effect.TargetNodes[0].Name);
			}
		}

		private static bool HasVisibleIntent(IEnumerable<IIntentNode> intents, TimeSpan visibleStart, TimeSpan visibleEnd)
		{
			foreach (var intent in intents)
			{
				if (intent != null && intent.EndTime > visibleStart && intent.StartTime < visibleEnd)
					return true;
			}

			return false;
		}

		private static double _GetPercentage(TimeSpan offset, TimeSpan totalTimeSpan) {
			return (double)offset.Ticks / totalTimeSpan.Ticks;
		}

	}
}