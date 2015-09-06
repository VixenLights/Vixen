using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.ComponentModel;
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

        [Value]
        [ProviderCategory(@"Color")]
        [PropertyOrder(6)]
        [ProviderDisplayName(@"Flip")]
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
            
            int spacing = 15; //smoothness. Intent length in ms
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

                    //Audio max is at 0db. The threshold gets shifted from 0 to 1 to -1 to 0 and then scaled.
                    if (((VerticalMeterData)_data).Inverted)
                    {
                        threshold = (((double)(ElementCount - currentElement)) / ElementCount - 1) * _data.Range;
                        GradientPosition = 1 - GradientPosition;
                    }
                    else
                    {
                        threshold = (((double)currentElement) / ElementCount - 1) * _data.Range;
                        
                    }
                    color = new LightingValue(GetColorAt(GradientPosition), MeterIntensityCurve.GetValue(GradientPosition*100)/100);
                   
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
