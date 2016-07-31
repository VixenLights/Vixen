using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.ComponentModel;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using Vixen.Execution;
using Vixen.Execution.Context;
using System.Drawing.Drawing2D;
using System.Linq;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using Vixen.Attributes;
using Vixen.TypeConverters;
using VixenModules.Effect.Effect;


namespace VixenModules.Effect.AudioHelp
{
    public enum MeterColorTypes { Linear, Discrete, Custom };

    public abstract class AudioPluginBase : BaseEffect
    {

        protected IAudioPluginData _data;
        protected EffectIntents _elementData = null;
        protected AudioHelper _audioHelper;
        protected Color[] _colors;
        protected TimeSpan _lastRenderedStartTime;

	    [Browsable(false)]
        public AudioHelper AudioHelper { get { return _audioHelper; } }

        private ColorGradient _workingGradient;

        #region Attribute Accessors

        [Value]
        [ProviderCategory(@"Audio Sensitivity Range")]
        [ProviderDisplayName(@"Low Pass Filter")]
        [ProviderDescription(@"Ignores frequencies below a given frequency")]
        [PropertyOrder(1)]
        public bool LowPass
        {
            get { return _data.LowPass; }
            set {
                _data.LowPass = value;
                _audioHelper.LowPass = value;
                IsDirty = true;
				OnPropertyChanged();
			}
        }

        [Value]
        [ProviderCategory(@"Audio Sensitivity Range")]
        [ProviderDisplayName(@"Low Pass Frequency")]
        [ProviderDescription(@"Ignore frequencies below this value")]
        [PropertyOrder(2)]
        public int LowPassFreq
        {
            get { return _data.LowPassFreq; }
            set {
                _data.LowPassFreq = value;
                _audioHelper.LowPassFreq = value;
                IsDirty = true;
				OnPropertyChanged();
			}
        }

        [Value]
        [ProviderCategory(@"Audio Sensitivity Range")]
        [ProviderDisplayName(@"High Pass Filter")]
        [ProviderDescription(@"Ignores frequencies above a given frequency")]
        [PropertyOrder(3)]
        public bool HighPass
        {
            get { return _data.HighPass; }
            set
            {
                _data.HighPass = value;
                _audioHelper.HighPass = value;
                IsDirty = true;
				OnPropertyChanged();
			}
        }

        [Value]
        [ProviderCategory(@"Audio Sensitivity Range")]
        [ProviderDisplayName(@"High Pass Frequency")]
        [ProviderDescription(@"Ignore frequencies above this value")]
        [PropertyOrder(4)]
        public int HighPassFreq
        {
            get { return _data.HighPassFreq; }
            set
            {
                _data.HighPassFreq = value;
                _audioHelper.HighPassFreq = value;
                IsDirty = true;
				OnPropertyChanged();
			}
        }

        [Value]
        [ProviderCategory(@"Audio Sensitivity Range")]
        [PropertyOrder(5)]
        [ProviderDisplayName(@"Gain")]
        [ProviderDescription(@"Boosts the volume")]
        [PropertyEditor("SliderEditor")]
        [NumberRange(0, 200, .5)]
        public int Gain
        {
            get { return _data.Gain*10; }
            set
            {
                _data.Gain = value/10;
                _audioHelper.Gain = value/10;
                IsDirty = true;
				OnPropertyChanged();
			}
        }

        [Value]
        [ProviderCategory(@"Audio Sensitivity Range")]
        [PropertyOrder(6)]
        [ProviderDisplayName(@"Zoom")]
        [ProviderDescription(@"The range of the volume levels displayed by the meter")]
        [PropertyEditor("SliderEditor")]
        [NumberRange(0, 20, 1)]
        public int Range
        {
            get { return 20 - _data.Range; }
            set
            {
                _data.Range = 20 - value;
                IsDirty = true;
				OnPropertyChanged();
			}
        }

        [Value]
        [ProviderCategory(@"Audio Sensitivity Range")]
        [ProviderDescription(@"Brings the peak volume of the selected audio range to the top of the meter")]
        [PropertyOrder(7)]
        public bool Normalize
        {
            get { return _data.Normalize; }
            set
            {
                _audioHelper.Normalize = value;
                _data.Normalize = value;
                IsDirty = true;
				OnPropertyChanged();
			}
        }

        [Value]
        [ProviderCategory(@"Response Speed")]
        [PropertyOrder(1)]
        [ProviderDisplayName(@"Decay Time")]
        [ProviderDescription(@"How quickly the meter falls from a volume peak")]
        [PropertyEditor("SliderEditor")]
        [NumberRange(0, 5000, 300)]
        public int DecayTime
        {
            get { return _data.DecayTime; }
            set
            {
                _data.DecayTime = value;
                _audioHelper.DecayTime = value;
                IsDirty = true;
				OnPropertyChanged();
			}
        }

