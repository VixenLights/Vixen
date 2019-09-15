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
using System.Drawing.Drawing2D;
using System.Linq;
using NLog;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using Vixen.Attributes;
using Vixen.Module.Media;
using Vixen.TypeConverters;
using VixenModules.Effect.Effect;
using VixenModules.Media.Audio;
using VixenModules.Property.Color;


namespace VixenModules.Effect.AudioHelp
{
    public enum MeterColorTypes { Linear, Discrete, Custom };

    public abstract class AudioPluginBase : BaseEffect
    {
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		protected IAudioPluginData Data;
        protected EffectIntents ElementData = null;
        private readonly AudioUtilities _audioUtilities;
        protected Color[] Colors;
        protected TimeSpan LastRenderedStartTime;

	    protected AudioPluginBase()
	    {
		    _audioUtilities = new AudioUtilities();
	    }

	    [Browsable(false)]
        public AudioUtilities AudioUtilities { get { return _audioUtilities; } }

        private ColorGradient _workingGradient;

        #region Attribute Accessors

		#region AudioSensitivityRange
		[Value]
		[ProviderCategory(@"AudioSensitivityRange", 1)]
		[PropertyOrder(0)]
		[ProviderDisplayName(@"Gain")]
		[ProviderDescription(@"Boosts the volume")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 200, .5)]
		public int Gain
		{
			get { return Data.Gain * 10; }
			set
			{
				Data.Gain = value / 10;
				_audioUtilities.Gain = value / 10;
				IsDirty = true;
				OnPropertyChanged();
			}
		}
		[Value]
		[ProviderCategory(@"AudioSensitivityRange", 1)]
		[ProviderDisplayName(@"HighPassFilter")]
		[ProviderDescription(@"Passes frequencies above a given frequency")]
		[PropertyOrder(1)]
		public bool HighPass
		{
			get { return Data.HighPass; }
			set
			{
				Data.HighPass = value;
				_audioUtilities.HighPass = value;
				IsDirty = true;
				UpdateLowHighPassAttributes(true);
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"AudioSensitivityRange", 1)]
		[ProviderDisplayName(@"HighPassFrequency")]
		[ProviderDescription(@"Passes frequencies above this value")]
		[PropertyOrder(2)]
		public int HighPassFreq
		{
			get { return Data.HighPassFreq; }
			set
			{
				Data.HighPassFreq = value;
				_audioUtilities.HighPassFreq = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

        [Value]
        [ProviderCategory(@"AudioSensitivityRange", 1)]
        [ProviderDisplayName(@"LowPassFilter")]
		[ProviderDescription(@"Passes frequencies below a given frequency")]
        [PropertyOrder(3)]
        public bool LowPass
        {
            get { return Data.LowPass; }
            set {
                Data.LowPass = value;
                _audioUtilities.LowPass = value;
                IsDirty = true;
	            UpdateLowHighPassAttributes(true);
				OnPropertyChanged();
			}
        }

        [Value]
        [ProviderCategory(@"AudioSensitivityRange", 1)]
        [ProviderDisplayName(@"LowPassFrequency")]
        [ProviderDescription(@"Passes frequencies below this value")]
        [PropertyOrder(4)]
        public int LowPassFreq
        {
            get { return Data.LowPassFreq; }
            set {
                Data.LowPassFreq = value;
                _audioUtilities.LowPassFreq = value;
                IsDirty = true;
				OnPropertyChanged();
			}
        }

		[Value]
		[ProviderCategory(@"AudioSensitivityRange", 1)]
		[ProviderDescription(@"Brings the peak volume of the selected audio range to the top of the meter")]
		[PropertyOrder(5)]
		public bool Normalize
		{
			get { return Data.Normalize; }
			set
			{
				_audioUtilities.Normalize = value;
				Data.Normalize = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}
       
        [Value]
        [ProviderCategory(@"AudioSensitivityRange", 1)]
        [PropertyOrder(6)]
        [ProviderDisplayName(@"Zoom")]
        [ProviderDescription(@"The range of the volume levels displayed by the meter")]
        [PropertyEditor("SliderEditor")]
        [NumberRange(0, 20, 1)]
        public int Range
        {
            get { return 20 - Data.Range; }
            set
            {
                Data.Range = 20 - value;
                IsDirty = true;
				OnPropertyChanged();
			}
        }

		#endregion

		#region Response Speed

		[Value]
		[ProviderCategory(@"ResponseSpeed", 2)]
		[PropertyOrder(1)]
		[ProviderDisplayName(@"AttackTime")]
		[ProviderDescription(@"How quickly the meter initially reacts to a volume peak")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 300, 10)]
		public int AttackTime
		{
			get { return Data.AttackTime; }
			set
			{
				Data.AttackTime = value;
				_audioUtilities.AttackTime = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
        [ProviderCategory(@"ResponseSpeed", 2)]
        [PropertyOrder(2)]
        [ProviderDisplayName(@"DecayTime")]
        [ProviderDescription(@"How quickly the meter falls from a volume peak")]
        [PropertyEditor("SliderEditor")]
        [NumberRange(0, 5000, 300)]
        public int DecayTime
        {
            get { return Data.DecayTime; }
            set
            {
                Data.DecayTime = value;
                _audioUtilities.DecayTime = value;
                IsDirty = true;
				OnPropertyChanged();
			}
        }

		#endregion

#region Color

		[Value]
        [ProviderCategory(@"Color", 3)]
        [PropertyOrder(1)]
        [ProviderDisplayName(@"ColorHandling")]
        [ProviderDescription(@"ColorHandling")]
        public MeterColorTypes MeterColorStyle
        {
            get { return Data.MeterColorStyle; }
            set
            {
                Data.MeterColorStyle = value;
				UpdateColorTypesAttributes(true);
                UpdateColorGradient();
                IsDirty = true;
				OnPropertyChanged();
			}
        }

        [Value]
        [ProviderCategory(@"Color", 3)]
        [PropertyOrder(2)]
        [ProviderDisplayName(@"Green Gradient Position")]
        [ProviderDescription(@"GradientPosition")]
        [PropertyEditor("SliderEditor")]
        [NumberRange(1, 99, 1)]
        public int GreenColorPosition
        {
            get { return Data.GreenColorPosition; }
            set
            {
                Data.GreenColorPosition = value;
                UpdateColorGradient();
                IsDirty = true;
				OnPropertyChanged();
			}
        }

        [Value]
        [ProviderCategory(@"Color",3)]
        [PropertyOrder(3)]
        [ProviderDisplayName(@"Red Gradient Position")]
        [ProviderDescription(@"GradientPosition")]
        [PropertyEditor("SliderEditor")]
        [NumberRange(1, 99, 1)]
        public int RedColorPosition
        {
            get { return Data.RedColorPosition; }
            set
            {
                Data.RedColorPosition = value;
                UpdateColorGradient();
                IsDirty = true;
				OnPropertyChanged();
			}
        }

        [Value]
        [ProviderCategory(@"Color",3)]
        [PropertyOrder(4)]
        [ProviderDisplayName(@"Custom Gradient")]
        [ProviderDescription(@"Color")]
        public ColorGradient MeterColorGradient
        {
            get { return Data.MeterColorGradient; }
            set
            {
                Data.MeterColorGradient = value;
                UpdateColorGradient();
                IsDirty = true;
				OnPropertyChanged();
			}
        }

#endregion

		#region Brightness

		[Value]
        [ProviderCategory(@"Brightness",4)]
        [PropertyOrder(1)]
        [ProviderDisplayName(@"Brightness")]
		[ProviderDescription(@"Brightness")]
        public Curve MeterIntensityCurve
        {
            get { return Data.IntensityCurve; }
            set
            {
                Data.IntensityCurve = value;
                IsDirty = true;
				OnPropertyChanged();
			}
        }

		#endregion

		#region Depth

		[Value]
		[ProviderCategory(@"Depth", 5)]
		[ProviderDisplayName(@"Depth")]
		[ProviderDescription(@"Depth")]
		[TypeConverter(typeof(TargetElementDepthConverter))]
		[PropertyEditor("SelectionEditor")]
		public virtual int DepthOfEffect
		{
			get { return Data.DepthOfEffect; }
			set
			{
				Data.DepthOfEffect = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#endregion

		protected override void TargetNodesChanged()
		{
			CheckForInvalidColorData();
			var firstNode = TargetNodes.FirstOrDefault();
			if (firstNode != null && DepthOfEffect > firstNode.GetMaxChildDepth() - 1)
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
				if (!Data.MeterColorGradient.GetColorsInGradient().IsSubsetOf(validColors))
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
			UpdateLowHighPassAttributes();
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

		protected void UpdateLowHighPassAttributes(bool refresh = false)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
			{
				{"LowPassFreq", LowPass},
				{"HighPassFreq", HighPass}
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
            if (Data == null)
                return;

            switch (Data.MeterColorStyle)
            {
                case MeterColorTypes.Discrete:
                    Color[] discreteColors = { Color.Lime, Color.Lime, Color.Yellow, Color.Yellow, Color.Red, Color.Red };
                    float greenPos = (float)Data.GreenColorPosition / 100;
                    float yellowPos = (float)Data.RedColorPosition / 100;
                    float[] DiscretePositions = { 0, greenPos, greenPos, yellowPos, yellowPos, 1 };
                    ColorBlend discreteBlend = new ColorBlend();
                    discreteBlend.Colors = discreteColors;
                    discreteBlend.Positions = DiscretePositions;
                    ColorGradient discreteGradient = new ColorGradient(discreteBlend);
                    _workingGradient = discreteGradient;
                    break;
                case MeterColorTypes.Linear:
                    Color[] linearColors = { Color.Lime, Color.Yellow, Color.Red };
                    float[] myPositions = { 0, (float)Data.GreenColorPosition / 100, (float)Data.RedColorPosition / 100 };
                    ColorBlend linearBlend = new ColorBlend();
                    linearBlend.Colors = linearColors;
                    linearBlend.Positions = myPositions;

                    ColorGradient linearGradient = new ColorGradient(linearBlend);
                    _workingGradient = linearGradient;
                    break;
                case MeterColorTypes.Custom: _workingGradient = Data.MeterColorGradient; break;
            }
        }

        protected override void _PreRender(CancellationTokenSource tokenSource = null)
        {
            ElementData = new EffectIntents();

			if (Media == null || Media.Count == 0)
			{
				Logging.Warn("No audio available to render effect!");
				return;
			}

			foreach (IMediaModuleInstance module in Media)
			{
				var audio = module as Audio;
				if (audio != null)
				{
					if (audio.Channels == 0)
					{
						continue;
					}
						
					_audioUtilities.TimeSpan = TimeSpan;
					_audioUtilities.StartTime = StartTime;
					_audioUtilities.ReloadAudio(audio);
				}
			}

			if (!_audioUtilities.AudioLoaded)
			{
				Logging.Warn("Unable to load audio to render audio effect!");
				return;
			}

			var nodes = GetNodesToRenderOn();

            foreach (IElementNode node in nodes)
            {
                if (tokenSource != null && tokenSource.IsCancellationRequested)
                    return;

                if (node != null)
                    RenderNode(node);
            }

			_audioUtilities.FreeMem();
        }

		private List<IElementNode> GetNodesToRenderOn()
		{
			IEnumerable<IElementNode> renderNodes = null;

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
			return ElementData;
		}

        public override IModuleDataModel ModuleData
		{
            get { return Data; }
			set {
                Data = value as IAudioPluginData;
				CheckForInvalidColorData();
				UpdateColorGradient();
				InitAllAttributes();
                _audioUtilities.DecayTime = Data.DecayTime;
                _audioUtilities.AttackTime = Data.AttackTime;
                _audioUtilities.Gain = Data.Gain;
                _audioUtilities.Normalize = Data.Normalize;
                _audioUtilities.LowPass = Data.LowPass;
                _audioUtilities.LowPassFreq = Data.LowPassFreq;
                _audioUtilities.HighPass = Data.HighPass;
                _audioUtilities.HighPassFreq = Data.HighPassFreq;
            }
		}

       protected abstract void RenderNode(IElementNode node);

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return (EffectTypeModuleData)Data; }
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
		protected EffectIntents GenerateEffectIntents(IElementNode node, ColorGradient gradient, Curve level, double startPos, double endPos,
			TimeSpan duration, TimeSpan startTime, bool isDiscrete)
		{
			EffectIntents result = new EffectIntents();
			if (isDiscrete)
			{
				IEnumerable<Color> colors = ColorModule.getValidColorsForElementNode(node, false)
						 .Intersect(gradient.GetColorsInGradient());
				foreach (Color color in colors)
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
