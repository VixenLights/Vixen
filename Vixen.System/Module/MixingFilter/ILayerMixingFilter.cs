using System.Drawing;
using Vixen.Data.Value;

namespace Vixen.Module.MixingFilter
{
	public interface ILayerMixingFilter
	{

		Color CombineFullColor(Color highLayerColor, Color lowLayerColor);

		DiscreteValue CombineDiscreteIntensity(DiscreteValue highLayerValue, DiscreteValue lowLayerValue);
	}
}
