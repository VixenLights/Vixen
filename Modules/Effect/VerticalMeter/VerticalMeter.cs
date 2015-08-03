using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Sys;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Property.Color;
using ZedGraph;
using System.Windows.Forms;
using Vixen.Module.Media;
using Vixen.Services;
using VixenModules.Media.Audio;
using Vixen.Execution;
using Vixen.Execution.Context;
using VixenModules.Effect.AudioHelp;
using VixenModules.EffectEditor.EffectDescriptorAttributes;


namespace VixenModules.Effect.VerticalMeter
{
    public class VerticalMeter : AudioPluginBase
    {

        #region Config Properties

        [Value]
        [ProviderCategory(@"Config",1)]
        [ProviderDisplayName(@"Attack Time")]
        [ProviderDescription(@"Attack Time")]
        [PropertyEditor("SliderEditor")]
        [NumberRange(0, 300, 10)]
        [PropertyOrder(0)]
        public int AttackTime
        {
            get { return _data.AttackTime; }
            set { _data.AttackTime = value; _audioHelper.AttackTime = value; _audioHelper.RecalculateVolume(); IsDirty = true; OnPropertyChanged(); }
        }

        [Value]
        [ProviderCategory(@"Config", 1)]
        [ProviderDisplayName(@"Decay Time")]
        [ProviderDescription(@"Decay Time")]
        [PropertyEditor("SliderEditor")]
        [NumberRange(0, 5000, 300)]
        [PropertyOrder(1)]
        public int DecayTime
        {
            get { return _data.DecayTime; }
            set { _data.DecayTime = value; _audioHelper.DecayTime = value; _audioHelper.RecalculateVolume(); IsDirty = true; OnPropertyChanged(); }
        }

        [Value]
        [ProviderCategory(@"Config", 1)]
        [ProviderDisplayName(@"Minimum")]
        [ProviderDescription(@"Minimum")]
        [PropertyEditor("SliderEditor")]
        [NumberRange(0, 50, 5)]
        [PropertyOrder(2)]
        public int DecayTime
        {
            get { return _data.DecayTime; }
            set { _data.Range = (50 - value)-_data.Gain; IsDirty = true; OnPropertyChanged(); }
        }

        [Value]
        [ProviderCategory(@"Config", 1)]
        [ProviderDisplayName(@"Peak")]
        [ProviderDescription(@"Peak")]
        [PropertyEditor("SliderEditor")]
        [NumberRange(0, 50, 5)]
        [PropertyOrder(3)]
        public int DecayTime
        {
            get { return _data.DecayTime; }
            set {  _audioHelper.RecalculateVolume(); IsDirty = true; OnPropertyChanged(); }
        }

        #endregion Config Properties

        public bool Inverted
        {
            get { return ((VerticalMeterData)_data).Inverted; }
            set
            {
                ((VerticalMeterData)_data).Inverted = value;
                IsDirty = true;
            }
        }

        protected override void TargetNodesChanged()
        {

        }

        public VerticalMeter()
        {
            _audioHelper = new AudioHelper(this);
        }

		// renders the given node to the internal ElementData dictionary. If the given node is
		// not a element, will recursively descend until we render its elements.
		protected override void RenderNode(ElementNode node)
		{
            _elementData.Clear();

            int currentElement = 0;
            bool lastValue = false;
            bool currentValue = false;
            TimeSpan lastTime;
            TimeSpan start;

            int ElementCount = node.GetLeafEnumerator().Count();
            
            int spacing = 30; //smoothness. Intent length in ms
            double threshold;
            LightingValue color;

			foreach (ElementNode elementNode in node.GetLeafEnumerator()) {
				// this is probably always going to be a single element for the given node, as
				// we have iterated down to leaf nodes in RenderNode() above. May as well do
				// it this way, though, in case something changes in future.
				if (elementNode == null || elementNode.Element == null)
					continue;

                if (!_audioHelper.AudioLoaded)
                    return;

			    if (elementNode.Element != null)
			    {
                    lastTime = TimeSpan.FromMilliseconds(0);

                    double GradientPosition = (double)(currentElement) / ElementCount;

                    //Some odd corner cases
                    if (GradientPosition == 0)
                        GradientPosition = .001;
                    if (GradientPosition == 1)
                        GradientPosition = .999;

                    if (((VerticalMeterData)_data).Inverted)
                    {
                        threshold = (((double)(ElementCount - currentElement)) / ElementCount - 1) * _data.Range;
                        GradientPosition = 1 - GradientPosition;
                    }
                    else
                    {
                        threshold = (((double)currentElement) / ElementCount - 1) * _data.Range;
                        
                    }
                    color = new LightingValue(GetColorAt(GradientPosition));
                    lastValue = _audioHelper.VolumeAtTime(0) >= threshold;

                    for(int i = 1;i<(int)(TimeSpan.TotalMilliseconds/spacing);i++)
                    {
                        //Current time in ms = i*spacing
                        currentValue = _audioHelper.VolumeAtTime(i * spacing) >= threshold;

                        if( currentValue != lastValue) {
                            start = lastTime;

                            if(lastValue) {
                                IIntent intent = new LightingIntent(color, color, TimeSpan.FromMilliseconds(i*spacing) - lastTime );
                                _elementData.AddIntentForElement(elementNode.Element.Id, intent, start);
                            }

                            lastTime = TimeSpan.FromMilliseconds(i * spacing);
                            lastValue = currentValue;
                        }

                    }

                    if (lastValue)
                    {
                        start = lastTime;
                        IIntent finalIntent = new LightingIntent(color, color, TimeSpan - lastTime);
                        _elementData.AddIntentForElement(elementNode.Element.Id, finalIntent, start);
                    }

                    currentElement++;
                }
		    }

		}


	}
}