        [Value]
        [ProviderCategory(@"Response Speed")]
        [PropertyOrder(2)]
        [ProviderDisplayName(@"Attack Time")]
        [ProviderDescription(@"How quickly the meter initially reacts to a volume peak")]
        [PropertyEditor("SliderEditor")]
        [NumberRange(0, 300, 10)]
        public int AttackTime
        {
            get { return _data.AttackTime; }
            set
            {
                _data.AttackTime = value;
                _audioHelper.AttackTime = value;
                IsDirty = true;
				OnPropertyChanged();
			}
        }

        [Value]
        [ProviderCategory(@"Color")]
        [PropertyOrder(1)]
        [ProviderDisplayName(@"Coloring Mode")]
        [ProviderDescription(@"Coloring Mode")]
        public MeterColorTypes MeterColorStyle
        {
            get { return _data.MeterColorStyle; }
            set
            {
                _data.MeterColorStyle = value;
				UpdateColorTypesAttributes(true);
                UpdateColorGradient();
                IsDirty = true;
				OnPropertyChanged();
			}
        }

        [Value]
        [ProviderCategory(@"Color")]
        [PropertyOrder(2)]
        [ProviderDisplayName(@"Green Gradient Position")]
        [ProviderDescription(@"Green Gradient Position")]
        [PropertyEditor("SliderEditor")]
        [NumberRange(1, 99, 1)]
        public int GreenColorPosition
        {
            get { return _data.GreenColorPosition; }
            set
            {
                _data.GreenColorPosition = value;
                UpdateColorGradient();
                IsDirty = true;
				OnPropertyChanged();
			}
        }

        [Value]
        [ProviderCategory(@"Color",0)]
        [PropertyOrder(3)]
        [ProviderDisplayName(@"Red Gradient Position")]
        [ProviderDescription(@"Red Gradient Position")]
        [PropertyEditor("SliderEditor")]
        [NumberRange(1, 99, 1)]
        public int RedColorPosition
        {
            get { return _data.RedColorPosition; }
            set
            {
                _data.RedColorPosition = value;
                UpdateColorGradient();
                IsDirty = true;
				OnPropertyChanged();
			}
        }

        [Value]
        [ProviderCategory(@"Color",0)]
        [PropertyOrder(4)]
        [ProviderDisplayName(@"Custom Gradient")]
        [ProviderDescription(@"Custom Gradient")]
        public ColorGradient MeterColorGradient
        {
            get { return _data.MeterColorGradient; }
            set
            {
                _data.MeterColorGradient = value;
                UpdateColorGradient();
                IsDirty = true;
				OnPropertyChanged();
			}
        }

        [Value]
        [ProviderCategory(@"Brightness",1)]
        [PropertyOrder(5)]
        [ProviderDisplayName(@"Intensity Curve")]
        public Curve MeterIntensityCurve
        {
            get { return _data.IntensityCurve; }
            set
            {
                _data.IntensityCurve = value;
                IsDirty = true;
				OnPropertyChanged();
			}
        }
		[Value]
		[ProviderCategory(@"Depth", 4)]
		[ProviderDisplayName(@"Depth")]
		[ProviderDescription(@"Depth")]
		[TypeConverter(typeof(TargetElementDepthConverter))]
		[PropertyEditor("SelectionEditor")]
		[MergableProperty(false)]
		public virtual int DepthOfEffect
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

		protected override void TargetNodesChanged()
		{
			CheckForInvalidColorData();
			if (DepthOfEffect > TargetNodes.FirstOrDefault().GetMaxChildDepth() - 1)
			{
				DepthOfEffect = 0;
			}
		}

		//Validate that the we are using valid colors and set appropriate defaults if not.
		private void CheckForInvalidColorData()
		{
			var validColors = GetValidColors();
			if (validColors.Any())
			{
				if (!_data.MeterColorGradient.GetColorsInGradient().IsSubsetOf(validColors))
				{
					//Our color is not valid for any elements we have.
					//Try to set a default color gradient from our available colors
					MeterColorGradient = new ColorGradient(validColors.First());
				}

				//ensure we are using Custom and we can't change it. We are limited in discrete color mode
				MeterColorStyle = MeterColorTypes.Custom;
				EnableColorTypesSelection(false);
			}
			else
			{
				EnableColorTypesSelection(true);
			}

		}

		private void InitAllAttributes()
		{
			UpdateColorTypesAttributes();
			TypeDescriptor.Refresh(this);
		}

	    protected void EnableColorTypesSelection(bool enable)
	    {
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
			{
				{"MeterColorStyle", enable}
			};
			SetBrowsable(propertyStates);
			TypeDescriptor.Refresh(this);
		}

