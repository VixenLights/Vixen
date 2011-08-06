using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using CommandStandard;

namespace Vixen.Module.Effect {
	//This is where caching would take place, but the subclass can override/disable it
	//-> Compose, template it
	abstract public class EffectModuleInstanceBase : ModuleInstanceBase, IEffectModuleInstance, IEqualityComparer<IEffectModuleInstance>, IEquatable<IEffectModuleInstance>, IEqualityComparer<EffectModuleInstanceBase>, IEquatable<EffectModuleInstanceBase> {
		public void PreRender(ChannelNode[] nodes, long timeSpan, object[] parameterValues) {
			// This hook isn't used, but it's here for consistency and future use.
			_PreRender(nodes, timeSpan, parameterValues);
		}

		public ChannelData Render(ChannelNode[] nodes, long timeSpan, object[] parameterValues) {
			//caching/dirty uses this hook
			return _Render(nodes, timeSpan, parameterValues);
		}

		abstract protected void _PreRender(ChannelNode[] nodes, long timeSpan, object[] parameterValues);

		abstract protected ChannelData _Render(ChannelNode[] nodes, long timeSpan, object[] parameterValues);

		public string EffectName {
			get { return (Descriptor as IEffectModuleDescriptor).EffectName; }
		}

		public CommandParameterSpecification[] Parameters {
			get { return (Descriptor as IEffectModuleDescriptor).Parameters; }
		}

		public Guid[] PropertyDependencies {
			get { return (Descriptor as EffectModuleDescriptorBase).PropertyDependencies; }
		}

		public override string ToString() {
			return EffectName;
		}

		public bool Equals(IEffectModuleInstance x, IEffectModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IEffectModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(IEffectModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(EffectModuleInstanceBase x, EffectModuleInstanceBase y) {
			return Equals(x as IEffectModuleInstance, y as IEffectModuleInstance);
		}

		public int GetHashCode(EffectModuleInstanceBase obj) {
			return GetHashCode(obj as IEffectModuleInstance);
		}

		public bool Equals(EffectModuleInstanceBase other) {
			return Equals(other as IEffectModuleInstance);
		}
	}
}
