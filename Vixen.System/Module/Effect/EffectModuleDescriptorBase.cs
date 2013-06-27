using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Vixen.Sys;

namespace Vixen.Module.Effect
{
	[Serializable]
	public abstract class EffectModuleDescriptorBase : ModuleDescriptorBase, IEffectModuleDescriptor,
	                                                   IEqualityComparer<IEffectModuleDescriptor>,
	                                                   IEquatable<IEffectModuleDescriptor>,
	                                                   IEqualityComparer<EffectModuleDescriptorBase>,
	                                                   IEquatable<EffectModuleDescriptorBase>
	{
		protected EffectModuleDescriptorBase()
		{
			PropertyDependencies = new Guid[0];
		}

		public abstract override string TypeName { get; }

		public abstract override Guid TypeId { get; }

		public abstract override Type ModuleClass { get; }

		public abstract override string Author { get; }

		public abstract override string Description { get; }

		public abstract override string Version { get; }

		public abstract string EffectName { get; }

		public abstract ParameterSignature Parameters { get; }

		public virtual Guid[] PropertyDependencies { get; private set; }

		public virtual Image GetRepresentativeImage(int desiredWidth, int desiredHeight)
		{
			int maxDimension = Math.Max(desiredWidth, desiredHeight);
			if (maxDimension <= 16) {
				return ImageResource.Effect16;
			}
			else if (maxDimension <= 48) {
				return ImageResource.Effect48;
			}
			else {
				return ImageResource.Effect64;
			}
		}

		public bool Equals(IEffectModuleDescriptor x, IEffectModuleDescriptor y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(IEffectModuleDescriptor obj)
		{
			return base.GetHashCode();
		}

		public bool Equals(IEffectModuleDescriptor other)
		{
			return base.Equals(other);
		}

		public bool Equals(EffectModuleDescriptorBase x, EffectModuleDescriptorBase y)
		{
			return Equals(x as IEffectModuleDescriptor, y as IEffectModuleDescriptor);
		}

		public int GetHashCode(EffectModuleDescriptorBase obj)
		{
			return GetHashCode(obj as IEffectModuleDescriptor);
		}

		public bool Equals(EffectModuleDescriptorBase other)
		{
			return Equals(other as IEffectModuleDescriptor);
		}
	}
}