using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.MediaRenderer
{
	public abstract class MediaRendererModuleDescriptorBase : ModuleDescriptorBase, IMediaRendererModuleDescriptor,
	                                                          IEqualityComparer<IMediaRendererModuleDescriptor>,
	                                                          IEquatable<IMediaRendererModuleDescriptor>,
	                                                          IEqualityComparer<MediaRendererModuleDescriptorBase>,
	                                                          IEquatable<MediaRendererModuleDescriptorBase>
	{
		public abstract override string TypeName { get; }

		public abstract override Guid TypeId { get; }

		public abstract override Type ModuleClass { get; }

		public abstract override string Author { get; }

		public abstract override string Description { get; }

		public abstract override string Version { get; }

		public abstract string[] FileExtensions { get; }

		public bool Equals(IMediaRendererModuleDescriptor x, IMediaRendererModuleDescriptor y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(IMediaRendererModuleDescriptor obj)
		{
			return base.GetHashCode();
		}

		public bool Equals(IMediaRendererModuleDescriptor other)
		{
			return base.Equals(other);
		}

		public bool Equals(MediaRendererModuleDescriptorBase x, MediaRendererModuleDescriptorBase y)
		{
			return Equals(x as IMediaRendererModuleDescriptor, y as IMediaRendererModuleDescriptor);
		}

		public int GetHashCode(MediaRendererModuleDescriptorBase obj)
		{
			return GetHashCode(obj as IMediaRendererModuleDescriptor);
		}

		public bool Equals(MediaRendererModuleDescriptorBase other)
		{
			return Equals(other as IMediaRendererModuleDescriptor);
		}
	}
}