using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Pulse;
using VixenModules.Property.Location;
using System.Drawing;
using System.Drawing.Design;
using VixenModules.EffectEditor.TypeConverters;
using VixenModules.Property.Color;

namespace VixenModules.Effect.Wipe
{
	public class WipeModule : EffectModuleInstanceBase
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

			IEnumerable<IGrouping<int, ElementNode>> renderNodes = null;


			var enumerator = TargetNodes.SelectMany(x => x.GetLeafEnumerator());
			var b = enumerator;
			switch (_data.Direction)
			{
				case WipeDirection.Up:
					renderNodes = TargetNodes
												.SelectMany(x => x.GetLeafEnumerator())
												.OrderByDescending(x =>
												{
													var prop = x.Properties.Get(VixenModules.Property.Location.LocationDescriptor._typeId);
													if (prop != null)
													{
														return ((LocationData)prop.ModuleData).Y;
													}
													else
														return 1;
												})
												.ThenBy(x =>
												{
													var prop = x.Properties.Get(VixenModules.Property.Location.LocationDescriptor._typeId);
													if (prop != null)
													{
														return ((LocationData)prop.ModuleData).X;
													}
													else
														return 1;
												})
												.GroupBy(x =>
												{
													var prop = x.Properties.Get(VixenModules.Property.Location.LocationDescriptor._typeId);
													if (prop != null)
													{
														return ((LocationData)prop.ModuleData).Y;
													}
													else
														return 1;
												})
												.Distinct();
					break;
				case WipeDirection.Down:

					renderNodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator())
												.OrderBy(x =>
												{
													var prop = x.Properties.Get(VixenModules.Property.Location.LocationDescriptor._typeId);
													if (prop != null)
													{
														return ((LocationData)prop.ModuleData).Y;
													}
													else
														return 1;
												})
												.ThenBy(x =>
												{
													var prop = x.Properties.Get(VixenModules.Property.Location.LocationDescriptor._typeId);
													if (prop != null)
													{
														return ((LocationData)prop.ModuleData).X;
													}
													else
														return 1;
												})
												.GroupBy(x =>
												{
													var prop = x.Properties.Get(VixenModules.Property.Location.LocationDescriptor._typeId);
													if (prop != null)
													{
														return ((LocationData)prop.ModuleData).Y;
													}
													else
														return 1;
												})
												.Distinct();
					break;
				case WipeDirection.Right:

					renderNodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator())
												.OrderBy(x =>
												{
													var prop = x.Properties.Get(VixenModules.Property.Location.LocationDescriptor._typeId);
													if (prop != null)
													{
														return ((LocationData)prop.ModuleData).X;
													}
													else
														return 1;
												})
												.ThenBy(x =>
												{
													var prop = x.Properties.Get(VixenModules.Property.Location.LocationDescriptor._typeId);
													if (prop != null)
													{
														return ((LocationData)prop.ModuleData).Y;
													}
													else
														return 1;
												})
												.GroupBy(x =>
												{
													var prop = x.Properties.Get(VixenModules.Property.Location.LocationDescriptor._typeId);
													if (prop != null)
													{
														return ((LocationData)prop.ModuleData).X;
													}
													else
														return 1;
												})
												.Distinct();
					break;
				case WipeDirection.Left:

					renderNodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator())
												.OrderByDescending(x =>
												{
													var prop = x.Properties.Get(VixenModules.Property.Location.LocationDescriptor._typeId);
													if (prop != null)
													{
														return ((LocationData)prop.ModuleData).X;
													}
													return 1;
												})
												.ThenBy(x =>
												{
													var prop = x.Properties.Get(VixenModules.Property.Location.LocationDescriptor._typeId);
													if (prop != null)
													{
														return ((LocationData)prop.ModuleData).Y;
													}
													return 1;
												})
												.GroupBy(x =>
												{
													var prop = x.Properties.Get(VixenModules.Property.Location.LocationDescriptor._typeId);
													if (prop != null)
													{
														return ((LocationData)prop.ModuleData).X;
													}
													return 1;
												})

												.Distinct();
					break;
				case WipeDirection.Out:
				case WipeDirection.In:
					RenderBurst(tokenSource, _data.Direction);

					return;

					break;
				default:
					break;
			}

			RenderNonBurst(tokenSource, renderNodes);
		}

		private void RenderNonBurst(CancellationTokenSource tokenSource, IEnumerable<IGrouping<int, ElementNode>> renderNodes)
		{

			if (renderNodes != null && renderNodes.Count() > 0)
			{
				TimeSpan effectTime = TimeSpan.Zero;
				if (WipeByCount)
				{
					int count = 0;
					double pulseSegment = (TimeSpan.TotalMilliseconds / PassCount) * (PulsePercent / 100);
					TimeSpan intervalTime = TimeSpan.FromMilliseconds((TimeSpan.TotalMilliseconds - pulseSegment) / (renderNodes.Count() * PassCount));
					TimeSpan segmentPulse = TimeSpan.FromMilliseconds(pulseSegment);

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
									var pulse = new Pulse.Pulse();
									pulse.TargetNodes = new ElementNode[] { element };
									pulse.TimeSpan = segmentPulse;
									pulse.ColorGradient = _data.ColorGradient;
									pulse.LevelCurve = _data.Curve;
									result = pulse.Render();
									result.OffsetAllCommandsByTime(effectTime);
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
									var pulse = new Pulse.Pulse();
									pulse.TargetNodes = new ElementNode[] { element };
									pulse.TimeSpan = segmentPulse;
									pulse.ColorGradient = _data.ColorGradient;
									pulse.LevelCurve = _data.Curve;
									result = pulse.Render();

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
														   var prop = s.Properties.Get(VixenModules.Property.Location.LocationDescriptor._typeId);
														   if (prop != null)
														   {
															   return new Tuple<ElementNode, int, int, int>(s, ((LocationData)prop.ModuleData).X, ((LocationData)prop.ModuleData).Y, ((LocationData)prop.ModuleData).Z);

														   }
														   return new Tuple<ElementNode, int, int, int>(null, -1, -1, -1);
														   //return null
													   })
													   .Where(s => s.Item2 > 0) // Ignore the pseudo null values
													   .ToList();

			var maxX = burstNodes.Max(m => m.Item2);
			var maxY = burstNodes.Max(m => m.Item3);

			var minX = burstNodes.Min(m => m.Item2);
			var minY = burstNodes.Min(m => m.Item3);

			var Steps = (int)(Math.Max(maxX - minX, maxY - minY) / 2);

			bool startX = maxX - minX > maxY - minY; //Are we starting the stepping on X or Y axis (based on shape)


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
					var prop = s.Properties.Get(VixenModules.Property.Location.LocationDescriptor._typeId);
					if (prop != null)
					{
						return ((LocationData)prop.ModuleData).X < minX + i || ((LocationData)prop.ModuleData).X > maxX - i;
					}
					return false;
				});
				xNodes.RemoveAll(s =>
				{
					var prop = s.Properties.Get(VixenModules.Property.Location.LocationDescriptor._typeId);
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


			if (renderNodes != null && renderNodes.Count() > 0)
			{
				TimeSpan effectTime = TimeSpan.Zero;
				if (WipeByCount)
				{
					int count = 0;
					double pulseSegment = (TimeSpan.TotalMilliseconds / PassCount) * (PulsePercent / 100);
					TimeSpan intervalTime = TimeSpan.FromMilliseconds((TimeSpan.TotalMilliseconds - pulseSegment) / (renderNodes.Count() * PassCount));
					TimeSpan segmentPulse = TimeSpan.FromMilliseconds(pulseSegment);

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
									var pulse = new Pulse.Pulse();
									pulse.TargetNodes = new ElementNode[] { element };
									pulse.TimeSpan = segmentPulse;
									pulse.ColorGradient = _data.ColorGradient;
									pulse.LevelCurve = _data.Curve;
									result = pulse.Render();
									result.OffsetAllCommandsByTime(effectTime);
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
									var pulse = new Pulse.Pulse();
									pulse.TargetNodes = new ElementNode[] { element };
									pulse.TimeSpan = segmentPulse;
									pulse.ColorGradient = _data.ColorGradient;
									pulse.LevelCurve = _data.Curve;
									result = pulse.Render();

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
				UpdateAttributes();
			}
		}



		private void CheckForInvalidColorData()
		{
			HashSet<Color> validColors = new HashSet<Color>();
			validColors.AddRange(TargetNodes.SelectMany(x => ColorModule.getValidColorsForElementNode(x, true)));
			if (validColors.Any() && !_data.ColorGradient.GetColorsInGradient().IsSubsetOf(validColors))
			{
				//Our color is not valid for any elements we have.
				//Try to set a default color gradient from our available colors
				_data.ColorGradient = new ColorGradient(validColors.First());
			}
		}

		[Value]
		[Category(@"Effect Color")]
		[DisplayName(@"Color")]
		[Description(@"Sets the Wipe color.")]
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
			}
		}

		[Value]
		[Category(@"Effect Direction")]
		[DisplayName(@"Direction")]
		[Description(@"Controls how the direction of the wipe.")]
		public WipeDirection Direction
		{
			get { return _data.Direction; }
			set
			{
				_data.Direction = value;
				IsDirty = true;
			}
		}

		[Value]
		[Category(@"Effect Brightness")]
		[DisplayName(@"Pulse Brightness")]
		[Description(@"Controls the individual pulse shape.")]
		public Curve Curve
		{
			get { return _data.Curve; }
			set
			{
				_data.Curve = value;
				IsDirty = true;
			}
		}

		[Value]
		[Category(@"Effect Pulse")]
		[DisplayName(@"Pulse Time")]
		[Description(@"Controls the individual pulse length in milliseconds.")]
		public int PulseTime
		{
			get { return _data.PulseTime; }
			set
			{
				_data.PulseTime = value;
				IsDirty = true;
			}
		}

		[Value]
		[Category(@"Effect Type")]
		[DisplayName(@"Type")]
		[Description(@"Controls how the wipe behaves. Either by a count of passes, or by time related to pulse length.")]
		[TypeConverter(typeof(WipeSelectionTypeConverter))]
		public bool WipeByCount
		{
			get { return _data.WipeByCount; }
			set
			{
				_data.WipeByCount = value;
				IsDirty = true;
				UpdateAttributes();
				TypeDescriptor.Refresh(this);
			}
		}

		[Value]
		[Category(@"Effect Pulse")]
		[DisplayName(@"Wipe Count")]
		[Description(@"Controls the number of passes the wipe makes.")]
		public int PassCount
		{
			get { return _data.PassCount; }
			set
			{
				_data.PassCount = value;
				IsDirty = true;
			}
		}

		[Value]
		[Category(@"Effect Pulse")]
		[DisplayName(@"Pulse Percent")]
		[Description(@"Controls the length of the pulse as a percentage of the effect time.")]
		[TypeConverter(typeof(PercentTypeConverter))]
		public double PulsePercent
		{
			get { return _data.PulsePercent; }
			set
			{
				_data.PulsePercent = value;
				IsDirty = true;
			}
		}

		#region Attributes

		private void UpdateAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(4)
			{
				{"PassCount", WipeByCount},
				{"PulsePercent", WipeByCount},
				{"PulseTime", !WipeByCount}
			};
			SetBrowsable(propertyStates);
		}

		#endregion

	}
}