using System;
using System.Collections.Generic;

namespace Vixen.Module.Preview
{
	public abstract class PreviewModuleDescriptorBase : ModuleDescriptorBase, IPreviewModuleDescriptor,
	                                                    IEqualityComparer<IPreviewModuleDescriptor>,
	                                                    IEquatable<IPreviewModuleDescriptor>,
	                                                    IEqualityComparer<PreviewModuleDescriptorBase>,
	                                                    IEquatable<PreviewModuleDescriptorBase>
	{
		private const int DEFAULT_UPDATE_INTERVAL = 50;

		public abstract override string TypeName { get; }

		public abstract override Guid TypeId { get; }

		public abstract override Type ModuleClass { get; }

		public abstract override string Author { get; }

		public abstract override string Description { get; }

		public abstract override string Version { get; }

		public virtual int UpdateInterval
		{
			get { return DEFAULT_UPDATE_INTERVAL; }
		}

		public bool Equals(IPreviewModuleDescriptor x, IPreviewModuleDescriptor y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(IPreviewModuleDescriptor obj)
		{
			return base.GetHashCode();
		}

		public bool Equals(IPreviewModuleDescriptor other)
		{
			return base.Equals(other);
		}

		public bool Equals(PreviewModuleDescriptorBase x, PreviewModuleDescriptorBase y)
		{
			return Equals(x as IPreviewModuleDescriptor, y as IPreviewModuleDescriptor);
		}

		public int GetHashCode(PreviewModuleDescriptorBase obj)
		{
			return GetHashCode(obj as IPreviewModuleDescriptor);
		}

		public bool Equals(PreviewModuleDescriptorBase other)
		{
			return Equals(other is IPreviewModuleDescriptor);
		}
	}
}