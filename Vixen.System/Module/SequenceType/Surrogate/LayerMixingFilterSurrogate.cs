using System;
using System.Runtime.Serialization;
using Vixen.Module.MixingFilter;
using Vixen.Services;
using Vixen.Sys.LayerMixing;

namespace Vixen.Module.SequenceType.Surrogate
{
	[DataContract(Namespace = "")]
	public class LayerMixingFilterSurrogate
	{
		public LayerMixingFilterSurrogate(ILayer layer)
		{
			TypeId = layer.LayerMixingFilter.Descriptor.TypeId;
			InstanceId = layer.LayerMixingFilter.InstanceId;
			LayerReferenceId = layer.Id;
		}

		[DataMember]
		public Guid TypeId { get; private set; }

		[DataMember]
		public Guid InstanceId { get; private set; }

		[DataMember]
		public Guid LayerReferenceId { get; private set; }

		public ILayerMixingFilterInstance CreateLayerMixingFilter()
		{
			ILayerMixingFilterInstance module = LayerMixingFilterService.Instance.GetInstance(TypeId);
			module.InstanceId = InstanceId;
			return module;
		}
	}
}
