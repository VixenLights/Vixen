using System.Runtime.Serialization;
using Vixen.Module.MixingFilter;
using Vixen.Module.SequenceType.Surrogate;

namespace Vixen.Sys.LayerMixing
{
	[DataContract]
	public abstract class BaseLayerMixingDefinition : ILayerMixingDefinition
	{
		[DataMember]
		public LayerMixingDefinitionType Type { get; protected set; }

		[DataMember]
		public int LayerLevel { get; internal set; }

		[DataMember]
		public string LayerName { get; set; }

		public string FilterName
		{
			get { return LayerMixingFilter != null ? LayerMixingFilter.Descriptor.TypeName : "None"; }
		}

		[DataMember]
		private LayerMixingFilterSurrogate _layerMixingFilterSurrogate;


		public ILayerMixingFilterInstance LayerMixingFilter { get; set; }

		[OnSerializing]
		private void SurrogateWrite(StreamingContext context)
		{
			if (LayerMixingFilter != null)
			{
				_layerMixingFilterSurrogate = new LayerMixingFilterSurrogate(LayerMixingFilter);
			}
		}

		[OnDeserialized]
		private void SurrogateRead(StreamingContext context)
		{
			if (_layerMixingFilterSurrogate != null)
			{
				LayerMixingFilter = _layerMixingFilterSurrogate.CreateLayerMixingFilter();
			}
		}
	}
}
