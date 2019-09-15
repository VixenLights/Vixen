using System;
using System.Linq;
using Vixen.Sys;
using Vixen.Attributes;
using Vixen.Sys.Attribute;
using VixenModules.Effect.AudioHelp;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using VixenModules.Property.Color;


namespace VixenModules.Effect.VerticalMeter
{
    public class VerticalMeter : AudioPluginBase
    {
		private const int Spacing = 30;

		[Value]
        [ProviderCategory(@"Color",3)]
        [PropertyOrder(6)]
        [ProviderDisplayName(@"Flip")]
        public bool Inverted
        {
            get { return ((VerticalMeterData)Data).Inverted; }
            set
            {
                ((VerticalMeterData)Data).Inverted = value;
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
		    get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/vertical-meter/"; }
	    }

	    #endregion

		// renders the given node to the internal ElementData dictionary. If the given node is
		// not a element, will recursively descend until we render its elements.
		protected override void RenderNode(IElementNode node)
		{
           int currentElement = 0;

			int elementCount = node.GetLeafEnumerator().Count();
            
           foreach (IElementNode elementNode in node.GetLeafEnumerator()) {
				// this is probably always going to be a single element for the given node, as
				// we have iterated down to leaf nodes in RenderNode() above. May as well do
				// it this way, though, in case something changes in future.
				if (elementNode == null || elementNode.Element == null)
					continue;

                if (!AudioUtilities.AudioLoaded)
                    return;
				bool discreteColors = ColorModule.isElementNodeDiscreteColored(elementNode);
				var lastTime = TimeSpan.FromMilliseconds(0);

                double gradientPosition = (double)(currentElement) / elementCount;

                //Audio max is at 0db. The threshold gets shifted from 0 to 1 to -1 to 0 and then scaled.
				double threshold;
				if (!((VerticalMeterData)Data).Inverted)
                {
                    threshold = (((double)(elementCount - currentElement)) / elementCount - 1) * Data.Range;
                    gradientPosition = 1 - gradientPosition;
                }
                else
                {
                    threshold = (((double)currentElement) / elementCount - 1) * Data.Range;
                        
                }
               
	           var lastValue = AudioUtilities.VolumeAtTime(0) >= threshold;

				TimeSpan start;
				for(int i = 1;i<(int)(TimeSpan.TotalMilliseconds/Spacing);i++)
                {
	                //Current time in ms = i*spacing
	                var currentValue = AudioUtilities.VolumeAtTime(i * Spacing) >= threshold;

	                if( currentValue != lastValue) {
                        start = lastTime;

                        if(lastValue)
                        {
	                        var effectIntents = GenerateEffectIntents(elementNode, WorkingGradient, MeterIntensityCurve, gradientPosition,
		                        gradientPosition, TimeSpan.FromMilliseconds(i*Spacing) - lastTime, start, discreteColors);
							ElementData.Add(effectIntents);
                        }

                        lastTime = TimeSpan.FromMilliseconds(i * Spacing);
                        lastValue = currentValue;
                    }
                }

				if (lastValue)
                {
                    start = lastTime;
					var effectIntents = GenerateEffectIntents(elementNode, WorkingGradient, MeterIntensityCurve, gradientPosition,
								gradientPosition, TimeSpan - lastTime, start, discreteColors);
					ElementData.Add(effectIntents);
				}

                currentElement++;
            }

		}
		
	}
}
