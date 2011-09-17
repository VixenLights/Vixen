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
		private ChannelNode[] _targetNodes;
		private TimeSpan _timeSpan;
		private object[] _parameterValues;

		protected EffectModuleInstanceBase() {
			TargetNodes = new ChannelNode[0];
			TimeSpan = TimeSpan.Zero;
			ParameterValues = new object[0];
			IsDirty = true;
		}

		//*** This reflects the change in references, but not change in the content of
		//    of the referenced objects.
		virtual public bool IsDirty { get; protected set; }

		virtual public ChannelNode[] TargetNodes {
			get { return _targetNodes; }
			set {
				if(value != _targetNodes) {
					_targetNodes = value;
					IsDirty = true;
				}
			}
		}

		virtual public TimeSpan TimeSpan {
			get { return _timeSpan; }
			set {
				if(value != _timeSpan) {
					_timeSpan = value;
					IsDirty = true;
				}
			}
		}

		virtual public object[] ParameterValues {
			get { return _parameterValues; }
			set {
				if(value != _parameterValues) {
					_parameterValues = value;
					IsDirty = true;
				}
			}
		}

		public void PreRender() {
			// System-side caching/dirty would use this hook.
			_PreRender();
			IsDirty = false;
		}

		public ChannelData Render() {
			// System-side caching/dirty would use this hook.
			if(IsDirty) {
				PreRender();
			}
			return _Render();
		}

		public ChannelData Render(TimeSpan restrictingOffsetTime, TimeSpan restrictingTimeSpan) {
			// System-side caching/dirty would use this hook.
			ChannelData channelData = Render();
			channelData = ChannelData.Restrict(channelData, restrictingOffsetTime, restrictingTimeSpan);
			return channelData;
		}

		abstract protected void _PreRender();

		abstract protected ChannelData _Render();

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
