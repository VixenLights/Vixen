using System;
using System.Collections.Generic;
using Vixen.Sys;

namespace Vixen.Module.SequenceFilter
{
	public abstract class SequenceFilterModuleInstanceBase : ModuleInstanceBase, ISequenceFilterModuleInstance,
	                                                         IEqualityComparer<ISequenceFilterModuleInstance>,
	                                                         IEquatable<ISequenceFilterModuleInstance>,
	                                                         IEqualityComparer<SequenceFilterModuleInstanceBase>,
	                                                         IEquatable<SequenceFilterModuleInstanceBase>
	{
		private IElementNode[] _targetNodes;
		private TimeSpan _timeSpan;

		public TimeSpan TimeSpan
		{
			get { return _timeSpan; }
			set
			{
				if (value != _timeSpan) {
					_timeSpan = value;
					IsDirty = true;
				}
			}
		}

		public IElementNode[] TargetNodes
		{
			get { return _targetNodes; }
			set
			{
				if (value != _targetNodes) {
					_targetNodes = value;
					IsDirty = true;
				}
			}
		}

		public virtual bool HasSetup
		{
			get { return false; }
		}

		public virtual bool Setup()
		{
			return false;
		}

		public bool IsDirty { get; protected set; }

		public abstract void AffectIntent(IIntentSegment intentSegment, TimeSpan filterRelativeStartTime,
		                                  TimeSpan filterRelativeEndTime);

		public bool Equals(ISequenceFilterModuleInstance x, ISequenceFilterModuleInstance y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(ISequenceFilterModuleInstance obj)
		{
			return base.GetHashCode(obj);
		}

		public bool Equals(ISequenceFilterModuleInstance other)
		{
			return base.Equals(other);
		}

		public bool Equals(SequenceFilterModuleInstanceBase x, SequenceFilterModuleInstanceBase y)
		{
			return Equals(x as ISequenceFilterModuleInstance, y as ISequenceFilterModuleInstance);
		}

		public int GetHashCode(SequenceFilterModuleInstanceBase obj)
		{
			return GetHashCode(obj as ISequenceFilterModuleInstance);
		}

		public bool Equals(SequenceFilterModuleInstanceBase other)
		{
			return Equals(other as ISequenceFilterModuleInstance);
		}
	}
}