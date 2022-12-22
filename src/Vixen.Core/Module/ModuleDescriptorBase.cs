﻿namespace Vixen.Module
{
	[Serializable]
	public abstract class ModuleDescriptorBase : IModuleDescriptor, IEqualityComparer<IModuleDescriptor>,
	                                             IEquatable<IModuleDescriptor>, IEqualityComparer<ModuleDescriptorBase>,
	                                             IEquatable<ModuleDescriptorBase>
	{
		public abstract string TypeName { get; }

		public abstract Guid TypeId { get; }

		public abstract Type ModuleClass { get; }

		public virtual Type ModuleDataClass
		{
			get { return null; }
		}

		public virtual Type ModuleStaticDataClass
		{
			get { return null; }
		}

		public abstract string Author { get; }

		public abstract string Description { get; }

		public abstract string Version { get; }

		public string FileName { get; set; }

		public System.Reflection.Assembly Assembly { get; set; }

		public virtual Guid[] Dependencies
		{
			get { return new Guid[] {}; }
		}

		public bool Equals(IModuleDescriptor x, IModuleDescriptor y)
		{
			return x.TypeId == y.TypeId;
		}

		public int GetHashCode(IModuleDescriptor obj)
		{
			return obj.TypeId.GetHashCode();
		}

		public bool Equals(IModuleDescriptor other)
		{
			return Equals(this, other);
		}

		public bool Equals(ModuleDescriptorBase x, ModuleDescriptorBase y)
		{
			return Equals(x as IModuleDescriptor, y as IModuleDescriptor);
		}

		public int GetHashCode(ModuleDescriptorBase obj)
		{
			return GetHashCode(obj as IModuleDescriptor);
		}

		public bool Equals(ModuleDescriptorBase other)
		{
			return Equals(other as IModuleDescriptor);
		}

		public override bool Equals(object obj)
		{
			if (obj is IModuleDescriptor) {
				return Equals(obj as IModuleDescriptor);
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return GetHashCode(this as IModuleDescriptor);
		}
	}
}