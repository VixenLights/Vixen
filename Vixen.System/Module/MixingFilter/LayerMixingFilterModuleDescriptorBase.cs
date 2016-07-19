using System;
using System.Collections.Generic;

namespace Vixen.Module.MixingFilter
{
	public abstract class LayerMixingFilterModuleDescriptorBase : ModuleDescriptorBase, ILayerMixingFilterModuleDescriptor,
															 IEqualityComparer<ILayerMixingFilterModuleDescriptor>,
															 IEquatable<ILayerMixingFilterModuleDescriptor>,
															 IEqualityComparer<LayerMixingFilterModuleDescriptorBase>,
															 IEquatable<LayerMixingFilterModuleDescriptorBase>
	{
		public abstract override string TypeName { get; }

		public abstract override Guid TypeId { get; }

		public abstract override Type ModuleClass { get; }

		public abstract override string Author { get; }

		public abstract override string Description { get; }

		public abstract override string Version { get; }

		public bool Equals(ILayerMixingFilterModuleDescriptor x, ILayerMixingFilterModuleDescriptor y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(ILayerMixingFilterModuleDescriptor obj)
		{
			return base.GetHashCode();
		}

		public bool Equals(ILayerMixingFilterModuleDescriptor other)
		{
			return base.Equals(other);
		}

		public bool Equals(LayerMixingFilterModuleDescriptorBase x, LayerMixingFilterModuleDescriptorBase y)
		{
			return Equals(x, y as ILayerMixingFilterModuleDescriptor);
		}

		public int GetHashCode(LayerMixingFilterModuleDescriptorBase obj)
		{
			return GetHashCode(obj as ILayerMixingFilterModuleDescriptor);
		}

		public bool Equals(LayerMixingFilterModuleDescriptorBase other)
		{
			return Equals(other as ILayerMixingFilterModuleDescriptor);
		}
	}

}
