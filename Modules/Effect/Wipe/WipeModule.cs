using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using Vixen.Attributes;
using Vixen.Intent;
using Vixen.Module;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using Vixen.TypeConverters;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Pulse;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using VixenModules.Property.Color;
using VixenModules.Property.Location;
using ZedGraph;

namespace VixenModules.Effect.Wipe
{
	public class WipeModule : BaseEffect
	{
		public WipeModule()
		{
			_data = new WipeData();
			UpdateAttributes();
		}

		private WipeData _data; 
		private EffectIntents _elementData = null;
		private List<WipeClass> _renderElements;
		private readonly int _timeInterval = 50;

		protected override void TargetNodesChanged()
		{
			CheckForInvalidColorData();
		}

		protected override void _PreRender(CancellationTokenSource tokenSource = null)
		{
			_elementData = new EffectIntents();

			RenderNodes(tokenSource);
		}

		private void RenderNodes(CancellationTokenSource tokenSource)
		{
			IEnumerable<IGrouping<int, ElementNode>> renderNodes = null;

			switch (_data.Direction)
			{
				case WipeDirection.Vertical:
					if (!ReverseDirection)
					{
						renderNodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator())
							.OrderBy(x =>
							{
								var prop = x.Properties.Get(LocationDescriptor._typeId);
								if (prop != null)
								{
									return ((LocationData)prop.ModuleData).Y;
								}
								else
									return 1;
							})
							.ThenBy(x =>
							{
								var prop = x.Properties.Get(LocationDescriptor._typeId);
								if (prop != null)
								{
									return ((LocationData)prop.ModuleData).X;
								}
								else
									return 1;
							})
							.GroupBy(x =>
							{
								var prop = x.Properties.Get(LocationDescriptor._typeId);
								if (prop != null)
								{
									return ((LocationData)prop.ModuleData).Y;
								}
								else
									return 1;
							})
							.Distinct();
					}
					else
					{
						renderNodes = TargetNodes
							.SelectMany(x => x.GetLeafEnumerator())
							.OrderByDescending(x =>
							{
								var prop = x.Properties.Get(LocationDescriptor._typeId);
								if (prop != null)
								{
									return ((LocationData)prop.ModuleData).Y;
								}
								else
									return 1;
							})
							.ThenBy(x =>
							{
								var prop = x.Properties.Get(LocationDescriptor._typeId);
								if (prop != null)
								{
									return ((LocationData)prop.ModuleData).X;
								}
								else
									return 1;
							})
							.GroupBy(x =>
							{
								var prop = x.Properties.Get(LocationDescriptor._typeId);
								if (prop != null)
								{
									return ((LocationData)prop.ModuleData).Y;
								}
								else
									return 1;
							})
							.Distinct();
					}
					RenderBasicDirection(tokenSource, renderNodes);

					break;
				case WipeDirection.Horizontal:
					if (!ReverseDirection)
					{
						renderNodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator())
							.OrderBy(x =>
							{
								var prop = x.Properties.Get(LocationDescriptor._typeId);
								if (prop != null)
								{
									return ((LocationData) prop.ModuleData).X;
								}
								else
									return 1;
							})
							.ThenBy(x =>
							{
								var prop = x.Properties.Get(LocationDescriptor._typeId);
								if (prop != null)
								{
									return ((LocationData) prop.ModuleData).Y;
								}
								else
									return 1;
							})
							.GroupBy(x =>
							{
								var prop = x.Properties.Get(LocationDescriptor._typeId);
								if (prop != null)
								{
									return ((LocationData) prop.ModuleData).X;
								}
								else
									return 1;
							})
							.Distinct();
					}
					else
					{
						renderNodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator())
							.OrderByDescending(x =>
							{
								var prop = x.Properties.Get(LocationDescriptor._typeId);
								if (prop != null)
								{
									return ((LocationData)prop.ModuleData).X;
								}
								return 1;
							})
							.ThenBy(x =>
							{
								var prop = x.Properties.Get(LocationDescriptor._typeId);
								if (prop != null)
								{
									return ((LocationData)prop.ModuleData).Y;
								}
								return 1;
							})
							.GroupBy(x =>
							{
								var prop = x.Properties.Get(LocationDescriptor._typeId);
								if (prop != null)
								{
									return ((LocationData)prop.ModuleData).X;
								}
								return 1;
							})
							.Distinct();
					}
					RenderBasicDirection(tokenSource, renderNodes);

