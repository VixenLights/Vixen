using System;
using System.Runtime.Serialization;
using Vixen.Module.MixingFilter;
using Vixen.Services;

namespace Vixen.Module.SequenceType.Surrogate
{
	[DataContract(Namespace = "")]
	public class LayerMixingFilterSurrogate
	{
		public LayerMixingFilterSurrogate(ILayerMixingFilterInstance mediaModuleInstance)
		{
			TypeId = mediaModuleInstance.Descriptor.TypeId;
			InstanceId = mediaModuleInstance.InstanceId;
		}

		[DataMember]
		public Guid TypeId { get; private set; }

		[DataMember]
		public Guid InstanceId { get; private set; }

		public ILayerMixingFilterInstance CreateLayerMixingFilter()
		{
			ILayerMixingFilterInstance module = LayerMixingFilterService.Instance.GetInstance(TypeId);
			module.InstanceId = InstanceId;
			return module;
		}
	}
}
