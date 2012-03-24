using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Vixen.Sys;

namespace Vixen.Module.Effect {
	abstract public class EffectModuleInstanceBase : ModuleInstanceBase, IEffectModuleInstance, IEqualityComparer<IEffectModuleInstance>, IEquatable<IEffectModuleInstance>, IEqualityComparer<EffectModuleInstanceBase>, IEquatable<EffectModuleInstanceBase> {
		private ChannelNode[] _targetNodes;
		private TimeSpan _timeSpan;
		//private PropertyInfo[] _parameterValues;
		private DefaultValueArrayMember _parameterValues;
		private ChannelIntents _channelIntents;

		protected EffectModuleInstanceBase() {
			TargetNodes = new ChannelNode[0];
			TimeSpan = TimeSpan.Zero;
			IsDirty = true;
			//*** Going with automatic value handling now!  Super cool! ***
			//ParameterValues = new object[0];
			//_parameterValues = ValueAttribute.GetValueMembers(this);
			_parameterValues = new DefaultValueArrayMember(this);
			SubordinateEffects = new List<SubordinateEffect>();
			_channelIntents = new ChannelIntents();
		}

		//*** consider subordinates
		virtual public bool IsDirty { get; protected set; }

		//*** update subordinates
		virtual public ChannelNode[] TargetNodes {
			get { return _targetNodes; }
			set {
				if(value != _targetNodes) {
					_targetNodes = value;
					IsDirty = true;
				}
			}
		}

		//*** update subordinates
		virtual public TimeSpan TimeSpan {
			get { return _timeSpan; }
			set {
				if(value != _timeSpan) {
					_timeSpan = value;
					IsDirty = true;
				}
			}
		}

		//virtual public object[] ParameterValues { get; set; }
		//virtual public object[] ParameterValues { 
		//    get { return _parameterValues.Select(x => x.GetValue(this, null)).ToArray(); }
		//    set {
		//        if(value.Length != _parameterValues.Length) throw new InvalidOperationException("Invalid number of values.  Expected " + _valueProperties.Length + ".");
		//        for(int i = 0; i < value.Length; i++) {
		//            _parameterValues[i].SetValue(this, value[i], null);
		//        }
		//    }
		//}
		public virtual object[] ParameterValues {
			get { return _parameterValues.Values; }
			set { _parameterValues.Values = value; }
		}

		public void PreRender() {
			// System-side caching/dirty would use this hook.
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

		public List<SubordinateEffect> SubordinateEffects { get; private set; }

		virtual public ChannelIntents GetChannelIntents(TimeSpan effectRelativeTime) {
			_channelIntents.Clear();

			_AddLocalIntents(effectRelativeTime);
			_AddSubordinateIntents(effectRelativeTime);
	
			return _channelIntents;
		}

		private void _AddLocalIntents(TimeSpan effectRelativeTime) {
			EffectIntents effectIntents = Render();
			foreach(Guid channelId in effectIntents.ChannelIds) {
				IntentNode channelIntent = _GetChannelIntent(effectIntents, channelId, effectRelativeTime);
				_channelIntents.AddIntentNodeToChannel(channelId, channelIntent, null);
			}
		}

		private void _AddSubordinateIntents(TimeSpan effectRelativeTime) {
			foreach(SubordinateEffect subordinateEffect in SubordinateEffects) {
				ChannelIntents subordinateChannelIntents = subordinateEffect.Effect.GetChannelIntents(effectRelativeTime);
				_channelIntents.AddIntentNodesToChannels(subordinateChannelIntents, subordinateEffect.CombinationOperation);
			}
		}

		private IntentNode _GetChannelIntent(EffectIntents effectIntents, Guid channelId, TimeSpan effectRelativeTime) {
			List<IntentNode> channelIntents;
			if(effectIntents.TryGetValue(channelId, out channelIntents)) {
				return channelIntents.FirstOrDefault(x => effectRelativeTime >= x.StartTime && effectRelativeTime <= x.EndTime);
			}
			return null;
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
