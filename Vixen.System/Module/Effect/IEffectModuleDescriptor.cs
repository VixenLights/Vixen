using System.Drawing;
using Vixen.Sys;

namespace Vixen.Module.Effect
{
	public interface IEffectModuleDescriptor : IModuleDescriptor
	{
		string EffectName { get; }
		ParameterSignature Parameters { get; }
		EffectGroups EffectGroup { get; }
		bool SupportsMedia { get; }
		Image GetRepresentativeImage(int desiredWidth, int desiredHeight);
	}
}