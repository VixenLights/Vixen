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
				case WipeDirection.Up:
					renderNodes = TargetNodes
						.SelectMany(x => x.GetLeafEnumerator())
						.OrderByDescending(x =>
						{
							var prop = x.Properties.Get(LocationDescriptor._typeId);
							if (prop != null)
							{
								return ((LocationData) prop.ModuleData).Y;
							}
							else
								return 1;
						})
						.ThenBy(x =>
						{
							var prop = x.Properties.Get(LocationDescriptor._typeId);
							if (prop != null)
							{
								return ((LocationData) prop.ModuleData).X;
							}
							else
								return 1;
						})
						.GroupBy(x =>
						{
							var prop = x.Properties.Get(LocationDescriptor._typeId);
							if (prop != null)
							{
								return ((LocationData) prop.ModuleData).Y;
							}
							else
								return 1;
						})
						.Distinct();
					RenderNonBurst(tokenSource, renderNodes);
					break;
				case WipeDirection.Down:

					renderNodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator())
						.OrderBy(x =>
						{
							var prop = x.Properties.Get(LocationDescriptor._typeId);
							if (prop != null)
							{
								return ((LocationData) prop.ModuleData).Y;
							}
							else
								return 1;
						})
						.ThenBy(x =>
						{
							var prop = x.Properties.Get(LocationDescriptor._typeId);
							if (prop != null)
							{
								return ((LocationData) prop.ModuleData).X;
							}
							else
								return 1;
						})
						.GroupBy(x =>
						{
							var prop = x.Properties.Get(LocationDescriptor._typeId);
							if (prop != null)
							{
								return ((LocationData) prop.ModuleData).Y;
							}
							else
								return 1;
						})
						.Distinct();
					RenderNonBurst(tokenSource, renderNodes);
					break;
				case WipeDirection.Right:

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
					RenderNonBurst(tokenSource, renderNodes);
					break;
				case WipeDirection.Left:

					renderNodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator())
						.OrderByDescending(x =>
						{
							var prop = x.Properties.Get(LocationDescriptor._typeId);
							if (prop != null)
							{
								return ((LocationData) prop.ModuleData).X;
							}
							return 1;
						})
						.ThenBy(x =>
						{
							var prop = x.Properties.Get(LocationDescriptor._typeId);
							if (prop != null)
							{
								return ((LocationData) prop.ModuleData).Y;
							}
							return 1;
						})
						.GroupBy(x =>
						{
							var prop = x.Properties.Get(LocationDescriptor._typeId);
							if (prop != null)
							{
								return ((LocationData) prop.ModuleData).X;
							}
							return 1;
						})
						.Distinct();
					RenderNonBurst(tokenSource, renderNodes);
					break;
				case WipeDirection.Out:
				case WipeDirection.In:
					RenderBurst(tokenSource, _data.Direction);
					break;
			}
		}

		private void RenderNonBurst(CancellationTokenSource tokenSource, IEnumerable<IGrouping<int, ElementNode>> renderNodes)
		{
			//var pulse = new Pulse.Pulse();
			if (renderNodes != null && renderNodes.Any())
			{
				TimeSpan effectTime = TimeSpan.Zero;
				if (WipeByCount)
				{
					int count = 0;
					double pulseSegment = TimeSpan.Ticks / (double)PassCount * (PulsePercent / 100);

					var maxKey = renderNodes.Select(x => x.Key).Max();
					var minKey = renderNodes.Select(x => x.Key).Min();
					double adjustedMax = maxKey - minKey;

					TimeSpan totalWipeTime = TimeSpan.FromTicks( (long) ( (TimeSpan.Ticks - pulseSegment) / PassCount));
					TimeSpan segmentPulse = TimeSpan.FromTicks((long)pulseSegment);

					while (count < PassCount)
					{
						foreach (var item in renderNodes)
						{
							if (tokenSource != null && tokenSource.IsCancellationRequested) return;

							switch (Direction)
							{
								case WipeDirection.Left:
								case WipeDirection.Up:
									effectTime = TimeSpan.FromTicks((long) (totalWipeTime.Ticks * (1 - (item.Key - minKey) / adjustedMax) + count * totalWipeTime.Ticks));
									break;
								default:
									effectTime = TimeSpan.FromTicks((long)(totalWipeTime.Ticks * (item.Key - minKey) / adjustedMax + count * totalWipeTime.Ticks));
									break;
							}

							foreach (ElementNode element in item)
							{

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
				}
				else
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
				}

			}
		}

		private void RenderBurst(CancellationTokenSource tokenSource, WipeDirection direction)
		{
			switch (direction)
			{

				case WipeDirection.In:
				case WipeDirection.Out:
					break;
				default:
					throw new InvalidOperationException("the RenderBurst method should only be called for Wipe Directions In and Out");
					break;
			}
			var burstNodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator())
													   .Select(s =>
													   {
														   var prop = s.Properties.Get(LocationDescriptor._typeId);
														   if (prop != null)
														   {
															   return new Tuple<ElementNode, int, int, int>(s, ((LocationData)prop.ModuleData).X, ((LocationData)prop.ModuleData).Y, ((LocationData)prop.ModuleData).Z);

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

			var Steps = (int)(Math.Max(maxX - minX, maxY - minY) / 2);

			List<Tuple<int, ElementNode[]>> groups = new List<Tuple<int, ElementNode[]>>();

			for (int i = 0; i < Steps; i++)
			{
				List<ElementNode> elements = new List<ElementNode>();

				var xNodes = burstNodes.Where(x =>
						  (x.Item2 == minX + i || x.Item2 == maxX - i)
						  )
						  .Select(s => s.Item1).ToList();

				var yNodes = burstNodes.Where(x =>
						 (
						 x.Item3 == minY + i ||
						 x.Item3 == maxY - i)
						 )
						 .Select(s => s.Item1).ToList();
				yNodes.RemoveAll(s =>
				{
					var prop = s.Properties.Get(LocationDescriptor._typeId);
					if (prop != null)
					{
						return ((LocationData)prop.ModuleData).X < minX + i || ((LocationData)prop.ModuleData).X > maxX - i;
					}
					return false;
				});
				xNodes.RemoveAll(s =>
				{
					var prop = s.Properties.Get(LocationDescriptor._typeId);
					if (prop != null)
					{
						return ((LocationData)prop.ModuleData).Y < minY + i || ((LocationData)prop.ModuleData).Y > maxY - i;
					}
					return false;
				});
				elements.AddRange(yNodes);
				elements.AddRange(xNodes);

				groups.Add(new Tuple<int, ElementNode[]>(i, elements.ToArray()));
			}
			List<ElementNode[]> renderNodes = new List<ElementNode[]>();
			switch (direction)
			{

				case WipeDirection.In:
					renderNodes = groups.OrderBy(o => o.Item1).Select(s => s.Item2).ToList();
					break;
				case WipeDirection.Out:
					renderNodes = groups.OrderByDescending(o => o.Item1).Select(s => s.Item2).ToList();
					break;
			}

			//var pulse = new Pulse.Pulse();
			if (renderNodes != null && renderNodes.Any())
			{
				TimeSpan effectTime = TimeSpan.Zero;
				if (WipeByCount)
				{
					
					int count = 0;
					double pulseSegment = TimeSpan.Ticks / (double)PassCount * (PulsePercent / 100);
					TimeSpan intervalTime = TimeSpan.FromTicks((long)((TimeSpan.Ticks - pulseSegment) / (renderNodes.Count() * PassCount)));
					TimeSpan segmentPulse = TimeSpan.FromTicks((long)pulseSegment);

					while (count < PassCount)
					{
						foreach (var item in renderNodes)
						{
							if (tokenSource != null && tokenSource.IsCancellationRequested) return;
							EffectIntents result;

							foreach (ElementNode element in item)
							{

								if (tokenSource != null && tokenSource.IsCancellationRequested)
									return;
								if (element != null)
								{
									//pulse.TimeSpan = segmentPulse;
									//pulse.ColorGradient = _data.ColorGradient;
									//pulse.LevelCurve = _data.Curve;
									//pulse.TargetNodes = new ElementNode[] { element };
									//result = pulse.Render();
									result = PulseRenderer.RenderNode(element, _data.Curve, _data.ColorGradient, segmentPulse, HasDiscreteColors);
									result.OffsetAllCommandsByTime(effectTime);
									//bool discreteElement = HasDiscreteColors && ColorModule.isElementNodeDiscreteColored(element);
									//_elementData.Add(IntentBuilder.ConvertToStaticArrayIntents(result, TimeSpan, discreteElement));

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
				else
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
									
									//pulse.TimeSpan = segmentPulse;
									//pulse.ColorGradient = _data.ColorGradient;
									//pulse.LevelCurve = _data.Curve;
									//pulse.TargetNodes = new ElementNode[] { element };
									//result = pulse.Render();
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
				}

			}



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
		[ProviderCategory(@"Type",1)]
		[ProviderDisplayName(@"Type")]
		[ProviderDescription(@"WipeType")]
		[TypeConverter(typeof(BooleanStringTypeConverter))]
		[BoolDescription("Count", "Pulse Length")]
		[PropertyEditor("SelectionEditor")]
		public bool WipeByCount
		{
			get { return _data.WipeByCount; }
			set
			{
				_data.WipeByCount = value;
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
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(4)
			{
				{"PassCount", WipeByCount},
				{"PulsePercent", WipeByCount},
				{"WipeOn", WipeByCount},
				{"WipeOff", WipeByCount},
				{"PulseTime", !WipeByCount},
				{"ColorHandling", Direction != WipeDirection.In && Direction != WipeDirection.Out }
			};
			SetBrowsable(propertyStates);
		}

		#endregion

	}
}