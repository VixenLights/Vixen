using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
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
using ZedGraph;

namespace VixenModules.Effect.Dissolve
{
    public class Dissolve : BaseEffect
    {
        private EffectIntents _elementData;
        private DissolveData _data;
        private IEnumerable<IMark> _marks = null;
        private int _pixels;
        private Random _random = new Random();
        private int _totalNodes;
	    private List<int> _tempNodes;
	    private List<int> _nodes;
	    private List<DissolveClass> _elements;
	    private List<DissolveClass> _renderElements;

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

            var renderNodes = GetNodesToRenderOn();
	        _totalNodes = renderNodes.GetElements().Length;
	        _pixels = 0;

			_tempNodes = new List<int>(_totalNodes);
	        _nodes = new List<int>(_totalNodes);
	        _elements = new List<DissolveClass>();
	        _renderElements = new List<DissolveClass>();

			InitializeDissolveClasses();
			
            _elementData.Add(RenderNode(renderNodes));

	        _nodes = null;
	        _tempNodes = null;
	        _elements = null;
	        _renderElements = null;
		}

        private IEnumerable<ElementNode> GetNodesToRenderOn()
        {
            IEnumerable<ElementNode> renderNodes = TargetNodes;
			
            renderNodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator());

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
	    [ProviderCategory(@"Color", 3)]
	    [ProviderDisplayName(@"Group Colors")]
	    [ProviderDescription(@"Each element will have all colors generated in parallel")]
	    [PropertyOrder(4)]
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
        [DisplayName(@"Timing Source")]
        [Description(@"Selects what source is used to determine change.")]
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
        [ProviderDisplayName(@"Mark Collection")]
        [ProviderDescription(@"Mark Collection that has the phonemes to align to.")]
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
		[ProviderDescription(@"Adjust the density of the elements.")]
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
	    [ProviderDisplayName(@"Dissolve Mark Type")]
	    [ProviderDescription(@"Can use the Marks to determine how the effect is dissolved.")]
	    [PropertyOrder(3)]
	    public DissolveMarkType DissolveMarkType
		{
		    get { return _data.DissolveMarkType; }
		    set
		    {
			    _data.DissolveMarkType = value;
			    IsDirty = true;
			    OnPropertyChanged();
		    }
		}

