using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Pulse;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
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
		private readonly int _timeInterval = 50;
		private int _maxX;
		private int _minX;
		private int _maxY;
		private int _minY;
		private int _midX;
		private int _midY;
		private int _bufferWidth;
		private int _bufferHeight;
		private int _pulsePercent;

		protected override void TargetNodesChanged()
		{
			CheckForInvalidColorData();
		}

		protected override void _PreRender(CancellationTokenSource tokenSource = null)
		{
			_elementData = new EffectIntents();

			List<ElementNode[]> renderNodes = new List<ElementNode[]>();
			List<Tuple<ElementNode, int, int, int>> renderedNodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator())
				.Select(s =>
				{
					var prop = s.Properties.Get(LocationDescriptor._typeId);
					if (prop != null)
					{
						return new Tuple<ElementNode, int, int, int>(s, ((LocationData)prop.ModuleData).X,
							((LocationData)prop.ModuleData).Y, ((LocationData)prop.ModuleData).Z);
					}
					return new Tuple<ElementNode, int, int, int>(null, -1, -1, -1);
				})
				.Where(s => s.Item2 > 0)
				.ToList();

			if (!renderedNodes.Any()) return;

			_maxX = renderedNodes.Max(m => m.Item2);
			_maxY = renderedNodes.Max(m => m.Item3);
			_minX = renderedNodes.Min(m => m.Item2);
			_minY = renderedNodes.Min(m => m.Item3);
			_bufferWidth = _maxX - _minX;
			_bufferHeight = _maxY - _minY;
			_midX = _bufferWidth / 2;
			_midY = _bufferHeight / 2;

			switch (Direction)
			{
				case WipeDirection.DiagonalUp:
				case WipeDirection.DiagonalDown:
					renderNodes = GetRenderedDiagonal(renderedNodes);
					break;
				case WipeDirection.Vertical:
				case WipeDirection.Horizontal:
					renderNodes = GetRenderedLRUD(renderedNodes);
					break;
				case WipeDirection.Circle:
					renderNodes = GetRenderedCircle(renderedNodes);
					break;
				case WipeDirection.Dimaond:
					renderNodes = GetRenderedDiamond(renderedNodes);
					break;
				case WipeDirection.Burst:
					renderNodes = GetRenderedRectangle(renderedNodes);
					break;
			}

			switch (WipeMovement)
			{
				case WipeMovement.Count:
					RenderCount(renderNodes, tokenSource);
					break;
				case WipeMovement.PulseLength:
					RenderPulseLength(renderNodes, tokenSource);
					break;
				case WipeMovement.Movement:
					RenderMovement(renderNodes, tokenSource);
					break;
			}
		}

		private List<ElementNode[]> GetRenderedCircle(List<Tuple<ElementNode, int, int, int>> renderedNodes)
		{
			List<Tuple<int, ElementNode[]>> groups = new List<Tuple<int, ElementNode[]>>();
			int steps = (int) (DistanceFromPoint(new Point(_maxX, _maxY), new Point(_minX, _minY)) / 2);
			Point centerPoint = new Point((_bufferWidth) / 2 + _minX, _bufferHeight / 2 + _minY);

			_pulsePercent = (int) (_bufferWidth * (PulsePercent / 100));
			if (WipeMovement == WipeMovement.Movement) steps += _pulsePercent;

			for (int i = 0; i < steps; i++)
			{
				List<ElementNode> elements = new List<ElementNode>();
				foreach (Tuple<ElementNode, int, int, int> node in renderedNodes)
				{
					int nodeLocation = (int)DistanceFromPoint(centerPoint, new Point(node.Item2, node.Item3));
					if (nodeLocation == i) elements.Add(node.Item1);
				}
				groups.Add(new Tuple<int, ElementNode[]>(i, elements.ToArray()));

			}
			return !ReverseDirection || WipeMovement == WipeMovement.Movement
				? groups.OrderBy(o => o.Item1).Select(s => s.Item2).ToList()
				: groups.OrderByDescending(o => o.Item1).Select(s => s.Item2).ToList();
		}

		private List<ElementNode[]> GetRenderedDiamond(List<Tuple<ElementNode, int, int, int>> renderedNodes)
		{
			List<Tuple<int, ElementNode[]>> groups = new List<Tuple<int, ElementNode[]>>();
			int steps = (int)(Math.Sqrt(Math.Pow(_bufferWidth, 2) + Math.Pow(_bufferHeight, 2)) / 1.5);
			_pulsePercent = (int)(_bufferWidth * (PulsePercent / 100));
			if (WipeMovement == WipeMovement.Movement) steps += _pulsePercent;
			
			for (int i = 0; i < steps; i++)
			{
				List<ElementNode> elements = new List<ElementNode>();
				foreach (Tuple<ElementNode, int, int, int> node in renderedNodes)
				{
					// Do the Down/Left or Up/Right directions
					int nodeLocation = (node.Item3 - _minY) - (node.Item2 - _minX) +
					                   (_bufferWidth - _bufferHeight) / 2;
					if (nodeLocation < 0) nodeLocation = -nodeLocation;
					if (nodeLocation == i &&
					    ((_maxY - _midY - node.Item3) <= i && (_maxX - _midX - node.Item2) <= i) &&
					    (node.Item3 - _minY - _midY) <= i && (node.Item2 - _minX - _midX) <= i)
					{
						elements.Add(node.Item1);
					}

					// Do the Down/Right or Up/Left directions
					nodeLocation = (node.Item2 - _minX + node.Item3 - _minY) -
					               (_bufferWidth + _bufferHeight) / 2;
					if (nodeLocation < 0) nodeLocation = -nodeLocation;
					if (nodeLocation == i &&
					    ((_maxY - _midY - node.Item3) <= i && (_maxX - _midX - node.Item2) <= i) &&
					    (node.Item3 - _minY - _midY) <= i && (node.Item2 - _minX - _midX) <= i)
					{
						elements.Add(node.Item1);
					}
				}
				groups.Add(new Tuple<int, ElementNode[]>(i, elements.ToArray()));
			}
			return !ReverseDirection || WipeMovement == WipeMovement.Movement
				? groups.OrderBy(o => o.Item1).Select(s => s.Item2).ToList()
				: groups.OrderByDescending(o => o.Item1).Select(s => s.Item2).ToList();

		}

		private List<ElementNode[]> GetRenderedRectangle(List<Tuple<ElementNode, int, int, int>> renderedNodes)
		{
			List<Tuple<int, ElementNode[]>> groups = new List<Tuple<int, ElementNode[]>>();
			int steps = (int)(Math.Max(_bufferWidth, _bufferHeight) / 2);
			_pulsePercent = (int)(_bufferWidth * (PulsePercent / 100));
			if (WipeMovement == WipeMovement.Movement) steps += _pulsePercent;

			for (int i = 0; i < steps; i++)
			{
				List<ElementNode> elements = new List<ElementNode>();

				foreach (Tuple<ElementNode, int, int, int> node in renderedNodes)
				{
					// Sets Left and Right side of burst
					if (_maxY - _midY - node.Item3 <= i && _maxY - _midY - node.Item3 >= -i &&
					    (_maxX - _midX - node.Item2 == i || _maxX - _midX - node.Item2 == -i))
						elements.Add(node.Item1);

					// Sets Top and Bottom side of burst
					if (_maxX - _midX - node.Item2 <= i && _maxX - _midX - node.Item2 >= -i &&
					    (_maxY - _midY - node.Item3 == i || _maxY - _midY - node.Item3 == -i))
						elements.Add(node.Item1);
				}
				groups.Add(new Tuple<int, ElementNode[]>(i, elements.ToArray()));
			}

			return !ReverseDirection || WipeMovement == WipeMovement.Movement
				? groups.OrderBy(o => o.Item1).Select(s => s.Item2).ToList()
				: groups.OrderByDescending(o => o.Item1).Select(s => s.Item2).ToList();
		}

		private List<ElementNode[]> GetRenderedDiagonal(List<Tuple<ElementNode, int, int, int>> renderedNodes)
		{
			List<Tuple<int, ElementNode[]>> groups = new List<Tuple<int, ElementNode[]>>();
			int steps = (int)(Math.Sqrt(Math.Pow(_bufferWidth, 2) + Math.Pow(_bufferHeight, 2))*1.41);
			_pulsePercent = (int)(_bufferWidth * (PulsePercent / 100));
			if (WipeMovement == WipeMovement.Movement) steps += _pulsePercent;

			for (int i = 0; i < steps; i++)
			{
				List<ElementNode> elements = new List<ElementNode>();
				foreach (Tuple<ElementNode, int, int, int> node in renderedNodes)
				{
					if (ReverseDirection || WipeMovement == WipeMovement.Movement)
					{
						if (Direction == WipeDirection.DiagonalUp)
						{
							if (node.Item2 - _minX + node.Item3 - _minY == steps - i) elements.Add(node.Item1);
						}
						else
						{
							if ((node.Item3 - _minY) - (node.Item2 - _minX) + _bufferWidth == i) elements.Add(node.Item1);
						}
					}
					else
					{
						if (Direction == WipeDirection.DiagonalUp)
						{
							if ((node.Item2 - _minX) - (node.Item3 - _minY) + _bufferHeight ==
							    i)
								elements.Add(node.Item1);
						}
						else
						{
							if ((node.Item2 - _minX) + (node.Item3 - _minY) == i)
								elements.Add(node.Item1);
						}
					}
				}
				groups.Add(new Tuple<int, ElementNode[]>(i, elements.ToArray()));
			}
			return groups.OrderBy(o => o.Item1).Select(s => s.Item2).ToList();
		}

		private List<ElementNode[]> GetRenderedLRUD(List<Tuple<ElementNode, int, int, int>> renderedNodes)
		{
			List<Tuple<int, ElementNode[]>> groups = new List<Tuple<int, ElementNode[]>>();
			int steps = 0;

			_pulsePercent = Direction == WipeDirection.Vertical
				? (int)(_bufferHeight * (PulsePercent / 100))
				: (int)(_bufferWidth * (PulsePercent / 100));

			switch (Direction)
			{
				case WipeDirection.Vertical:
					steps = _bufferHeight;
					break;
				case WipeDirection.Horizontal:
					steps = _bufferWidth;
					break;
			}
			
			if (WipeMovement == WipeMovement.Movement) steps += _pulsePercent;
			
			for (int i = 0; i < steps; i++)
			{
				List<ElementNode> elements = new List<ElementNode>();
				switch (Direction)
				{
					case WipeDirection.Vertical:
						foreach (Tuple<ElementNode, int, int, int> node in renderedNodes)
						{
							if (_bufferHeight - (node.Item3 - _minY) == i) elements.Add(node.Item1);
						}

						break;

					case WipeDirection.Horizontal:
						foreach (Tuple<ElementNode, int, int, int> node in renderedNodes)
						{
							if (_bufferWidth - (node.Item2 - _minX) == i) elements.Add(node.Item1);
						}

						break;
				}
				groups.Add(new Tuple<int, ElementNode[]>(i, elements.ToArray()));
			}

			return ReverseDirection || WipeMovement == WipeMovement.Movement
				? groups.OrderBy(o => o.Item1).Select(s => s.Item2).ToList()
				: groups.OrderByDescending(o => o.Item1).Select(s => s.Item2).ToList();
		}

		private void RenderPulseLength(List<ElementNode[]> renderNodes, CancellationTokenSource tokenSource)
		{
			TimeSpan effectTime = TimeSpan.Zero;
			double intervals = (double)PulseTime / renderNodes.Count();
			var intervalTime = TimeSpan.FromMilliseconds(intervals);
			// the calculation above blows up render time/memory as count goes up, try this.. 
			// also fails if intervals is less than half a ms and intervalTime then gets 0
			intervalTime = TimeSpan.FromMilliseconds(Math.Max(intervalTime.TotalMilliseconds, 5));
			TimeSpan segmentPulse = TimeSpan.FromMilliseconds(PulseTime);
			while (effectTime < TimeSpan)
			{
				foreach (var item in renderNodes)
				{
					if (tokenSource != null && tokenSource.IsCancellationRequested)
						return;
					foreach (ElementNode element in item)
					{
						if (tokenSource != null && tokenSource.IsCancellationRequested)
							return;
						if (element == null) continue;

						EffectIntents result;
						if (ColorHandling == ColorHandling.GradientThroughWholeEffect)
						{
							result = PulseRenderer.RenderNode(element, _data.Curve, _data.ColorGradient, segmentPulse,
								HasDiscreteColors);
							result.OffsetAllCommandsByTime(effectTime);

							_elementData.Add(result);
						}
						else
						{
							double positionWithinGroup = (double)(1.0 / (TimeSpan.Ticks - segmentPulse.Ticks)) * (effectTime.Ticks);
							if (ColorAcrossItemPerCount) positionWithinGroup = positionWithinGroup * PassCount % 1;
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

									result = PulseRenderer.RenderNode(element, newCurve,
										new ColorGradient(colorProportion.Item1), segmentPulse, HasDiscreteColors);
									result.OffsetAllCommandsByTime(effectTime);
									
								}
							}
							else
							{
								result = PulseRenderer.RenderNode(element, _data.Curve,
									new ColorGradient(_data.ColorGradient.GetColorAt(positionWithinGroup)),
									segmentPulse, HasDiscreteColors);
								result.OffsetAllCommandsByTime(effectTime);
								
								_elementData.Add(result);
							}
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
			double pulseSegment = (TimeSpan.Ticks * (PulsePercent / 100)) / PassCount;
			TimeSpan intervalTime = TimeSpan.FromTicks((long)((TimeSpan.Ticks - pulseSegment) / (renderNodes.Count() * PassCount)));
			TimeSpan segmentPulse = TimeSpan.FromTicks((long)pulseSegment);

			while (count < PassCount)
			{
				foreach (ElementNode[] item in renderNodes)
				{
					if (tokenSource != null && tokenSource.IsCancellationRequested) return;

					foreach (ElementNode element in item)
					{
						if (tokenSource != null && tokenSource.IsCancellationRequested)
							return;
						if (element == null) continue;

						EffectIntents result;
						if (ColorHandling == ColorHandling.GradientThroughWholeEffect)
						{
							result = PulseRenderer.RenderNode(element, _data.Curve, _data.ColorGradient, segmentPulse,
								HasDiscreteColors);
							result.OffsetAllCommandsByTime(effectTime);
							if (WipeOff && count == 0)
							{
								foreach (var effectIntent in result.FirstOrDefault().Value)
								{
									_elementData.Add(PulseRenderer.GenerateStartingStaticPulse(element, effectIntent,
										HasDiscreteColors));
								}
							}

							if (WipeOn && count == PassCount - 1)
							{
								foreach (var effectIntent in result.FirstOrDefault().Value)
								{
									_elementData.Add(PulseRenderer.GenerateExtendedStaticPulse(element, effectIntent,
										TimeSpan, HasDiscreteColors));
								}
							}

							_elementData.Add(result);
						}
						else
						{
							double positionWithinGroup = (double)(1.0 / (TimeSpan.Ticks - segmentPulse.Ticks)) * (effectTime.Ticks);
							if (ColorAcrossItemPerCount) positionWithinGroup = positionWithinGroup * PassCount % 1;
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

									result = PulseRenderer.RenderNode(element, newCurve,
										new ColorGradient(colorProportion.Item1), segmentPulse, HasDiscreteColors);
									result.OffsetAllCommandsByTime(effectTime);

									if (WipeOff && count == 0)
									{
										foreach (var effectIntent in result.FirstOrDefault().Value)
										{
											_elementData.Add(PulseRenderer.GenerateStartingStaticPulse(element,
												effectIntent, HasDiscreteColors,
												new ColorGradient(colorProportion.Item1)));
										}
									}

									if (result.Count > 0) _elementData.Add(result);

									if (WipeOn && count == PassCount - 1)
									{
										foreach (var effectIntent in result.FirstOrDefault().Value)
										{
											_elementData.Add(PulseRenderer.GenerateExtendedStaticPulse(element,
												effectIntent, TimeSpan, HasDiscreteColors,
												new ColorGradient(colorProportion.Item1)));
										}
									}
								}
							}
							else
							{
								result = PulseRenderer.RenderNode(element, _data.Curve,
									new ColorGradient(_data.ColorGradient.GetColorAt(positionWithinGroup)),
									segmentPulse, HasDiscreteColors);
								result.OffsetAllCommandsByTime(effectTime);

								if (WipeOff && count == 0)
								{
									foreach (var effectIntent in result.FirstOrDefault().Value)
									{
										_elementData.Add(PulseRenderer.GenerateStartingStaticPulse(element,
											effectIntent, HasDiscreteColors));
									}
								}

								if (WipeOn && count == PassCount - 1)
								{
									foreach (var effectIntent in result.FirstOrDefault().Value)
									{
										_elementData.Add(PulseRenderer.GenerateExtendedStaticPulse(element,
											effectIntent, TimeSpan, HasDiscreteColors));
									}
								}
								_elementData.Add(result);
							}
						}
					}
					effectTime += intervalTime;
				}
				count++;
			}
		}
		
		private void RenderMovement(List<ElementNode[]> renderNodes, CancellationTokenSource tokenSource)
		{
			double previousMovement = 2.0;
			TimeSpan startTime = TimeSpan.Zero;
			TimeSpan timeInterval = TimeSpan.FromMilliseconds(_timeInterval);
			int intervals = Convert.ToInt32(Math.Ceiling(TimeSpan.TotalMilliseconds / _timeInterval));
			int burst = Direction != WipeDirection.DiagonalUp ? 0 : _pulsePercent - 1;

			List<WipeClass> renderElements = new List<WipeClass>();
			for (int i = 0; i < intervals; i++)
			{
				double position = (double)100 / intervals * i;
				double movement = MovementCurve.GetValue(position) / 100;
				if (previousMovement != movement)
				{
					if (renderElements.Count > 0) renderElements.Last().Duration = startTime - renderElements.Last().StartTime;

					WipeClass wc = new WipeClass
					{
						ElementIndex = (int)((renderNodes.Count - 1) * movement),
						StartTime = startTime,
						Duration = TimeSpan - startTime
					};

					if (ReverseColorDirection)
					{
						wc.ReverseColorDirection = previousMovement < movement ? 0 : 1;
					}

					renderElements.Add(wc);
				}

				previousMovement = movement;
				startTime += timeInterval;
			}
			double pos = ((double)100 / _pulsePercent) / 100;
			// Now render element
			foreach (var wipeNode in renderElements)
			{
				for (int i = 0; i < _pulsePercent; i++)
				{
					double position = wipeNode.ReverseColorDirection - pos * i;
					if (position < 0) position = -position;
					Color color = _data.ColorGradient.GetColorAt(position);
					double curveValue = _data.Curve.GetValue(position * 100) / 100;

					if (wipeNode.ElementIndex - i > 0 && wipeNode.ElementIndex - i + burst < renderNodes.Count)
					{
						ElementNode[] elementGroup = renderNodes[wipeNode.ElementIndex - i + burst];
						if (tokenSource != null && tokenSource.IsCancellationRequested) return;

						foreach (var item in elementGroup)
						{
							if (tokenSource != null && tokenSource.IsCancellationRequested)
								return;
							if (item != null)
							{
								var result = PulseRenderer.RenderNode(item, curveValue, color, wipeNode.Duration);
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
			public int ReverseColorDirection;
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
				UpdateAttributes();
				TypeDescriptor.Refresh(this);
			}
		}

		[Value]
		[ProviderCategory(@"Color", 3)]
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
		[ProviderDisplayName(@"ColorPerCount")]
		[ProviderDescription(@"ColorPerCount")]
		[PropertyOrder(2)]
		public bool ColorAcrossItemPerCount
		{
			get { return _data.ColorAcrossItemPerCount; }
			set
			{
				_data.ColorAcrossItemPerCount = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 3)]
		[ProviderDisplayName(@"ReverseColorCurve")]
		[ProviderDescription(@"ReverseColorCurve")]
		[PropertyOrder(3)]
		public bool ReverseColorDirection
		{
			get { return _data.ReverseColorDirection; }
			set
			{
				_data.ReverseColorDirection = value;
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
		[ProviderDisplayName(@"ReverseDirection")]
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
		[ProviderDisplayName(@"Movement")]
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
		[ProviderDisplayName(@"Movement")]
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
				{"ColorHandling", WipeMovement != WipeMovement.Movement},
				{"WipeMovementDirection", WipeMovement == WipeMovement.Movement },
				{"MovementCurve", WipeMovement == WipeMovement.Movement },
				{"ReverseDirection", WipeMovement != WipeMovement.Movement },
				{"ColorAcrossItemPerCount", ColorHandling == ColorHandling.ColorAcrossItems && WipeMovement != WipeMovement.Movement},
				{"ReverseColorDirection", WipeMovement == WipeMovement.Movement}
			};
			SetBrowsable(propertyStates);
		}

		#endregion

	}
}