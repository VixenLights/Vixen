using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using CommandStandard;

namespace Vixen.Module.EffectEditor {
	abstract public class EffectEditorModuleInstanceBase : ModuleInstanceBase, IEffectEditorModuleInstance, IEqualityComparer<IEffectEditorModuleInstance> {
		abstract public IEffectEditorControl CreateEditorControl();

		public Guid EffectTypeId {
			get { return (Descriptor as IEffectEditorModuleDescriptor).EffectTypeId; }
		}

		public CommandParameterSpecification[] CommandSignature {
			get { return (Descriptor as IEffectEditorModuleDescriptor).CommandSignature; }
		}

		public bool Equals(IEffectEditorModuleInstance x, IEffectEditorModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IEffectEditorModuleInstance obj) {
			return base.GetHashCode(obj);
		}
	}
}