	    [Value]
	    [ProviderCategory(@"Config", 1)]
	    [ProviderDisplayName(@"Random Dissolve")]
	    [ProviderDescription(@"Can use the Marks to determine how the effect is dissolved.")]
	    [PropertyOrder(4)]
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
	    [ProviderDisplayName(@"Dissolve Flip")]
	    [ProviderDescription(@"Flips to direction of the sequential dissolve.")]
	    [PropertyOrder(5)]
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
            TypeDescriptor.Refresh(this);
        }

		private void UpdateDissolveModeAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(4); 
			propertyStates.Add("MarkCollectionId", DissolveMode == DissolveMode.MarkCollection);
			propertyStates.Add("DissolveCurve", DissolveMarkType == DissolveMarkType.PerMark || DissolveMode == DissolveMode.TimeInterval);
			propertyStates.Add("DissolveMarkType", DissolveMode == DissolveMode.MarkCollection);
			propertyStates.Add("DissolveFlip", !RandomDissolve);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
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
        private EffectIntents RenderNode(IEnumerable<ElementNode> nodes)
        {
            EffectIntents effectIntents = new EffectIntents();
            TimeSpan intervalTime = TimeSpan;
            List<TimeSpan> markInterval = new List<TimeSpan>();
	        List<string> markPercentage = new List<string>();
			List<ElementNode> elementGroups = nodes.ToList();
			int pixelCount = _nodes.Count;
	        TimeSpan startTime = TimeSpan.Zero;
	        int intervals;
	        double interval = 1;
	        var gradientLevelItem = 0;
	        int colorCount = Colors.Count;
	        int totalPixelCount = 0;

			if (DissolveMode == DissolveMode.MarkCollection)
            {
                SetupMarks();
	            if (_marks != null)
	            {
		            markInterval.AddRange(_marks.Select(mark => mark.StartTime - StartTime));
		            markPercentage.AddRange(_marks.Select(mark => mark.Text));
				}
                markInterval.Add(TimeSpan);
                intervals = markInterval.Count;
	            if (DissolveMarkType == DissolveMarkType.MarkLabelValue) intervals--;
            }
	        else
	        {
		        intervals = Convert.ToInt32(Math.Ceiling(TimeSpan.TotalMilliseconds / 50));
		        if (intervals >= 1) intervalTime = TimeSpan.FromMilliseconds(50);
	        }

	        var endTime = TimeSpan;
			for (var i = 0; i < intervals; i++)
            {
	            if (DissolveMode == DissolveMode.MarkCollection && DissolveMarkType == DissolveMarkType.PerMark)
	            {
		            interval = (markInterval[i] - startTime).TotalMilliseconds / 50;
		            endTime = markInterval[i];
	            }

				for (int j = 0; j < interval; j++)
                {
	                double position = DissolveMode == DissolveMode.TimeInterval || DissolveMarkType == DissolveMarkType.MarkLabelValue
						? (double) 100 / intervals * i
		                : (double) 100 / interval * j;
					
					// Gets number of Pixels that need to be created/removed.
	                if (DissolveMode == DissolveMode.MarkCollection && DissolveMarkType == DissolveMarkType.MarkLabelValue)
	                {
		                int.TryParse(markPercentage[i], out int percentage);
						_pixels = (int)(pixelCount * percentage / 100) - totalPixelCount;
					}
	                else
	                {
						_pixels = (int)(pixelCount * DissolveCurve.GetValue(position) / 100) - totalPixelCount;
					}
	                
	                if (_pixels >= 0)
	                {
						// Adds Displayed Pixels.
		                for (int pixel = 0; pixel < _pixels; pixel++)
		                {
			                if ( _nodes.Count <= 0) break;
			                totalPixelCount++;
							var random = _nodes.Count - 1;
			                if (RandomDissolve)
			                {
				                random = _random.Next(0, _nodes.Count);
							}

							// Add element.
							DissolveClass m = new DissolveClass();
							m.StartTime = startTime;
			                m.EndTime = endTime;
							m.Duration = endTime - startTime;
			                m.ColorIndex = _random.Next(0, colorCount);
							m.ElementIndex = _nodes[random];
							_elements.Add(m);

			                _tempNodes.Add(_nodes[random]);
							_nodes.RemoveAt(random);
						}
	                }
	                else
					{
						// Removes Displayed Pixels.
						for (int pixel = 0; pixel < -_pixels; pixel++)
		                {
			                if (_tempNodes.Count <= 0) break;
			                var random = _tempNodes.Count - 1;
			                if (RandomDissolve)
			                {
				                random = _random.Next(0, _tempNodes.Count);
			                }
			                totalPixelCount--;
							
							for (int ii = 0; ii < _elements.Count; ii++)
							{
								if (_elements[ii].ElementIndex == _tempNodes[random] && _elements[ii].StartTime + _elements[ii].Duration > startTime)
								{
									// Transfer the Element data to the RenderElement as we are finished with it.
									// Element is then removed to save cycles.
									// This saved a considerable amount of time as it will only loop through
									// elements that have not be finalized.
									DissolveClass m = new DissolveClass();
									m.Duration = startTime - _elements[ii].StartTime;
									m.StartTime = _elements[ii].StartTime;
									m.EndTime = _elements[ii].EndTime;
									m.ColorIndex = _elements[ii].ColorIndex;
									m.ElementIndex = _elements[ii].ElementIndex;
									_renderElements.Add(m);
									_elements.RemoveAt(ii);
									break;
								}
							}
							_nodes.Add(_tempNodes[random]);
							_tempNodes.RemoveAt(random);
						}
	                }
					
	                if (DissolveMode == DissolveMode.MarkCollection && DissolveMarkType == DissolveMarkType.PerMark) startTime = startTime.Add(TimeSpan.FromMilliseconds(50));
                }

	            startTime = DissolveMode == DissolveMode.TimeInterval ? startTime + intervalTime :
		            DissolveMarkType == DissolveMarkType.MarkLabelValue ? markInterval[i + 1] : markInterval[i];
				
	            if (DissolveMode == DissolveMode.MarkCollection && DissolveMarkType == DissolveMarkType.PerMark)
	            {
		            InitializeDissolveClasses();
		            totalPixelCount = 0;
		            CopyElements();
	            }
			}

	        CopyElements();

			// Now render element
			foreach (DissolveClass dissolveNode in _renderElements)
			{
				ElementNode element = elementGroups[dissolveNode.ElementIndex];
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
		        
		        gradientLevelItem = ++gradientLevelItem % colorCount;
			}

			return effectIntents;
        }

        private void RenderElement(GradientLevelPair gradient, TimeSpan startTime, TimeSpan interval,
            ElementNode element, EffectIntents effectIntents)
        {
            if (interval <= TimeSpan.Zero) return;
            var result = PulseRenderer.RenderNode(element, gradient.Curve, gradient.ColorGradient, interval, HasDiscreteColors);
            result.OffsetAllCommandsByTime(startTime);
            effectIntents.Add(result);
		}

	    private void CopyElements()
	    {
		    foreach (var element in _elements) _renderElements.Add(element);
		    _elements.Clear();
		}

	    private void InitializeDissolveClasses()
	    {
		    _nodes.Clear();
		    _tempNodes.Clear();

		    for (int x = 0; x < _totalNodes; x++)
		    {
				_nodes.Add(x);
		    }

			if (DissolveFlip) _nodes.Reverse();

	    }
	    public class DissolveClass
	    {
		    public int ElementIndex;
		    public int ColorIndex;
		    public TimeSpan StartTime;
		    public TimeSpan Duration;
			public TimeSpan EndTime;
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