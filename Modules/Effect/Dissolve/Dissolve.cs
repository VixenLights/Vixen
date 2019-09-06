using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Vixen.Attributes;
using Vixen.Marks;
using Vixen.Module;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using Vixen.TypeConverters;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Pulse;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Dissolve
{
	public class Dissolve : BaseEffect
	{
		private EffectIntents _elementData;
		private DissolveData _data;
		private IEnumerable<IMark> _marks = null;
		private int _pixels;
		private int _totalNodes;
		private List<TempClass> _tempNodes;
		private List<TempClass> _nodes;
		private List<DissolveClass> _elements;
		private List<DissolveClass> _renderElements;
		private readonly int _timeInterval = 25;

		public Dissolve()
		{
			_data = new DissolveData();
			InitAllAttributes();
		}

		protected override void TargetNodesChanged()
		{
			if (TargetNodes.Any())
			{
				CheckForInvalidColorData();
			}
		}

		protected override void _PreRender(CancellationTokenSource cancellationToken = null)
		{
			_elementData = new EffectIntents();
			IEnumerable<IGrouping<int, IElementNode>> elements;
			IEnumerable<IElementNode> renderNodes = GetNodesToRenderOn();
			switch (DissolveMode)
			{
				case DissolveMode.MarkCollection:
				{
					SetupMarks();
					if ((DissolveMarkType == DissolveMarkType.PerMarkFill ||
					    DissolveMarkType == DissolveMarkType.PerMarkDissolve) && ColorPerStep)
					{
						int nodeCount = renderNodes.Count();
						int nodesPerGroup = DirectionsTogether && BothDirections
							? nodeCount / (_marks.Count() - 1) / 2
							: nodeCount / (_marks.Count() - 1);
						elements = renderNodes.Select((x, index) => new { x, index })
							.GroupBy(x => x.index / nodesPerGroup, y => y.x);
					}
					else
					{
						elements = renderNodes.Select((x, index) => new { x, index })
							.GroupBy(x => x.index / GroupLevel, y => y.x);
					}

					break;
				}
				default:
					elements = renderNodes.Select((x, index) => new { x, index })
						.GroupBy(x => x.index / GroupLevel, y => y.x);
					break;
			}

			
			var renderNodes1 = elements as IGrouping<int, IElementNode>[] ?? elements.ToArray();
			_totalNodes = renderNodes1.Count();

			_pixels = 0;

			_tempNodes = new List<TempClass>(_totalNodes);
			_nodes = new List<TempClass>(_totalNodes);
			_elements = new List<DissolveClass>();
			_renderElements = new List<DissolveClass>();

			InitializeDissolveClasses(1);

			_elementData.Add(RenderNode(renderNodes1));

			_nodes = null;
			_tempNodes = null;
			_elements = null;
			_renderElements = null;
		}

		private IEnumerable<IElementNode> GetNodesToRenderOn()
		{
			IEnumerable<IElementNode> renderNodes = TargetNodes;

			if (!EnableDepth)
			{
				renderNodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator());
			}
			else
			{
				for (int i = 0; i < DepthOfEffect; i++)
				{
					renderNodes = renderNodes.SelectMany(x => x.Children);
				}
			}

			// If the given DepthOfEffect results in no nodes (because it goes "too deep" and misses all nodes), 
			// then we'll default to the LeafElements, which will at least return 1 element (the TargetNode)
			if (!renderNodes.Any())
				renderNodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator());

			return renderNodes;

		}

		//Validate that the we are using valid colors and set appropriate defaults if not.
		//we only need to check against 1 color variable,
		//it should be checked at a later time than what this is doing currently
		private void CheckForInvalidColorData()
		{
			// check for sane default colors when first rendering it
			var validColors = GetValidColors();
			if (validColors.Any())
			{
				bool changed = false;
				for (var index = 0; index < Colors.Count; index++)
				{
					if (!Colors[index].ColorGradient.GetColorsInGradient().IsSubsetOf(validColors))
					{
						Colors[index].ColorGradient = new ColorGradient(validColors.First());
						changed = true;
					}
				}

				if (changed)
				{
					OnPropertyChanged("Colors");
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
				_data = value as DissolveData;
				CheckForInvalidColorData();
				InitAllAttributes();
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		#region Color

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"ColorGradients")]
		[ProviderDescription(@"Gradient")]
		[PropertyOrder(0)]
		public List<GradientLevelPair> Colors
		{
			get { return _data.Colors; }
			set
			{
				_data.Colors = value;
				IsDirty = true;
				CheckForInvalidColorData();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"ColorPerStep")]
		[ProviderDescription(@"ColorPerStep")]
		[PropertyOrder(1)]
		public bool ColorPerStep
		{
			get { return _data.ColorPerStep; }
			set
			{
				_data.ColorPerStep = value;
				IsDirty = true;
				UpdateDissolveModeAttributes();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"RandomColorOrder")]
		[ProviderDescription(@"RandomColorOrder")]
		[PropertyOrder(2)]
		public bool RandomColor
		{
			get { return _data.RandomColor; }
			set
			{
				_data.RandomColor = value;
				IsDirty = true;
				UpdateDissolveModeAttributes();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"GroupColors")]
		[ProviderDescription(@"GroupColors")]
		[PropertyOrder(3)]
		public bool GroupColors
		{
			get { return _data.GroupColors; }
			set
			{
				_data.GroupColors = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Config

		[Value]
		[ProviderCategory("Config", 1)]
		[ProviderDisplayName(@"TimingSource")]
		[ProviderDescription(@"TimingSource")]
		[PropertyOrder(0)]
		public DissolveMode DissolveMode
		{
			get
			{
				return _data.DissolveMode;
			}
			set
			{
				if (_data.DissolveMode != value)
				{
					_data.DissolveMode = value;
					UpdateDissolveModeAttributes();
					IsDirty = true;
					OnPropertyChanged();
				}
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"MarkCollection")]
		[ProviderDescription(@"DissolveMarkCollection")]
		[TypeConverter(typeof(IMarkCollectionNameConverter))]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(1)]
		public string MarkCollectionId
		{
			get
			{
				return MarkCollections.FirstOrDefault(x => x.Id == _data.MarkCollectionId)?.Name;
			}
			set
			{
				var newMarkCollection = MarkCollections.FirstOrDefault(x => x.Name.Equals(value));
				var id = newMarkCollection?.Id ?? Guid.Empty;
				if (!id.Equals(_data.MarkCollectionId))
				{
					var oldMarkCollection = MarkCollections.FirstOrDefault(x => x.Id.Equals(_data.MarkCollectionId));
					RemoveMarkCollectionListeners(oldMarkCollection);
					_data.MarkCollectionId = id;
					AddMarkCollectionListeners(newMarkCollection);
					IsDirty = true;
					OnPropertyChanged();
				}
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Density")]
		[ProviderDescription(@"Density")]
		[PropertyOrder(2)]
		public Curve DissolveCurve
		{
			get { return _data.DissolveCurve; }
			set
			{
				_data.DissolveCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"MarkType")]
		[ProviderDescription(@"MarkType")]
		[PropertyOrder(3)]
		public DissolveMarkType DissolveMarkType
		{
			get { return _data.DissolveMarkType; }
			set
			{
				_data.DissolveMarkType = value;
				IsDirty = true;
				UpdateDissolveModeAttributes();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"GroupLevel")]
		[ProviderDescription(@"GroupLevel")]
		[NumberRange(1, 50000, 1)]
		[PropertyOrder(4)]
		public int GroupLevel
		{
			get { return _data.GroupLevel; }
			set
			{
				_data.GroupLevel = value > 0 ? value : 1;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"StartingElement")]
		[ProviderDescription(@"StartingElement")]
		[NumberRange(1, 50000, 1)]
		[PropertyOrder(5)]
		public int StartingNode
		{
			get { return _data.StartingNode; }
			set
			{
				_data.StartingNode = value > 0 ? value : 1;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"RandomDissolve")]
		[ProviderDescription(@"RandomDissolve")]
		[PropertyOrder(6)]
		public bool RandomDissolve
		{
			get { return _data.RandomDissolve; }
			set
			{
				_data.RandomDissolve = value;
				IsDirty = true;
				UpdateDissolveModeAttributes();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"DissolveFlip")]
		[ProviderDescription(@"DissolveFlip")]
		[PropertyOrder(7)]
		public bool DissolveFlip
		{
			get { return _data.DissolveFlip; }
			set
			{
				_data.DissolveFlip = value;
				IsDirty = true;
				UpdateDissolveModeAttributes();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"BothDirections")]
		[ProviderDescription(@"BothDirections")]
		[PropertyOrder(8)]
		public bool BothDirections
		{
			get { return _data.BothDirections; }
			set
			{
				_data.BothDirections = value;
				IsDirty = true;
				UpdateDissolveModeAttributes();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"DirectionsTogether")]
		[ProviderDescription(@"DirectionsTogether")]
		[PropertyOrder(9)]
		public bool DirectionsTogether
		{
			get { return _data.DirectionsTogether; }
			set
			{
				_data.DirectionsTogether = value;
				IsDirty = true;
				UpdateDissolveModeAttributes();
				OnPropertyChanged();
			}
		}

		#endregion

		#region Depth

		[Value]
		[ProviderCategory(@"Depth", 2)]
		[ProviderDisplayName(@"IndividualElements")]
		[ProviderDescription(@"AlternatingDepth")]
		[PropertyOrder(0)]
		public bool EnableDepth
		{
			get { return (bool)_data.EnableDepth; }
			set
			{
				_data.EnableDepth = value;
				IsDirty = true;
				UpdateDepthAttributes();
				TypeDescriptor.Refresh(this);
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Depth", 2)]
		[ProviderDisplayName(@"Depth")]
		[ProviderDescription(@"Depth")]
		[TypeConverter(typeof(TargetElementDepthConverter))]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(1)]
		public int DepthOfEffect
		{
			get { return _data.DepthOfEffect; }
			set
			{
				_data.DepthOfEffect = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/Dissolve/"; }
		}

		#endregion

		#region Attributes

		private void InitAllAttributes()
		{
			UpdateDissolveModeAttributes(false);
			UpdateDepthAttributes();
			TypeDescriptor.Refresh(this);
		}

		private void UpdateDissolveModeAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(10)
			{
				{"MarkCollectionId", DissolveMode == DissolveMode.MarkCollection},
				{"DissolveCurve", DissolveMarkType == DissolveMarkType.PerMark || DissolveMode == DissolveMode.TimeInterval},
				{"DissolveMarkType", DissolveMode == DissolveMode.MarkCollection},
				{"DissolveFlip", !RandomDissolve},
				{"StartingNode", !RandomDissolve},
				{"BothDirections", !RandomDissolve},
				{"DirectionsTogether", !RandomDissolve && BothDirections},
				{"ColorPerMark", (!BothDirections && DissolveMode == DissolveMode.TimeInterval) || (DissolveMode == DissolveMode.MarkCollection && DissolveMarkType <= (DissolveMarkType)2 )},
				{"GroupLevel", DissolveMode != DissolveMode.MarkCollection || !ColorPerStep || (DissolveMarkType != DissolveMarkType.PerMarkFill && DissolveMarkType != DissolveMarkType.PerMarkDissolve)},
				{"EnableDepth", !ColorPerStep || (DissolveMarkType != DissolveMarkType.PerMarkFill && DissolveMarkType != DissolveMarkType.PerMarkDissolve)}
			};

			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateDepthAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"DepthOfEffect", EnableDepth}
			};
			SetBrowsable(propertyStates);
		}

		#endregion

		public override bool IsDirty
		{
			get
			{
				if (Colors.Any(x => !x.ColorGradient.CheckLibraryReference() || !x.Curve.CheckLibraryReference()))
				{
					base.IsDirty = true;
				}

				return base.IsDirty;
			}
			protected set { base.IsDirty = value; }
		}

		// renders the given node to the internal ElementData dictionary. If the given node is
		// not a element, will recursively descend until we render its elements.
		private EffectIntents RenderNode(IGrouping<int, IElementNode>[] elements)
		{
			EffectIntents effectIntents = new EffectIntents();
			TimeSpan intervalTime = TimeSpan;
			List<TimeSpan> markInterval = new List<TimeSpan>();
			List<string> markPercentage = new List<string>();
			int pixelCount = _nodes.Count;
			TimeSpan startTime = TimeSpan.Zero;
			int intervals;
			double interval = 1;
			int totalPixelCount = 0;

			if (DissolveMode == DissolveMode.MarkCollection)
			{
				if (_marks != null)
				{
					markInterval.AddRange(_marks.Select(mark => mark.StartTime - StartTime));
					markPercentage.AddRange(_marks.Select(mark => mark.Text));
				}
				markInterval.Add(TimeSpan);
				intervals = markInterval.Count;
				if (DissolveMarkType != DissolveMarkType.PerMark) intervals--;
			}
			else
			{
				intervals = Convert.ToInt32(Math.Ceiling(TimeSpan.TotalMilliseconds / _timeInterval));
				if (intervals >= 1) intervalTime = TimeSpan.FromMilliseconds(_timeInterval);
			}

			var endTime = TimeSpan;
			for (var i = 0; i < intervals; i++)
			{
				if (DissolveMode == DissolveMode.MarkCollection && DissolveMarkType == DissolveMarkType.PerMark)
				{
					interval = (markInterval[i] - startTime).TotalMilliseconds / _timeInterval;
					endTime = markInterval[i];
				}

				for (int j = 0; j < interval; j++)
				{
					double position = DissolveMode == DissolveMode.TimeInterval || DissolveMarkType != DissolveMarkType.PerMark
						? (double)100 / intervals * i
						: (double)100 / interval * j;

					// Gets number of Pixels that need to be created/removed.
					if (DissolveMode == DissolveMode.MarkCollection && DissolveMarkType != DissolveMarkType.PerMark)
					{
						switch (DissolveMarkType)
						{
							case DissolveMarkType.MarkLabelValue:
								int.TryParse(markPercentage[i], out int percentage);
								_pixels = (int)Math.Ceiling((double)pixelCount * percentage / 100) - totalPixelCount;
								break;
							case DissolveMarkType.MarkLabelPixels:
								int.TryParse(markPercentage[i], out int pixels);
								_pixels = (int)Math.Ceiling((double)pixels) - totalPixelCount;
								break;
							case DissolveMarkType.PerMarkFill:
								_pixels = !ColorPerStep
									? (int) Math.Ceiling((double) pixelCount / (intervals - 1) * (i + 1)) -
									  totalPixelCount
									: 1;
								break;
							case DissolveMarkType.PerMarkDissolve:
								if (ColorPerStep)
								{
									_pixels = i == 0 ? !ColorPerStep ? pixelCount :
										_marks.Count() - 1 :
										!ColorPerStep ? -Colors.Count : -1;
								}
								else
								{
									_pixels = (int)Math.Ceiling((double)pixelCount / (intervals - 1) * (intervals - i - 1)) - totalPixelCount;
								}
								break;
						}
					}
					else
					{
						_pixels = (int)Math.Ceiling(pixelCount * DissolveCurve.GetValue(position) / 100) - totalPixelCount;
					}

					int together = 1;
					int bothTogether = 1;
					if (ColorPerStep)
					{
						switch (DissolveMode)
						{
							case DissolveMode.MarkCollection:
								switch (DissolveMarkType)
								{
									case DissolveMarkType.PerMark when BothDirections && DirectionsTogether:
										together = 2;
										bothTogether = 2;
										break;
									case DissolveMarkType.PerMarkFill:
									case DissolveMarkType.PerMarkDissolve:
										together = 1;
										bothTogether = BothDirections && !RandomDissolve && DirectionsTogether ? 2 : together;
										break;
								}
								_pixels = _pixels / together;
								break;
							default:
								if (!BothDirections)
								{
									together = 1;
									bothTogether = together;
									_pixels = _pixels / together;
								}
								else
								{
									together = !DirectionsTogether && BothDirections ? 1 : 2;
									bothTogether = together;
									_pixels = _pixels / together;
								}
								break;
						}
					}
					else
					{
						together = !DirectionsTogether && BothDirections ? 1 : 2;
						bothTogether = together;
						_pixels = _pixels / together;
					}

					if (_pixels >= 0)
					{
						// Adds Displayed Pixels.
						for (int pixel = 0; pixel < _pixels; pixel++)
						{
							for (int k = 0; k < bothTogether; k++)
							{
								if (_nodes.Count <= 0) break;
								totalPixelCount++;

								// Add element.
								DissolveClass m = new DissolveClass
								{
									StartTime = startTime,
									EndTime = endTime,
									Duration = endTime - startTime,
									ColorIndex = _nodes[0].ColorIndex,
									ElementIndex = _nodes[0].ElementIndex
								};
								_elements.Add(m);

								if (RandomDissolve)
								{
									_tempNodes.Add(_nodes[0]);
								}
								else
								{
									_tempNodes.Insert(0, _nodes[0]);
								}

								_nodes.RemoveAt(0);
							}
						}
					}
					else
					{
						// Removes Displayed Pixels.
						for (int pixel = 0; pixel < -_pixels; pixel++)
						{
							for (int k = 0; k < bothTogether; k++)
							{
								if (_tempNodes.Count <= 0) break;
								totalPixelCount--;

								for (int ii = 0; ii < _elements.Count; ii++)
								{
									if (_elements[ii].ElementIndex == _tempNodes[0].ElementIndex &&
									    _elements[ii].StartTime + _elements[ii].Duration > startTime)
									{
										// Transfer the Element data to the RenderElement as we are finished with it.
										// Element is then removed to save cycles.
										// This saved a considerable amount of time as it will only loop through
										// elements that have not be finalized.
										DissolveClass m = new DissolveClass
										{
											Duration = startTime - _elements[ii].StartTime -
											           TimeSpan.FromMilliseconds(1),
											StartTime = _elements[ii].StartTime,
											EndTime = _elements[ii].EndTime,
											ColorIndex = _elements[ii].ColorIndex,
											ElementIndex = _elements[ii].ElementIndex
										};
										_renderElements.Add(m);
										_elements.RemoveAt(ii);
										break;
									}
								}

								if (RandomDissolve)
								{
									_nodes.Add(_tempNodes[0]);
								}
								else
								{
									_nodes.Insert(0, _tempNodes[0]);
								}

								_tempNodes.RemoveAt(0);
							}
						}
					}

					if (DissolveMode == DissolveMode.MarkCollection && DissolveMarkType == DissolveMarkType.PerMark) startTime = startTime.Add(TimeSpan.FromMilliseconds(_timeInterval));
				}

				startTime = DissolveMode == DissolveMode.TimeInterval ? startTime + intervalTime :
					DissolveMarkType != DissolveMarkType.PerMark ? markInterval[i + 1] : markInterval[i];

				if (DissolveMode == DissolveMode.MarkCollection && DissolveMarkType == DissolveMarkType.PerMark)
				{
					InitializeDissolveClasses(i);
					totalPixelCount = 0;
					// Copies Elements to be rendered. This will be done per Mark.
					CopyElements();
				}
			}

			// Copies any remaining Elements to be rendered.
			CopyElements();

			// Now render element
			foreach (DissolveClass dissolveNode in _renderElements)
			{
				IGrouping<int, IElementNode> elementGroup = elements[dissolveNode.ElementIndex];
				foreach (var element in elementGroup)
				{
					if (GroupColors)
					{
						foreach (var gradientLevelPair in Colors)
						{
							RenderElement(gradientLevelPair, dissolveNode.StartTime, dissolveNode.Duration,
								element, effectIntents);
						}
					}
					else
					{
						RenderElement(Colors[dissolveNode.ColorIndex], dissolveNode.StartTime, dissolveNode.Duration,
							element, effectIntents);
					}
				}
			}

			return effectIntents;
		}

		private void RenderElement(GradientLevelPair gradient, TimeSpan startTime, TimeSpan interval,
			IElementNode element, EffectIntents effectIntents)
		{
			if (interval <= TimeSpan.Zero) return;
			var result = PulseRenderer.RenderNode(element, gradient.Curve, gradient.ColorGradient, interval, HasDiscreteColors);
			result.OffsetAllCommandsByTime(startTime);
			effectIntents.Add(result);
		}

		private void CopyElements()
		{
			_renderElements.AddRange(_elements);
			_elements.Clear();
		}

		private void InitializeDissolveClasses(int intervals)
		{
			_nodes.Clear();
			_tempNodes.Clear();
			int colorCount = Colors.Count;
			int startingNode = StartingNode - 1;
			int backwardsNode = startingNode - 1;
			List<int> elementIndex = new List<int>();
			
			for (int node = 0; node < _totalNodes; node++)
			{
				elementIndex.Add(node);
			}

			int colorIndex = 0;
			for (int x = 0; x < _totalNodes; x++)
			{
				int currentNode;
				if (RandomDissolve)
				{
					int currentIndex = Rand(0, elementIndex.Count);
					currentNode = elementIndex[currentIndex];
					elementIndex.RemoveAt(currentIndex);
				}
				else
				{
					if (startingNode < 0) startingNode = 0;
					if (backwardsNode < 0) backwardsNode = _totalNodes - 1;
					if (BothDirections)
					{
						// Dissolves or Fills in both directions.
						if (x % 2 == 0)
						{
							currentNode = backwardsNode % _totalNodes;
							backwardsNode--;
						}
						else
						{
							currentNode = startingNode % _totalNodes;
							startingNode++;
						}
					}
					else
					{
						currentNode = startingNode % _totalNodes;
						startingNode++;
					}
				}

				// Randomly or sequentially add color index to class and Element Index.
				switch (DissolveMarkType)
				{
					case DissolveMarkType.PerMark when ColorPerStep && DissolveMode == DissolveMode.MarkCollection:
						if (RandomColor)
						{
							if (x == 0) colorIndex = Rand(0, colorCount);
						}
						else
						{
							colorIndex = intervals % colorCount;
						}
						break;
					default:
						colorIndex = RandomColor ? Rand(0, colorCount) : currentNode % colorCount;
						break;
				}
				
				TempClass tc = new TempClass {ElementIndex = currentNode, ColorIndex = colorIndex};
				_nodes.Add(tc);
			}

			if (!DissolveFlip) _nodes.Reverse();
		}

		private class DissolveClass
		{
			public int ElementIndex;
			public int ColorIndex;
			public TimeSpan StartTime;
			public TimeSpan Duration;
			public TimeSpan EndTime;
		}

		private class TempClass
		{
			public int ElementIndex;
			public int ColorIndex;
		}

		private void SetupMarks()
		{
			IMarkCollection mc = MarkCollections.FirstOrDefault(x => x.Id == _data.MarkCollectionId);
			_marks = mc?.MarksInclusiveOfTime(StartTime, StartTime + TimeSpan);
		}

		/// <inheritdoc />
		protected override void MarkCollectionsChanged()
		{
			if (DissolveMode == DissolveMode.MarkCollection)
			{
				var markCollection = MarkCollections.FirstOrDefault(x => x.Name.Equals(MarkCollectionId));
				InitializeMarkCollectionListeners(markCollection);
			}
		}

		/// <inheritdoc />
		protected override void MarkCollectionsRemoved(IList<IMarkCollection> addedCollections)
		{
			var mc = addedCollections.FirstOrDefault(x => x.Id == _data.MarkCollectionId);
			if (mc != null)
			{
				//Our collection is gone!!!!
				RemoveMarkCollectionListeners(mc);
				MarkCollectionId = String.Empty;
			}
		}
	}

}