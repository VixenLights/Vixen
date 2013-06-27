using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Media
{
	public abstract class MediaModuleDescriptorBase : ModuleDescriptorBase, IMediaModuleDescriptor,
	                                                  IEqualityComparer<IMediaModuleDescriptor>,
	                                                  IEquatable<IMediaModuleDescriptor>,
	                                                  IEqualityComparer<MediaModuleDescriptorBase>,
	                                                  IEquatable<MediaModuleDescriptorBase>
	{
		public abstract override string TypeName { get; }

		public abstract override Guid TypeId { get; }

		public abstract override Type ModuleClass { get; }

		public abstract override string Author { get; }

		public abstract override string Description { get; }

		public abstract override string Version { get; }

		public abstract string[] FileExtensions { get; }

		public abstract bool IsTimingSource { get; }

		public bool Equals(IMediaModuleDescriptor x, IMediaModuleDescriptor y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(IMediaModuleDescriptor obj)
		{
			return base.GetHashCode();
		}

		public bool Equals(IMediaModuleDescriptor other)
		{
			return base.Equals(other);
		}

		public bool Equals(MediaModuleDescriptorBase x, MediaModuleDescriptorBase y)
		{
			return Equals(x as IMediaModuleDescriptor, y as IMediaModuleDescriptor);
		}

		public int GetHashCode(MediaModuleDescriptorBase obj)
		{
			return GetHashCode(obj as IMediaModuleDescriptor);
		}

		public bool Equals(MediaModuleDescriptorBase other)
		{
			return Equals(other as IMediaModuleDescriptor);
		}
	}
}