using System;
using System.Runtime.Serialization;
using Vixen.Module.MixingFilter;

namespace Vixen.Sys.LayerMixing
{
	public interface ILayer
	{
		Guid Id { get; set; }
		LayerType Type { get; }

		string LayerName { get; set; }

		int LayerLevel { get; set; }

		string FilterName { get; }

		Guid FilterTypeId { get; set; }

		ILayerMixingFilterInstance LayerMixingFilter { get; set; }
	}

	[DataContract]
	public enum LayerType
	{
		[EnumMember]
		Default,
		[EnumMember]
		Standard
	}
}
