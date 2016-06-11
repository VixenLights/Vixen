using System.Runtime.Serialization;
using Vixen.Module.MixingFilter;

namespace Vixen.Sys.LayerMixing
{
	public interface ILayerMixingDefinition
	{

		LayerMixingDefinitionType Type { get; }

		int LayerLevel { get; }

		string LayerName { get; set; }

		string FilterName { get; }

		ILayerMixingFilterInstance LayerMixingFilter { get; set; }

	}

	[DataContract]
	public enum LayerMixingDefinitionType
	{
		[EnumMember]
		Default,
		[EnumMember]
		Standard
	}
}