					break;
				default:
					RenderAdvancedDirection(tokenSource);
					break;
			}
		}

		private void RenderBasicDirection(CancellationTokenSource tokenSource, IEnumerable<IGrouping<int, ElementNode>> renderNodes)
		{
			//var pulse = new Pulse.Pulse();
			if (renderNodes != null && renderNodes.Any())
			{
				TimeSpan effectTime = TimeSpan.Zero;
				var maxKey = renderNodes.Select(x => x.Key).Max();
				var minKey = renderNodes.Select(x => x.Key).Min();
				double adjustedMax = maxKey - minKey;
				switch (WipeMovement)
				{
					case WipeMovement.Count:
					{
						int count = 0;
						double pulseSegment = TimeSpan.Ticks / (double)PassCount * (PulsePercent / 100);


						TimeSpan totalWipeTime = TimeSpan.FromTicks( (long) ( (TimeSpan.Ticks - pulseSegment) / PassCount));
						TimeSpan segmentPulse = TimeSpan.FromTicks((long)pulseSegment);

						while (count < PassCount)
						{
							foreach (var item in renderNodes)
							{
								if (tokenSource != null && tokenSource.IsCancellationRequested) return;
								if (!ReverseDirection)
								{
									effectTime = TimeSpan.FromTicks((long)(totalWipeTime.Ticks * (item.Key - minKey) / adjustedMax + count * totalWipeTime.Ticks));
								}
								else
								{
									effectTime = TimeSpan.FromTicks((long)(totalWipeTime.Ticks * (1 - (item.Key - minKey) / adjustedMax) + count * totalWipeTime.Ticks));
								}

								foreach (ElementNode element in item)
								{

									var test = item.Count();
										if (tokenSource != null && tokenSource.IsCancellationRequested)
										return;
									if (element != null)
									{
										EffectIntents result;
										if (ColorHandling == ColorHandling.GradientThroughWholeEffect)
										{
											result = PulseRenderer.RenderNode(element, _data.Curve, _data.ColorGradient, segmentPulse, HasDiscreteColors);
											result.OffsetAllCommandsByTime(effectTime);

											if (WipeOff && count == 0)
											{
												foreach (var effectIntent in result.FirstOrDefault().Value)
												{
													_elementData.Add(PulseRenderer.GenerateStartingStaticPulse(element, effectIntent, HasDiscreteColors));
												}
											}

											_elementData.Add(result);

											if (WipeOn && count == PassCount - 1)
											{
												foreach (var effectIntent in result.FirstOrDefault().Value)
												{
													_elementData.Add(PulseRenderer.GenerateExtendedStaticPulse(element, effectIntent, TimeSpan, HasDiscreteColors));
												}
											}
										}
										else
										{
											double positionWithinGroup = (effectTime.Ticks - (double)totalWipeTime.Ticks * count) / totalWipeTime.Ticks;
											if (HasDiscreteColors)
											{
												List<Tuple<Color, float>> colorsAtPosition =
													ColorGradient.GetDiscreteColorsAndProportionsAt(positionWithinGroup);
												foreach (Tuple<Color, float> colorProportion in colorsAtPosition)
												{
													float proportion = colorProportion.Item2;
													// scale all levels of the pulse curve by the proportion that is applicable to this color
													Curve newCurve = new Curve(Curve.Points);
													foreach (PointPair pointPair in newCurve.Points)
													{
														pointPair.Y *= proportion;
													}
													result = PulseRenderer.RenderNode(element, newCurve, new ColorGradient(colorProportion.Item1), segmentPulse, HasDiscreteColors);
													result.OffsetAllCommandsByTime(effectTime);

													if (WipeOff && count == 0)
													{
														foreach (var effectIntent in result.FirstOrDefault().Value)
														{
															_elementData.Add(PulseRenderer.GenerateStartingStaticPulse(element, effectIntent, HasDiscreteColors, new ColorGradient(colorProportion.Item1)));
														}
													}

													if (result.Count > 0) _elementData.Add(result);

													if (WipeOn && count == PassCount - 1)
													{
														foreach (var effectIntent in result.FirstOrDefault().Value)
														{
															_elementData.Add(PulseRenderer.GenerateExtendedStaticPulse(element, effectIntent, TimeSpan, HasDiscreteColors, new ColorGradient(colorProportion.Item1)));
														}
													}
												}
											}
											else
											{
												result = PulseRenderer.RenderNode(element, _data.Curve,
													new ColorGradient(_data.ColorGradient.GetColorAt(positionWithinGroup)), segmentPulse, HasDiscreteColors);
												result.OffsetAllCommandsByTime(effectTime);

												if (WipeOff && count == 0)
												{
													foreach (var effectIntent in result.FirstOrDefault().Value)
													{
														_elementData.Add(PulseRenderer.GenerateStartingStaticPulse(element, effectIntent, HasDiscreteColors));
													}
												}

												_elementData.Add(result);

												if (WipeOn && count == PassCount - 1)
												{
													foreach (var effectIntent in result.FirstOrDefault().Value)
													{
														_elementData.Add(PulseRenderer.GenerateExtendedStaticPulse(element, effectIntent, TimeSpan, HasDiscreteColors));
													}
												}
											}
										}
									}
								}

							}
							count++;

						}

						break;
					}
					case WipeMovement.PulseLength:
					{
						double intervals = (double)PulseTime / (double)renderNodes.Count();
						var intervalTime = TimeSpan.FromMilliseconds(intervals);
						// the calculation above blows up render time/memory as count goes up, try this.. 
						// also fails if intervals is less than half a ms and intervalTime then gets 0
						intervalTime = TimeSpan.FromMilliseconds(Math.Max(intervalTime.TotalMilliseconds, 5));
						TimeSpan segmentPulse = TimeSpan.FromMilliseconds(PulseTime);
						while (effectTime < TimeSpan)
						{
							foreach (var item in renderNodes)
							{
								EffectIntents result;

								if (tokenSource != null && tokenSource.IsCancellationRequested)
									return;

								foreach (ElementNode element in item)
								{
									if (element != null)
									{

										if (tokenSource != null && tokenSource.IsCancellationRequested)
											return;
										result = PulseRenderer.RenderNode(element, _data.Curve, _data.ColorGradient, segmentPulse, HasDiscreteColors);
										result.OffsetAllCommandsByTime(effectTime);
										//bool discreteElement = HasDiscreteColors && ColorModule.isElementNodeDiscreteColored(element);
										//_elementData.Add(IntentBuilder.ConvertToStaticArrayIntents(result, TimeSpan, discreteElement));
										_elementData.Add(result);
									}
								}
								effectTime += intervalTime;
								if (effectTime >= TimeSpan)
									return;
							}
						}

						break;
					}
					case WipeMovement.Movement:
					{
						int pulsePercent = (int) (adjustedMax * (PulsePercent / 100));
						var enumerable = renderNodes.ToList();
						if (Direction == WipeDirection.Vertical) enumerable.Reverse();
							_renderElements = new List<WipeClass>();
							double previousMovement = 2.0;
							TimeSpan startTime = TimeSpan.Zero;
							TimeSpan timeInterval = TimeSpan.FromMilliseconds(_timeInterval);
							int intervals = Convert.ToInt32(Math.Ceiling(TimeSpan.TotalMilliseconds / _timeInterval));

							for (int i = 0; i < intervals; i++)
							{
								double position = (double)100 / intervals * i;
								double movement = MovementCurve.GetValue(position) / 100;
								if (previousMovement != movement)
								{
									if (_renderElements.Count > 0) _renderElements.Last().Duration = startTime - _renderElements.Last().StartTime;

									WipeClass wc = new WipeClass
									{
										ElementIndex = (int)((enumerable.Count - 1) * movement),
										StartTime = startTime,
										Duration = TimeSpan - startTime

									};
									_renderElements.Add(wc);
								}

								previousMovement = movement;
								startTime += timeInterval;
							}

						List<Color> validColor = new List<Color>();
							// Now render element
							foreach (var wipeNode in _renderElements)
							{
								for (int i = 0; i < (int)pulsePercent; i++)
								{
									double curveValue = _data.Curve.GetValue(((double)100 / (int)pulsePercent) * (i + 1));
									Curve curve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { curveValue, curveValue }));
									if (wipeNode.ElementIndex - i > 0)
									{
										IGrouping<int, ElementNode> elementGroup = enumerable[wipeNode.ElementIndex - i];
										if (tokenSource != null && tokenSource.IsCancellationRequested) return;
										EffectIntents result;

										ColorGradient colorGradient = new ColorGradient(Color.Black);
										Color color = new Color();
										foreach (ElementNode item in elementGroup)
										{
											var tests = elementGroup.Count();
											validColor = ColorModule.getValidColorsForElementNode(item, false).ToList();
											if (validColor.Any())
											{
												foreach (ColorPoint color1 in _data.ColorGradient.Colors)
												{
													if (color1.Color.ToRGB().ToArgb() == validColor[0])
													{
														colorGradient = new ColorGradient(validColor[0]);
														HasDiscreteColors = true;
														break;
													}
												}
											}
											else
											{
												color = _data.ColorGradient.GetColorAt((((double)100 / (int)pulsePercent) * (i + 1) / 100));
												colorGradient = new ColorGradient(color);
												HasDiscreteColors = false;
											}

											if (tokenSource != null && tokenSource.IsCancellationRequested)
												return;
											if (item != null)
											{
												result = PulseRenderer.RenderNode(item, curve, colorGradient,
													wipeNode.Duration, HasDiscreteColors);
												result.OffsetAllCommandsByTime(wipeNode.StartTime);
												_elementData.Add(result);
											}
										}
									}
								}
								_renderElements = null;
							}
							break;
					}
				}
			}
		}

		private void RenderAdvancedDirection(CancellationTokenSource tokenSource)
		{
			_renderElements = new List<WipeClass>();
			List<Tuple<ElementNode, int, int, int>> burstNodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator())
				.Select(s =>
				{
					var prop = s.Properties.Get(LocationDescriptor._typeId);
					if (prop != null)
					{
						return new Tuple<ElementNode, int, int, int>(s, ((LocationData) prop.ModuleData).X,
							((LocationData) prop.ModuleData).Y, ((LocationData) prop.ModuleData).Z);

					}

					return new Tuple<ElementNode, int, int, int>(null, -1, -1, -1);
					//return null
				})
				.Where(s => s.Item2 > 0) // Ignore the pseudo null values
				.ToList();

			if (!burstNodes.Any()) return;
			var maxX = burstNodes.Max(m => m.Item2);
			var maxY = burstNodes.Max(m => m.Item3);

			var minX = burstNodes.Min(m => m.Item2);
			var minY = burstNodes.Min(m => m.Item3);

			int xMid = (maxX - minX) / 2;
			int yMid = (maxY - minY) / 2;

			Point centerPoint = new Point((maxX - minX) / 2 + minX, (maxY - minY) / 2 + minY);

			int steps;
			switch (Direction)
			{
				case WipeDirection.Circle:
					steps = (int)(DistanceFromPoint(new Point(maxX, maxY), new Point(minX, minY)) / 2);
					break;
				case WipeDirection.Dimaond:
					steps = (int)( Math.Sqrt(Math.Pow(maxX - minX, 2) + Math.Pow(maxY - minY, 2)) / 1.5);
					break;
				case WipeDirection.DiagonalUp:
				case WipeDirection.DiagonalDown:
					steps = (int) (DistanceFromPoint(new Point(maxX, maxY), new Point(minX, minY)) * 2);
					break;
				default:
					steps = (int) (Math.Max(maxX - minX, maxY - minY) / 2);
					break;
			}

			int pulsePercent = (int)((maxX- minX) * (PulsePercent / 100));
			if (WipeMovement == WipeMovement.Movement) steps += (int)pulsePercent;

			List<Tuple<int, ElementNode[]>> groups = new List<Tuple<int, ElementNode[]>>();

			for (int i = 0; i < steps; i++)
			{
					List<ElementNode> elements = new List<ElementNode>();

					switch (Direction)
					{
						case WipeDirection.Circle:
							foreach (Tuple<ElementNode, int, int, int> node in burstNodes)
							{
								int nodeLocation = (int) DistanceFromPoint(centerPoint, new Point(node.Item2, node.Item3));
								if (nodeLocation == i) elements.Add(node.Item1);
							}

							break;
						case WipeDirection.DiagonalUp:
						foreach (Tuple<ElementNode, int, int, int> node in burstNodes)
							{
								if (ReverseDirection)
								{
									if (node.Item2 - minX + node.Item3 - minY == i) elements.Add(node.Item1);
								}
								else
								{
									if ((node.Item3 - minY) - (node.Item2 - minX) + steps / 2 == i)
										elements.Add(node.Item1);
								}

							}

							break;
						case WipeDirection.DiagonalDown:
							foreach (Tuple<ElementNode, int, int, int> node in burstNodes)
							{
								if (ReverseDirection)
								{
									if ((node.Item3 - minY) - (node.Item2 - minX) + steps / 2 == i)
										elements.Add(node.Item1);
								}
								else
								{
									if (node.Item2 - minX + node.Item3 - minY == i) elements.Add(node.Item1);
								}

							}

							break;
					case WipeDirection.Dimaond:
							foreach (Tuple<ElementNode, int, int, int> node in burstNodes)
							{
								// Do the Down/Left or Up/Right directions
								int nodeLocation = (node.Item3 - minY) - (node.Item2 - minX) +
								                   ((maxX - minX) - (maxY - minY)) / 2;
								if (nodeLocation < 0) nodeLocation = -nodeLocation;
								if (nodeLocation == i &&
								    ((maxY - yMid - node.Item3) <= i && (maxX - xMid - node.Item2) <= i) &&
								    (node.Item3 - minY - yMid) <= i && (node.Item2 - minX - xMid) <= i)
								{
									elements.Add(node.Item1);
								}

								// Do the Down/Right or Up/Left directions
								nodeLocation = (node.Item2 - minX + node.Item3 - minY) -
								               ((maxX - minX) + (maxY - minY)) / 2;
								if (nodeLocation < 0) nodeLocation = -nodeLocation;
								if (nodeLocation == i &&
								    ((maxY - yMid - node.Item3) <= i && (maxX - xMid - node.Item2) <= i) &&
								    (node.Item3 - minY - yMid) <= i && (node.Item2 - minX - xMid) <= i)
								{
									elements.Add(node.Item1);
								}
							}

							break;
						default:

							foreach (Tuple<ElementNode, int, int, int> node in burstNodes)
							{
								// Sets Left and Right side of burst
								if (maxY - yMid - node.Item3 <= i && maxY - yMid - node.Item3 >= -i &&
								    (maxX - xMid - node.Item2 == i || maxX - xMid - node.Item2 == -i))
									elements.Add(node.Item1);

								// Sets Top and Bottom side of burst
							if (maxX - xMid - node.Item2 <= i && maxX - xMid - node.Item2 >= -i &&
								    (maxY - yMid - node.Item3 == i || maxY - yMid - node.Item3 == -i))
									elements.Add(node.Item1);
							}

							break;
					}
					groups.Add(new Tuple<int, ElementNode[]>(i, elements.ToArray()));
			}

			List<ElementNode[]> renderNodes = new List<ElementNode[]>();

			switch (Direction)
			{
				case WipeDirection.Circle:
				case WipeDirection.Dimaond:
				case WipeDirection.Burst:
					renderNodes = !ReverseDirection || WipeMovement == WipeMovement.Movement
						? groups.OrderBy(o => o.Item1).Select(s => s.Item2).ToList()
						: groups.OrderByDescending(o => o.Item1).Select(s => s.Item2).ToList();
					break;
				case WipeDirection.DiagonalUp:
					renderNodes = groups.OrderByDescending(o => o.Item1).Select(s => s.Item2).ToList();
					break;
				case WipeDirection.DiagonalDown:
					renderNodes = groups.OrderBy(o => o.Item1).Select(s => s.Item2).ToList();
					break;
				default:
					renderNodes = ReverseDirection
						? groups.OrderBy(o => o.Item1).Select(s => s.Item2).ToList()
						: groups.OrderByDescending(o => o.Item1).Select(s => s.Item2).ToList();
					break;
			}
	
			if (renderNodes != null && renderNodes.Any())
			{
				switch (WipeMovement)
				{
					case WipeMovement.PulseLength:
						RenderPulseLength(renderNodes, tokenSource);
						break;
					case WipeMovement.Count:
						RenderCount(renderNodes, tokenSource);
						break;
					case WipeMovement.Movement:
						RenderMovement(renderNodes, tokenSource, pulsePercent);
						break;
				}
			}
			
			_renderElements = null;

		}

		private void RenderPulseLength(List<ElementNode[]> renderNodes, CancellationTokenSource tokenSource)
		{
			TimeSpan effectTime = TimeSpan.Zero;
			double intervals = (double)PulseTime / (double)renderNodes.Count();
			var intervalTime = TimeSpan.FromMilliseconds(intervals);
			// the calculation above blows up render time/memory as count goes up, try this.. 
			// also fails if intervals is less than half a ms and intervalTime then gets 0
			intervalTime = TimeSpan.FromMilliseconds(Math.Max(intervalTime.TotalMilliseconds, 5));
			TimeSpan segmentPulse = TimeSpan.FromMilliseconds(PulseTime);
			while (effectTime < TimeSpan)
			{
				foreach (var item in renderNodes)
				{
					EffectIntents result;

					if (tokenSource != null && tokenSource.IsCancellationRequested)
						return;
					foreach (ElementNode element in item)
					{
						if (element != null)
						{

							if (tokenSource != null && tokenSource.IsCancellationRequested)
								return;
							result = PulseRenderer.RenderNode(element, _data.Curve, _data.ColorGradient, segmentPulse, HasDiscreteColors);
							result.OffsetAllCommandsByTime(effectTime);
							_elementData.Add(result);
						}
					}
					effectTime += intervalTime;
					if (effectTime >= TimeSpan)
						return;
				}
			}

		}

		private void RenderCount(List<ElementNode[]> renderNodes, CancellationTokenSource tokenSource)
		{
			TimeSpan effectTime = TimeSpan.Zero;
			int count = 0;
			double pulseSegment = TimeSpan.Ticks / (double)PassCount * (PulsePercent / 100);
			TimeSpan intervalTime = TimeSpan.FromTicks((long)((TimeSpan.Ticks - pulseSegment) / (renderNodes.Count() * PassCount)));
			TimeSpan segmentPulse = TimeSpan.FromTicks((long)pulseSegment);

			while (count < PassCount)
			{
				foreach (ElementNode[] item in renderNodes)
				{
					if (tokenSource != null && tokenSource.IsCancellationRequested) return;
					EffectIntents result;

					foreach (ElementNode element in item)
					{
						if (tokenSource != null && tokenSource.IsCancellationRequested)
							return;
						if (element != null)
						{
							result = PulseRenderer.RenderNode(element, _data.Curve, _data.ColorGradient, segmentPulse, HasDiscreteColors);
							result.OffsetAllCommandsByTime(effectTime);
							if (WipeOff && count == 0)
							{
								foreach (var effectIntent in result.FirstOrDefault().Value)
								{
									_elementData.Add(PulseRenderer.GenerateStartingStaticPulse(element, effectIntent, HasDiscreteColors));
								}
							}

							if (WipeOn && count == PassCount - 1)
							{
								foreach (var effectIntent in result.FirstOrDefault().Value)
								{
									_elementData.Add(PulseRenderer.GenerateExtendedStaticPulse(element, effectIntent, TimeSpan, HasDiscreteColors));
								}
							}

							_elementData.Add(result);
						}
					}
					effectTime += intervalTime;

				}
				count++;
			}
		}
		
		private void RenderMovement(List<ElementNode[]> renderNodes, CancellationTokenSource tokenSource, int pulsePercent)
		{
			double previousMovement = 2.0;
			TimeSpan startTime = TimeSpan.Zero;
			TimeSpan timeInterval = TimeSpan.FromMilliseconds(_timeInterval);
			int intervals = Convert.ToInt32(Math.Ceiling(TimeSpan.TotalMilliseconds / _timeInterval));

			for (int i = 0; i < intervals; i++)
			{
				double position = (double)100 / intervals * i;
				double movement = MovementCurve.GetValue(position) / 100;
				if (previousMovement != movement)
				{
					if (_renderElements.Count > 0) _renderElements.Last().Duration = startTime - _renderElements.Last().StartTime;

					WipeClass wc = new WipeClass
					{
						ElementIndex = (int)((renderNodes.Count - 1) * movement),
						StartTime = startTime,
						Duration = TimeSpan - startTime

					};
					_renderElements.Add(wc);
				}

				previousMovement = movement;
				startTime += timeInterval;
			}

			// Now render element
			foreach (var wipeNode in _renderElements)
			{
				for (int i = 0; i < (int)pulsePercent; i++)
				{
					Color color = _data.ColorGradient.GetColorAt(((double)100 / (int)pulsePercent) * (i + 1) / 100);
					ColorGradient colorGradient = new ColorGradient(color);
					double curveValue = _data.Curve.GetValue(((double)100 / (int)pulsePercent) * (i + 1));
					Curve curve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { curveValue, curveValue }));
					if (wipeNode.ElementIndex - i > 0)
					{
						ElementNode[] elementGroup = renderNodes[wipeNode.ElementIndex - i];
						if (tokenSource != null && tokenSource.IsCancellationRequested) return;
						EffectIntents result;

						foreach (var item in elementGroup)
						{
							if (tokenSource != null && tokenSource.IsCancellationRequested)
								return;
							if (item != null)
							{
								result = PulseRenderer.RenderNode(item, curve, colorGradient,
									wipeNode.Duration, HasDiscreteColors);
								result.OffsetAllCommandsByTime(wipeNode.StartTime);
								_elementData.Add(result);
							}
						}
					}
				}
			}
		}

		private class WipeClass
		{
			public int ElementIndex;
			public TimeSpan StartTime;
			public TimeSpan Duration;
		}

		protected override EffectIntents _Render()
		{
			return _elementData;
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as WipeData;
				CheckForInvalidColorData();
				IsDirty = true;
				UpdateAttributes();
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		private void CheckForInvalidColorData()
		{
			var validColors = GetValidColors();
			if (validColors.Any())
			{
				if (!_data.ColorGradient.GetColorsInGradient().IsSubsetOf(validColors))
				{
					//Our color is not valid for any elements we have.
					//Try to set a default color gradient from our available colors
					_data.ColorGradient = new ColorGradient(validColors.First());
				}
			}
		}

		[Value]
		[ProviderCategory(@"Color",3)]
		[ProviderDisplayName(@"ColorGradient")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(1)]
		public ColorGradient ColorGradient
		{
			get
			{
				return _data.ColorGradient;
			}
			set
			{
				_data.ColorGradient = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 3)]
		[ProviderDisplayName(@"ColorHandling")]
		[ProviderDescription(@"ColorHandling")]
		[PropertyOrder(0)]
		public ColorHandling ColorHandling
		{
			get { return _data.ColorHandling; }
			set
			{
				_data.ColorHandling = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Direction",2)]
		[DisplayName(@"Direction")]
		[ProviderDescription(@"Direction")]
		[PropertyOrder(0)]
		public WipeDirection Direction
		{
			get { return _data.Direction; }
			set
			{
				_data.Direction = value;
				IsDirty = true;
				OnPropertyChanged();
				UpdateAttributes();
				TypeDescriptor.Refresh(this);

			}
		}

		[Value]
		[ProviderCategory(@"Direction", 2)]
		[DisplayName(@"ReverseDirection")]
		[ProviderDescription(@"ReverseDirection")]
		[PropertyOrder(1)]
		public bool ReverseDirection
		{
			get { return _data.ReverseDirection; }
			set
			{
				_data.ReverseDirection = value;
				IsDirty = true;
				OnPropertyChanged();
				UpdateAttributes();
				TypeDescriptor.Refresh(this);

			}
		}

		[Value]
		[ProviderCategory(@"Direction", 2)]
		[DisplayName(@"Movement")]
		[ProviderDescription(@"Movement")]
		[PropertyOrder(2)]
		public Curve MovementCurve
		{
			get { return _data.MovementCurve; }
			set
			{
				_data.MovementCurve = value;
				IsDirty = true;
				OnPropertyChanged();
				UpdateAttributes();
				TypeDescriptor.Refresh(this);

			}
		}

		[Value]
		[ProviderCategory(@"Brightness",4)]
		[ProviderDisplayName(@"Brightness")]
		[ProviderDescription(@"PulseShape")]
		public Curve Curve
		{
			get { return _data.Curve; }
			set
			{
				_data.Curve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Pulse",6)]
		[ProviderDisplayName(@"PulseDuration")]
		[ProviderDescription(@"PulseDuration")]
		public int PulseTime
		{
			get { return _data.PulseTime; }
			set
			{
				_data.PulseTime = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Type", 1)]
		[DisplayName(@"Movement")]
		[ProviderDescription(@"Movement")]
		[PropertyOrder(0)]
		public WipeMovement WipeMovement
		{
			get { return _data.WipeMovement; }
			set
			{
				_data.WipeMovement = value;
				IsDirty = true;
				OnPropertyChanged();
				UpdateAttributes();
				TypeDescriptor.Refresh(this);

			}
		}

		[Value]
		[ProviderCategory(@"Type",1)]
		[ProviderDisplayName(@"WipeCount")]
		[ProviderDescription(@"WipeCount")]
		[PropertyOrder(1)]
		public int PassCount
		{
			get { return _data.PassCount; }
			set
			{
				_data.PassCount = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Type", 1)]
		[ProviderDisplayName(@"WipeOn")]
		[ProviderDescription(@"ExtendPulseEnd")]
		[PropertyOrder(2)]
		public bool WipeOn
		{
			get { return _data.WipeOn; }
			set
			{
				_data.WipeOn = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Type", 1)]
		[ProviderDisplayName(@"WipeOff")]
		[ProviderDescription(@"ExtendPulseStart")]
		[PropertyOrder(3)]
		public bool WipeOff
		{
			get { return _data.WipeOff; }
			set
			{
				_data.WipeOff = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Pulse",7)]
		[ProviderDisplayName(@"PulsePercent")]
		[ProviderDescription(@"WipePulsePercent")]
		[PropertyEditor("SliderDoubleEditor")]
		public double PulsePercent
		{
			get { return _data.PulsePercent; }
			set
			{
				_data.PulsePercent = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/wipe/"; }
		}

		#endregion

		#region Attributes

		private void UpdateAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(9)
			{
				{"PassCount", WipeMovement == WipeMovement.Count},
				{"PulsePercent", WipeMovement != WipeMovement.PulseLength},
				{"WipeOn", WipeMovement == WipeMovement.Count},
				{"WipeOff", WipeMovement == WipeMovement.Count},
				{"PulseTime", WipeMovement == WipeMovement.PulseLength},
				{"ColorHandling", Direction != WipeDirection.Burst && WipeMovement != WipeMovement.Movement},
				{"WipeMovementDirection", WipeMovement == WipeMovement.Movement },
				{"MovementCurve", WipeMovement == WipeMovement.Movement },
				{"ReverseDirection", WipeMovement != WipeMovement.Movement }
			};
			SetBrowsable(propertyStates);
		}

		#endregion

	}
}