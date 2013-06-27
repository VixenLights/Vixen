using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Timing;

namespace Vixen.Module.RuntimeBehavior
{
	public abstract class RuntimeBehaviorModuleInstanceBase : ModuleInstanceBase, IRuntimeBehaviorModuleInstance,
	                                                          IEqualityComparer<IRuntimeBehaviorModuleInstance>,
	                                                          IEquatable<IRuntimeBehaviorModuleInstance>,
	                                                          IEqualityComparer<RuntimeBehaviorModuleInstanceBase>,
	                                                          IEquatable<RuntimeBehaviorModuleInstanceBase>
	{
		public abstract void Startup(ISequence sequence);

		public abstract void Shutdown();

		public abstract void Handle(IEffectNode effectNode);

		public virtual bool Enabled { get; set; }

		public abstract Tuple<string, Action>[] BehaviorActions { get; }

		public bool Equals(IRuntimeBehaviorModuleInstance x, IRuntimeBehaviorModuleInstance y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(IRuntimeBehaviorModuleInstance obj)
		{
			return base.GetHashCode(obj);
		}

		public bool Equals(IRuntimeBehaviorModuleInstance other)
		{
			return base.Equals(other);
		}

		public bool Equals(RuntimeBehaviorModuleInstanceBase x, RuntimeBehaviorModuleInstanceBase y)
		{
			return Equals(x as IRuntimeBehaviorModuleInstance, y as IRuntimeBehaviorModuleInstance);
		}

		public int GetHashCode(RuntimeBehaviorModuleInstanceBase obj)
		{
			return GetHashCode(obj as IRuntimeBehaviorModuleInstance);
		}

		public bool Equals(RuntimeBehaviorModuleInstanceBase other)
		{
			return Equals(other as IRuntimeBehaviorModuleInstance);
		}
	}
}