using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Vixen.Module
{
	[Serializable]
	public abstract class ModuleInstanceBase : IModuleInstance, IEqualityComparer<IModuleInstance>,
	                                           IEquatable<IModuleInstance>, IEqualityComparer<ModuleInstanceBase>,
	                                           IEquatable<ModuleInstanceBase>
	{
		protected ModuleInstanceBase()
		{
			InstanceId = Guid.NewGuid();
		}

		[Browsable(false)]
		public Guid InstanceId { get; set; }

		[Browsable(false)]
		public Guid TypeId
		{
			get { return Descriptor.TypeId; }
		}

		[Browsable(false)]
		public virtual IModuleDataModel ModuleData { get; set; }

		[Browsable(false)]
		public virtual IModuleDataModel StaticModuleData { get; set; }

		[Browsable(false)]
		public virtual IModuleDescriptor Descriptor { get; set; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			
		}

		public virtual IModuleInstance Clone()
		{
			IModuleInstance newInstance = (IModuleInstance) MemberwiseClone();
			newInstance.InstanceId = Guid.NewGuid();
			newInstance.ModuleData = ModuleData.Clone();
			return newInstance;
		}

		public bool Equals(IModuleInstance x, IModuleInstance y)
		{
			return x.InstanceId == y.InstanceId;
		}

		public int GetHashCode(IModuleInstance obj)
		{
			return obj.InstanceId.GetHashCode();
		}

		public bool Equals(IModuleInstance other)
		{
			return Equals(this, other);
		}

		public bool Equals(ModuleInstanceBase x, ModuleInstanceBase y)
		{
			return Equals(x as IModuleInstance, y as IModuleInstance);
		}

		public int GetHashCode(ModuleInstanceBase obj)
		{
			return GetHashCode(obj as IModuleInstance);
		}

		public bool Equals(ModuleInstanceBase other)
		{
			return Equals(other as IModuleInstance);
		}

		public override bool Equals(object obj)
		{
			if (obj is IModuleInstance) {
				return Equals(obj as IModuleInstance);
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return GetHashCode(this as IModuleInstance);
		}
	}
}