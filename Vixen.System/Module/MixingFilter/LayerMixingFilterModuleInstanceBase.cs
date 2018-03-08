using System;
using System.Collections.Generic;
using System.Drawing;
using Vixen.Data.Value;

namespace Vixen.Module.MixingFilter
{
	public abstract class LayerMixingFilterModuleInstanceBase : ModuleInstanceBase,
														   ILayerMixingFilterInstance,
														   IEqualityComparer<ILayerMixingFilterInstance>,
														   IEquatable<ILayerMixingFilterInstance>,
														   IEqualityComparer<LayerMixingFilterModuleInstanceBase>,
														   IEquatable<LayerMixingFilterModuleInstanceBase>
	{

		public virtual bool HasSetup
		{
			get { return false; }
		}

		public virtual bool Setup()
		{
			return false;
		}

		public virtual Color CombineFullColor(Color highLayerColor, Color lowLayerColor)
		{
			//A default implementation of highest layer wins.
			return highLayerColor;
		}

		public virtual DiscreteValue CombineDiscreteIntensity(DiscreteValue highLayerValue, DiscreteValue lowLayerValue)
		{
			//A default implementation of highest layer wins.
			return highLayerValue;
		}

		public virtual bool RequiresMixingPartner
		{
			get { return false; }
		}

		#region Equality

		public bool Equals(ILayerMixingFilterInstance x, ILayerMixingFilterInstance y)
		{
			return x.InstanceId == y.InstanceId;
		}

		public int GetHashCode(ILayerMixingFilterInstance obj)
		{
			return obj.InstanceId.GetHashCode();
		}

		public bool Equals(ILayerMixingFilterInstance other)
		{
			return Equals(this, other);
		}

		public bool Equals(LayerMixingFilterModuleInstanceBase x, LayerMixingFilterModuleInstanceBase y)
		{
			return Equals(x, y as ILayerMixingFilterInstance);
		}

		public int GetHashCode(LayerMixingFilterModuleInstanceBase obj)
		{
			return GetHashCode(obj as ILayerMixingFilterInstance);
		}

		public bool Equals(LayerMixingFilterModuleInstanceBase other)
		{
			return Equals(other as ILayerMixingFilterInstance);
		}
		
		#endregion

	}
}
