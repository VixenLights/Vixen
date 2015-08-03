using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Sys;
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

namespace VixenModules.Effect.VUMeter
{
    public class VUMeter : AudioPluginBase
	{

        protected override void TargetNodesChanged()
        {

        }

        public VUMeter()
        {
            _audioHelper = new AudioHelper(this);
        }

		// renders the given node to the internal ElementData dictionary. If the given node is
		// not a element, will recursively descend until we render its elements.
		protected override void RenderNode(ElementNode node)
		{
            _elementData.Clear();

            if (!_audioHelper.AudioLoaded)
                return;
            
            int spacing = 30; //smoothness. Intent length in ms
            LightingValue color1, color2;

			foreach (ElementNode elementNode in node.GetLeafEnumerator()) {
				// this is probably always going to be a single element for the given node, as
				// we have iterated down to leaf nodes in RenderNode() above. May as well do
				// it this way, though, in case something changes in future.
				if (elementNode == null || elementNode.Element == null)
					continue;

			    if (elementNode.Element != null)
			    {
                    for(int i = 1;i<(int)((TimeSpan.TotalMilliseconds/spacing)-1);i++)
                    {

                        double GradientPosition1 = (_audioHelper.VolumeAtTime(i * spacing) + _data.Range)/_data.Range ;
                        double GradientPosition2 = (_audioHelper.VolumeAtTime((i+1) * spacing) + _data.Range)/_data.Range;

                        //Some odd corner cases
                        if (GradientPosition1 <= 0)
                            GradientPosition1 = .001;
                        if (GradientPosition1 >= 1)
                            GradientPosition1 = .999;

                        //Some odd corner cases
                        if (GradientPosition2 <= 0)
                            GradientPosition2 = .001;
                        if (GradientPosition2 >= 1)
                            GradientPosition2 = .999;

                        color1 = new LightingValue(GetColorAt(GradientPosition1));
                        color2 = new LightingValue(GetColorAt(GradientPosition2));

                        TimeSpan startTime = TimeSpan.FromMilliseconds(i * spacing);
                        TimeSpan endTime = TimeSpan.FromMilliseconds((i + 1) * spacing);

                        int startAudioTime = i * spacing + 1;

                        IIntent intent = new LightingIntent(color1, color2, TimeSpan.FromMilliseconds(spacing));

                        _elementData.AddIntentForElement(elementNode.Element.Id, intent, startTime);
                    }
                }
		    }

		}


	}
}
