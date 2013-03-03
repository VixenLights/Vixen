using System;
using System.Collections.Generic;
using Vixen.Execution;
using Vixen.IO;
using Vixen.Sys;

namespace Vixen.Module.SequenceType {
	abstract public class SequenceTypeModuleInstanceBase : ModuleInstanceBase, ISequenceTypeModuleInstance, IEqualityComparer<ISequenceTypeModuleInstance>, IEquatable<ISequenceTypeModuleInstance>, IEqualityComparer<SequenceTypeModuleInstanceBase>, IEquatable<SequenceTypeModuleInstanceBase> {
		public string FileExtension {
			get { return ((ISequenceTypeModuleDescriptor)Descriptor).FileExtension; }
		}

		public int ClassVersion {
			get { return ((ISequenceTypeModuleDescriptor)Descriptor).ClassVersion; }
		}

		abstract public ISequence CreateSequence();

		abstract public IContentMigrator CreateMigrator();
		
		abstract public ISequenceExecutor CreateExecutor();

		virtual public bool IsCustomSequenceLoader
		{
			get { return false; }
		}

		virtual public ISequence LoadSequenceFromFile(string filePath)
		{
			throw new NotImplementedException();
		}

		public override IModuleInstance Clone() {
			// Singleton
			throw new NotSupportedException();
		}

		public bool Equals(ISequenceTypeModuleInstance x, ISequenceTypeModuleInstance y) {
			return x.InstanceId == y.InstanceId;
		}

		public int GetHashCode(ISequenceTypeModuleInstance obj) {
			return obj.InstanceId.GetHashCode();
		}

		public bool Equals(ISequenceTypeModuleInstance other) {
			return Equals(this, other);
		}

		public bool Equals(SequenceTypeModuleInstanceBase x, SequenceTypeModuleInstanceBase y) {
			return Equals(x as ISequenceTypeModuleInstance, y as ISequenceTypeModuleInstance);
		}

		public int GetHashCode(SequenceTypeModuleInstanceBase obj) {
			return GetHashCode(obj as ISequenceTypeModuleInstance);
		}

		public bool Equals(SequenceTypeModuleInstanceBase other) {
			return Equals(other as ISequenceTypeModuleInstance);
		}
	}
}