		protected void UpdateColorTypesAttributes(bool refresh=false)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
			{
				{"MeterColorGradient", MeterColorStyle == MeterColorTypes.Custom},
				{"RedColorPosition", MeterColorStyle != MeterColorTypes.Custom},
				{"GreenColorPosition", MeterColorStyle != MeterColorTypes.Custom}

			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		protected Color GetColorAt(double pos)
        {
            if (_workingGradient == null) return Color.Black;
            return _workingGradient.GetColorAt(pos);
        }

	    protected ColorGradient WorkingGradient
	    {
		    get
		    {
			    return _workingGradient;
		    }
	    }

        protected void UpdateColorGradient(){
            if (_data == null)
                return;

            switch (_data.MeterColorStyle)
            {
                case MeterColorTypes.Discrete:
                    Color[] discreteColors = { Color.Lime, Color.Lime, Color.Yellow, Color.Yellow, Color.Red, Color.Red };
                    float greenPos = (float)_data.GreenColorPosition / 100;
                    float yellowPos = (float)_data.RedColorPosition / 100;
                    float[] DiscretePositions = { 0, greenPos, greenPos, yellowPos, yellowPos, 1 };
                    ColorBlend discreteBlend = new ColorBlend();
                    discreteBlend.Colors = discreteColors;
                    discreteBlend.Positions = DiscretePositions;
                    ColorGradient discreteGradient = new ColorGradient(discreteBlend);
                    _workingGradient = discreteGradient;
                    break;
                case MeterColorTypes.Linear:
                    Color[] linearColors = { Color.Lime, Color.Yellow, Color.Red };
                    float[] myPositions = { 0, (float)_data.GreenColorPosition / 100, (float)_data.RedColorPosition / 100 };
                    ColorBlend linearBlend = new ColorBlend();
                    linearBlend.Colors = linearColors;
                    linearBlend.Positions = myPositions;

                    ColorGradient linearGradient = new ColorGradient(linearBlend);
                    _workingGradient = linearGradient;
                    break;
                case MeterColorTypes.Custom: _workingGradient = _data.MeterColorGradient; break;
            }
        }

        protected override void _PreRender(CancellationTokenSource tokenSource = null)
        {
            _elementData = new EffectIntents();

            _audioHelper.ReloadAudio();

	        var nodes = GetNodesToRenderOn();

            foreach (ElementNode node in nodes)
            {
                if (tokenSource != null && tokenSource.IsCancellationRequested)
                    return;

                if (node != null)
                    RenderNode(node);
            }
        }

		private List<ElementNode> GetNodesToRenderOn()
		{
			IEnumerable<ElementNode> renderNodes = null;

			if (DepthOfEffect == 0)
			{
				renderNodes = TargetNodes;
			}
			else
			{
				renderNodes = TargetNodes;
				for (int i = 0; i < DepthOfEffect; i++)
				{
					renderNodes = renderNodes.SelectMany(x => x.Children);
				}
			}

			// If the given DepthOfEffect results in no nodes (because it goes "too deep" and misses all nodes), 
			// then we'll default to the LeafElements, which will at least return 1 element (the TargetNode)
			if (!renderNodes.Any())
				renderNodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator());

			return renderNodes.ToList();
		}

		protected override EffectIntents _Render()
		{
			return _elementData;
		}

        public override IModuleDataModel ModuleData
		{
            get { return _data; }
			set {
                _data = value as IAudioPluginData;
                UpdateColorGradient();
				InitAllAttributes();
                _audioHelper.DecayTime = _data.DecayTime;
                _audioHelper.AttackTime = _data.AttackTime;
                _audioHelper.Gain = _data.Gain;
                _audioHelper.Normalize = _data.Normalize;
                _audioHelper.LowPass = _data.LowPass;
                _audioHelper.LowPassFreq = _data.LowPassFreq;
                _audioHelper.HighPass = _data.HighPass;
                _audioHelper.HighPassFreq = _data.HighPassFreq;
            }
		}

       protected abstract void RenderNode(ElementNode node);

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return (EffectTypeModuleData)_data; }
		}

		/// <summary>
		/// Convienience method to generate intents that knows how to deal with discrete colors.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="gradient"></param>
		/// <param name="level"></param>
		/// <param name="startPos"></param>
		/// <param name="endPos"></param>
		/// <param name="duration"></param>
		/// <param name="startTime"></param>
		/// <param name="isDiscrete"></param>
		/// <returns></returns>
		protected EffectIntents GenerateEffectIntents(ElementNode node, ColorGradient gradient, Curve level, double startPos, double endPos,
			TimeSpan duration, TimeSpan startTime, bool isDiscrete)
		{
			EffectIntents result = new EffectIntents();
			if (isDiscrete)
			{
				foreach (Color color in gradient.GetColorsInGradient())
				{
					double proportion = gradient.GetProportionOfColorAt(startPos, color);
					var startIntensity = (level.GetValue(startPos * 100) / 100) * proportion;
					proportion = gradient.GetProportionOfColorAt(endPos, color);
					var endIntensity = (level.GetValue(endPos * 100) / 100) * proportion;
					if (startIntensity > 0 && endIntensity > 0)
					{
						var intent = CreateDiscreteIntent(color, startIntensity, endIntensity, duration);
						result.AddIntentForElement(node.Element.Id, intent, startTime);
					}
				}
			}
			else
			{
				var startIntensity = level.GetValue(startPos * 100) / 100;
				var endIntensity = level.GetValue(endPos * 100) / 100;
				if (startIntensity > 0 && endIntensity > 0)
				{
					var intent = CreateIntent(gradient.GetColorAt(startPos), gradient.GetColorAt(endPos), startIntensity, endIntensity, duration);
					result.AddIntentForElement(node.Element.Id, intent, startTime);
				}
			}

			return result;
		}


	}
}
