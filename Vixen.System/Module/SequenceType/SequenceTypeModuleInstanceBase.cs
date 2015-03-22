using System;
using System.Collections.Generic;
using Vixen.Cache.Sequence;
using Vixen.Execution;
using Vixen.IO;
using Vixen.Sys;

namespace Vixen.Module.SequenceType
{
	public abstract class SequenceTypeModuleInstanceBase : ModuleInstanceBase, ISequenceTypeModuleInstance,
	                                                       IEqualityComparer<ISequenceTypeModuleInstance>,
	                                                       IEquatable<ISequenceTypeModuleInstance>,
	                                                       IEqualityComparer<SequenceTypeModuleInstanceBase>,
	                                                       IEquatable<SequenceTypeModuleInstanceBase>
	{
		public string FileExtension
		{
			get { return ((ISequenceTypeModuleDescriptor) Descriptor).FileExtension; }
		}

		public int ObjectVersion
		{
			get { return ((ISequenceTypeModuleDescriptor) Descriptor).ObjectVersion; }
		}

		public abstract ISequence CreateSequence();

		public abstract ISequenceCache CreateSequenceCache();

		public abstract IContentMigrator CreateMigrator();

		public abstract ISequenceExecutor CreateExecutor();

		public virtual bool IsCustomSequenceLoader
		{
			get { return false; }
		}

		public virtual ISequence LoadSequenceFromFile(string filePath)
		{
			throw new NotImplementedException();
		}

		public override IModuleInstance Clone()
		{
			// Singleton
			throw new NotSupportedException();
		}

		public bool Equals(ISequenceTypeModuleInstance x, ISequenceTypeModuleInstance y)
		{
			return x.InstanceId == y.InstanceId;
		}

		public int GetHashCode(ISequenceTypeModuleInstance obj)
		{
			return obj.InstanceId.GetHashCode();
		}

		public bool Equals(ISequenceTypeModuleInstance other)
		{
			return Equals(this, other);
		}

		public bool Equals(SequenceTypeModuleInstanceBase x, SequenceTypeModuleInstanceBase y)
		{
			return Equals(x as ISequenceTypeModuleInstance, y as ISequenceTypeModuleInstance);
		}

		public int GetHashCode(SequenceTypeModuleInstanceBase obj)
		{
			return GetHashCode(obj as ISequenceTypeModuleInstance);
		}

		public bool Equals(SequenceTypeModuleInstanceBase other)
		{
			return Equals(other as ISequenceTypeModuleInstance);
		}
	}
}