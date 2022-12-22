﻿namespace Vixen.Module.Timing
{
	public abstract class TimingModuleInstanceBase : ModuleInstanceBase, ITimingModuleInstance,
	                                                 IEqualityComparer<ITimingModuleInstance>,
	                                                 IEquatable<ITimingModuleInstance>,
	                                                 IEqualityComparer<TimingModuleInstanceBase>,
	                                                 IEquatable<TimingModuleInstanceBase>
	{
		public abstract TimeSpan Position { get; set; }

		public abstract void Start();

		public abstract void Stop();

		public abstract void Pause();

		public abstract void Resume();

		public virtual bool SupportsVariableSpeeds
		{
			get { return false; }
		}

		public virtual float Speed
		{
			get { return 1; } // 1 = 100%
			set { throw new NotSupportedException(); }
		}

		public bool Equals(ITimingModuleInstance x, ITimingModuleInstance y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(ITimingModuleInstance obj)
		{
			return base.GetHashCode(obj);
		}

		public bool Equals(ITimingModuleInstance other)
		{
			return base.Equals(other);
		}

		public bool Equals(TimingModuleInstanceBase x, TimingModuleInstanceBase y)
		{
			return Equals(x as ITimingModuleInstance, y as ITimingModuleInstance);
		}

		public int GetHashCode(TimingModuleInstanceBase obj)
		{
			return GetHashCode(obj as ITimingModuleInstance);
		}

		public bool Equals(TimingModuleInstanceBase other)
		{
			return Equals(other as ITimingModuleInstance);
		}
	}
}