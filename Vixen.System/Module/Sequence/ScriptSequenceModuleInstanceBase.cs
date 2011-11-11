using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Sequence {
	abstract public class ScriptSequenceModuleInstanceBase : ScriptSequence, ISequenceModuleInstance, IEqualityComparer<ISequenceModuleInstance>, IEquatable<ISequenceModuleInstance>, IEqualityComparer<ScriptSequenceModuleInstanceBase>, IEquatable<ScriptSequenceModuleInstanceBase> {
		protected ScriptSequenceModuleInstanceBase(string language)
			: base(language) {
		}

		protected ScriptSequenceModuleInstanceBase(string language, ScriptSequenceModuleInstanceBase original)
			: base(language, original) {
			InstanceId = Guid.NewGuid();
			ModuleData = original.ModuleData.Clone();
			StaticModuleData = original.StaticModuleData;
			Descriptor = original.Descriptor;
		}

		public string FileExtension {
			get { return (Descriptor as ISequenceModuleDescriptor).FileExtension; }
		}

		public Guid InstanceId { get; set; }

		virtual public IModuleDataModel ModuleData { get; set; }

		virtual public IModuleDataModel StaticModuleData { get; set; }

		virtual public IModuleDescriptor Descriptor { get; set; }

		virtual public void Dispose() { }

		abstract public IModuleInstance Clone();

		public bool Equals(ISequenceModuleInstance x, ISequenceModuleInstance y) {
			return x.InstanceId == y.InstanceId;
		}

		public int GetHashCode(ISequenceModuleInstance obj) {
			return obj.InstanceId.GetHashCode();
		}

		public bool Equals(ISequenceModuleInstance other) {
			return Equals(this, other);
		}

		public bool Equals(ScriptSequenceModuleInstanceBase x, ScriptSequenceModuleInstanceBase y) {
			return Equals(x as ISequenceModuleInstance, y as ISequenceModuleInstance);
		}

		public int GetHashCode(ScriptSequenceModuleInstanceBase obj) {
			return GetHashCode(obj as ISequenceModuleInstance);
		}

		public bool Equals(ScriptSequenceModuleInstanceBase other) {
			return Equals(other as ISequenceModuleInstance);
		}
	}
}
