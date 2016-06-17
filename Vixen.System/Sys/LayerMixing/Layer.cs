using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vixen.Module.MixingFilter;
using Vixen.Module.SequenceType.Surrogate;

namespace Vixen.Sys.LayerMixing
{
	[DataContract]
	public abstract class Layer : ILayer, IEquatable<Layer>, IEqualityComparer<Layer>
	{
		protected Layer()
		{
			Id = Guid.NewGuid();
		}

		[DataMember]
		public Guid Id { get; set; }

		[DataMember]
		public LayerType Type { get; protected set; }

		[DataMember]
		public int LayerLevel { get; set; }

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

		public bool Equals(Layer other)
		{
			return Id == other.Id;
		}

		public bool Equals(Layer x, Layer y)
		{
			return x.Id == y.Id;
		}

		public int GetHashCode(Layer obj)
		{
			return obj.Id.GetHashCode();
		}
	}
}
