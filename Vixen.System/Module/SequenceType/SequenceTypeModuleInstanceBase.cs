using System;
using System.Collections.Generic;
using Vixen.Execution;
using Vixen.IO;
using Vixen.Sys;

namespace Vixen.Module.SequenceType {
	abstract public class SequenceTypeModuleInstanceBase : ModuleInstanceBase, ISequenceTypeModuleInstance, IEqualityComparer<ISequenceTypeModuleInstance>, IEquatable<ISequenceTypeModuleInstance>, IEqualityComparer<SequenceTypeModuleInstanceBase>, IEquatable<SequenceTypeModuleInstanceBase> {

		//protected SequenceTypeModuleInstanceBase() {
		//}

		//protected SequenceTypeModuleInstanceBase(SequenceTypeModuleInstanceBase original) {
		//    InstanceId = Guid.NewGuid();
		//    ModuleData = original.ModuleData.Clone();
		//    StaticModuleData = original.StaticModuleData;
		//    Descriptor = original.Descriptor;
		//}

		public string FileExtension {
			get { return ((ISequenceTypeModuleDescriptor)Descriptor).FileExtension; }
		}

		public int ClassVersion {
			get { return ((ISequenceTypeModuleDescriptor)Descriptor).ClassVersion; }
		}

		//public Type SequenceDataType {
		//    get { return ((ISequenceTypeModuleDescriptor)Descriptor).SequenceDataType; }
		//}

		abstract public ISequence CreateSequence();

		abstract public IMigrator CreateMigrator();
		
		abstract public ISequenceExecutor CreateExecutor();

		public override IModuleInstance Clone() {
			// Singleton
			throw new NotSupportedException();
		}

		//public Guid InstanceId { get; set; }

		//virtual public IModuleDataModel ModuleData { get; set; }

		//virtual public IModuleDataModel StaticModuleData { get; set; }

		//virtual public IModuleDescriptor Descriptor { get; set; }

		//virtual public void Dispose() { }

		//abstract public IModuleInstance Clone();

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
