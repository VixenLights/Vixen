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
        private static Random _random = new Random();
        private int _totalNodes;
	    private List<DissolveClass> _tempNodes;
	    private List<DissolveClass> _nodes;

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

			_tempNodes = new List<DissolveClass>(_totalNodes);
	        _nodes = new List<DissolveClass>(_totalNodes);

			InitializeDissolveClasses();
			
            _elementData.Add(RenderNode(renderNodes));

	        _nodes = null;
	        _tempNodes = null;
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
                foreach (GradientLevelPair t in Colors)
                {
                    if (!t.ColorGradient.GetColorsInGradient().IsSubsetOf(validColors))
                    {
                        t.ColorGradient = new ColorGradient(validColors.First());
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
        [ProviderDescription(@"GradientLevelPair")]
        public List<GradientLevelPair> Colors
        {
            get { return _data.Colors; }
            set
            {
                _data.Colors = value;
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
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1); 
			propertyStates.Add("MarkCollectionId", DissolveMode == DissolveMode.MarkCollection);
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
	        List<ElementNode> elementGroups = nodes.ToList();
			int pixelCount = _nodes.Count;
	        TimeSpan startTime = TimeSpan.Zero;
	        int intervals;
	        double interval = 1;

			if (DissolveMode == DissolveMode.MarkCollection)
            {
                SetupMarks();
                if (_marks != null) markInterval.AddRange(_marks.Select(mark => mark.StartTime - StartTime));
                markInterval.Add(TimeSpan);
                intervals = markInterval.Count;
            }
	        else
	        {
		        intervals = Convert.ToInt32(Math.Ceiling(TimeSpan.TotalMilliseconds / 50));
		        if (intervals >= 1) intervalTime = TimeSpan.FromMilliseconds(50);
	        }
			
			for (var i = 0; i < intervals; i++)
            {
	            if (DissolveMode == DissolveMode.MarkCollection) interval = (markInterval[i] - startTime).TotalMilliseconds / 50;

                for (int j = 0; j < interval; j++)
                {
	                double position = DissolveMode == DissolveMode.TimeInterval
		                ? (double) 100 / intervals * i
		                : (double) 100 / interval * j;
					
					// Gets number of Pixels that need to be created/removed.
	                _pixels = (int) (pixelCount * DissolveCurve.GetValue(position) / 100) - _tempNodes.Count;

	                if (_pixels >= 0)
	                {
						//Adds pixels to the Temp buffer and remove from initial buffer. Gives a Fill effect
		                for (int pixel = 0; pixel < _pixels; pixel++)
		                {
			                if ( _nodes.Count <= 0) break;
			                var random = _random.Next(0, _nodes.Count);
			                _tempNodes.Add(_nodes[random]);
			                _nodes.RemoveAt(random);
						}
	                }
	                else
					{
						//Adds pixels to the initial buffer and remove from Temp buffer. Gives a Dissolve effect
						for (int pixel = 0; pixel < -_pixels; pixel++)
		                {
			                if (_tempNodes.Count <= 0) break;
			                var random = _random.Next(0, _tempNodes.Count);
			                _nodes.Add(_tempNodes[random]);
			                _tempNodes.RemoveAt(random);
						}
	                }
					
					// Gets current Color and Level based on position.
	                List<GradientLevelPair> currentColorPosition = GetCurrentPositionGLP(position);

					// Now render element
					foreach (DissolveClass dissolveNode in _tempNodes)
	                {
		                ElementNode element = elementGroups[dissolveNode.X];
						RenderElement(currentColorPosition[dissolveNode.ColorIndex], startTime, TimeSpan.FromMilliseconds(50),
			                element, effectIntents);
	                }

	                if (DissolveMode == DissolveMode.MarkCollection) startTime = startTime.Add(TimeSpan.FromMilliseconds(50));
                }

                startTime = DissolveMode == DissolveMode.TimeInterval
                    ? startTime + intervalTime
                    : markInterval[i];

	            if (DissolveMode == DissolveMode.MarkCollection) InitializeDissolveClasses();
            }

            return effectIntents;
        }

	    private List<GradientLevelPair> GetCurrentPositionGLP(double position)
	    {
		    List<GradientLevelPair> currentColorPosition = new List<GradientLevelPair>();
			foreach (var glp in Colors)
		    {
			    Color color = glp.ColorGradient.GetColorAt(position / 100);
			    double intensity = glp.Curve.GetValue(position);
			    GradientLevelPair levelPair = new GradientLevelPair(color, new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { intensity, intensity })));
			    currentColorPosition.Add(levelPair);
		    }
		    return currentColorPosition;
	    }

        private void RenderElement(GradientLevelPair gradientLevelPair, TimeSpan startTime, TimeSpan interval,
            ElementNode element, EffectIntents effectIntents)
        {
            if (interval <= TimeSpan.Zero) return;
            var result = PulseRenderer.RenderNode(element, gradientLevelPair.Curve, gradientLevelPair.ColorGradient, interval, HasDiscreteColors);
            result.OffsetAllCommandsByTime(startTime);
            effectIntents.Add(result);
		}

	    private void InitializeDissolveClasses()
	    {
		    _nodes.Clear();
		    _tempNodes.Clear();
		    int colorCount = Colors.Count;

		    for (int x = 0; x < _totalNodes; x++)
		    {
			    DissolveClass m = new DissolveClass();
			    m.X = x;
			    m.ColorIndex = _random.Next(0, colorCount);
				_nodes.Add(m);
		    }

	    }
	    public class DissolveClass
	    {
		    public int X;
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