using System;
using System.ComponentModel;
using Vixen.Sys;
using VixenModules.Effect.AudioHelp;
using VixenModules.Property.Color;

namespace VixenModules.Effect.VUMeter
{
    public class VUMeter : AudioPluginBase
	{
		private const int Spacing = 30;

		[Browsable(false)]
		public override int DepthOfEffect
		{
			//epth makes no difference here so we turn it off
			get { return Data.DepthOfEffect; }
			set
			{
				Data.DepthOfEffect = value;
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
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/vu-meter/"; }
		}

		#endregion

		// renders the given node to the internal ElementData dictionary. If the given node is
		// not a element, will recursively descend until we render its elements.
		protected override void RenderNode(IElementNode node)
		{
            if (!AudioUtilities.AudioLoaded)
                return;

			foreach (IElementNode elementNode in node.GetLeafEnumerator()) {
				// this is probably always going to be a single element for the given node, as
				// we have iterated down to leaf nodes in RenderNode() above. May as well do
				// it this way, though, in case something changes in future.
				if (elementNode == null || elementNode.Element == null)
					continue;
				bool discreteColors = ColorModule.isElementNodeDiscreteColored(elementNode);
				
                for(int i = 0;i<(int)((TimeSpan.TotalMilliseconds/Spacing)-1);i++)
                {

                    double gradientPosition1 = (AudioUtilities.VolumeAtTime(i * Spacing) + Data.Range)/Data.Range ;
                    double gradientPosition2 = (AudioUtilities.VolumeAtTime((i+1) * Spacing) + Data.Range)/Data.Range;
					if (gradientPosition1 <= 0)
						gradientPosition1 = 0;
					if (gradientPosition1 >= 1)
						gradientPosition1 = 1;

					//Some odd corner cases
					if (gradientPosition2 <= 0)
						gradientPosition2 = 0;
					if (gradientPosition2 >= 1)
						gradientPosition2 = 1;
					TimeSpan startTime = TimeSpan.FromMilliseconds(i * Spacing);
					ElementData.Add(GenerateEffectIntents(elementNode, WorkingGradient, MeterIntensityCurve, gradientPosition1, gradientPosition2, TimeSpan.FromMilliseconds(Spacing), startTime, discreteColors));
					
                }
                
		    }

		}

	}
}
