using System.Drawing;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Module.MixingFilter
{
	public interface ILayerMixingFilter: IHasSetup
	{

		Color CombineFullColor(Color highLayerColor, Color lowLayerColor);

		DiscreteValue CombineDiscreteIntensity(DiscreteValue highLayerValue, DiscreteValue lowLayerValue);
	}
}
