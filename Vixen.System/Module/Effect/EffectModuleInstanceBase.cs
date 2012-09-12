using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Vixen.Services;
using Vixen.Sys;

namespace Vixen.Module.Effect {
	[Serializable]
	abstract public class EffectModuleInstanceBase : ModuleInstanceBase, IEffectModuleInstance, IEqualityComparer<IEffectModuleInstance>, IEquatable<IEffectModuleInstance>, IEqualityComparer<EffectModuleInstanceBase>, IEquatable<EffectModuleInstanceBase> {
		private ChannelNode[] _targetNodes;
		private TimeSpan _timeSpan;
		private DefaultValueArrayMember _parameterValues;
		private ChannelIntents _channelIntents;

		protected EffectModuleInstanceBase() {
			TargetNodes = new ChannelNode[0];
			TimeSpan = TimeSpan.Zero;
			IsDirty = true;
			_parameterValues = new DefaultValueArrayMember(this);
			_channelIntents = new ChannelIntents();
		}

		virtual public bool IsDirty { get; protected set; }

		public ChannelNode[] TargetNodes {
			get { return _targetNodes; }
			set {
				if(value != _targetNodes) {
					_targetNodes = value;
					_EnsureTargetNodeProperties();
					IsDirty = true;
				}
			}
		}

		public TimeSpan TimeSpan {
			get { return _timeSpan; }
			set {
				if(value != _timeSpan) {
					_timeSpan = value;
					IsDirty = true;
				}
			}
		}

		public object[] ParameterValues {
			get { return _parameterValues.Values; }
			set { 
				_parameterValues.Values = value;
				IsDirty = true;
			}
		}

		public void PreRender() {
			_PreRender();
			IsDirty = false;
		}

		public EffectIntents Render() {
			if(IsDirty) {
				PreRender();
			}
			return _Render();
		}

		public EffectIntents Render(TimeSpan restrictingOffsetTime, TimeSpan restrictingTimeSpan) {
			EffectIntents effectIntents = Render();
			// NB: the ChannelData.Restrict method takes a start and end time, not a start and duration
			effectIntents = EffectIntents.Restrict(effectIntents, restrictingOffsetTime, restrictingOffsetTime + restrictingTimeSpan);
			return effectIntents;
		}

		abstract protected void _PreRender();

		abstract protected EffectIntents _Render();

		public string EffectName {
			get { return ((IEffectModuleDescriptor)Descriptor).EffectName; }
		}

		public ParameterSignature Parameters {
			get { return ((IEffectModuleDescriptor)Descriptor).Parameters; }
		}

		public Guid[] PropertyDependencies {
			get { return ((EffectModuleDescriptorBase)Descriptor).PropertyDependencies; }
		}

		public virtual void GenerateVisualRepresentation(Graphics g, Rectangle clipRectangle) {
			g.Clear(Color.White);
			g.DrawRectangle(Pens.Black, clipRectangle.X, clipRectangle.Y, clipRectangle.Width - 1, clipRectangle.Height - 1);
		}

		public ChannelIntents GetChannelIntents(TimeSpan effectRelativeTime) {
			_channelIntents.Clear();

			_AddLocalIntents(effectRelativeTime);

			return _channelIntents;
		}

		private void _AddLocalIntents(TimeSpan effectRelativeTime) {
			EffectIntents effectIntents = Render();
			foreach(Guid channelId in effectIntents.ChannelIds) {
				IIntentNode[] channelIntents = effectIntents.GetChannelIntentsAtTime(channelId, effectRelativeTime);
				_channelIntents.AddIntentNodeToChannel(channelId, channelIntents);
			}
		}

		private void _EnsureTargetNodeProperties() {
			// If the effect requires any properties, make sure the target nodes have those properties.
			if(TargetNodes == null || TargetNodes.Length == 0) return;

			if(!ApplicationServices.AreAllEffectRequiredPropertiesPresent(this)) {
				EffectModuleDescriptorBase effectDescriptor = Modules.GetDescriptorById<EffectModuleDescriptorBase>(Descriptor.TypeId);

				List<string> message = new List<string> {
						"The \"" + effectDescriptor.TypeName + "\" effect has property requirements that are missing:", 
						""};
				foreach(ChannelNode channelNode in TargetNodes) {
					Guid[] missingPropertyIds = effectDescriptor.PropertyDependencies.Except(channelNode.Properties.Select(x => x.Descriptor.TypeId)).ToArray();
					if(missingPropertyIds.Length > 0) {
						message.Add((channelNode.Children.Any() ? "Group " : "Channel ") + channelNode.Name);
						message.AddRange(missingPropertyIds.Select(x => " - Property " + Modules.GetDescriptorById(x).TypeName));
					}
				}
				throw new InvalidOperationException(string.Join(Environment.NewLine, message));
			}
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
