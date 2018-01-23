using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Vixen.Annotations;
using Vixen.Module.MixingFilter;
using Vixen.Services;

namespace Vixen.Sys.LayerMixing
{
	[DataContract]
	public abstract class Layer : ILayer, IEquatable<Layer>, IEqualityComparer<Layer>, INotifyPropertyChanged
	{
		private ILayerMixingFilterInstance _layerMixingFilter;
		private string _layerName;
		private LayerType _type;
		private int _layerLevel;

		protected Layer()
		{
			Id = Guid.NewGuid();
		}

		[DataMember]
		public Guid Id { get; set; }

		[DataMember]
		public LayerType Type
		{
			get { return _type; }
			protected set
			{
				if (value == _type) return;
				_type = value;
				OnPropertyChanged();
			}
		}

		[DataMember]
		public int LayerLevel
		{
			get { return _layerLevel; }
			set
			{
				if (value == _layerLevel) return;
				_layerLevel = value;
				OnPropertyChanged();
			}
		}

		[DataMember]
		public string LayerName
		{
			get { return _layerName; }
			set
			{
				if (value == _layerName) return;
				_layerName = value;
				OnPropertyChanged();
			}
		}

		public string FilterName
		{
			get { return LayerMixingFilter != null ? LayerMixingFilter.Descriptor.TypeName : "None"; }
		}

		public Guid FilterTypeId
		{
			get { return LayerMixingFilter != null ? LayerMixingFilter.Descriptor.TypeId : Guid.Empty; }
			set
			{
				LayerMixingFilter = LayerMixingFilterService.Instance.GetInstance(value);
			}
		}

		public bool RequiresMixingPartner
		{
			get { return Type == LayerType.Standard && _layerMixingFilter.RequiresMixingPartner; }
		}

		public ILayerMixingFilterInstance LayerMixingFilter
		{
			get { return _layerMixingFilter; }
			set
			{
				if (Equals(value, _layerMixingFilter)) return;
				_layerMixingFilter = value;
				OnPropertyChanged();
				OnPropertyChanged("FilterName");
				OnPropertyChanged("FilterTypeId");
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

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
