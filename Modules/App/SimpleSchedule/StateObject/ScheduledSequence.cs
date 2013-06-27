using System;
using Vixen.Execution;
using Vixen.Sys;
using VixenModules.App.SimpleSchedule.Service;

namespace VixenModules.App.SimpleSchedule.StateObject
{
	internal class ScheduledSequence : IScheduledItemStateObject
	{
		private ISequence _sequence;

		public ScheduledSequence(IScheduledItem item)
		{
			if (item == null) throw new ArgumentNullException("item");

			OriginatingItem = item;

			Start = ScheduledItemService.Instance.CalculateConcreteStartDateTime(item);
			End = ScheduledItemService.Instance.CalculateConcreteEndDateTime(item);
		}

		public Guid Id
		{
			get { return OriginatingItem.Id; }
		}

		public IScheduledItem OriginatingItem { get; private set; }

		public DateTime Start { get; private set; }

		public DateTime End { get; private set; }

		public void RequestContext()
		{
			if (_Sequence != null) {
				Context = VixenSystem.Contexts.CreateSequenceContext(new ContextFeatures(ContextCaching.ContextLevelCaching),
				                                                     _Sequence);
			}
		}

		public IContext Context { get; set; }

		public void ReleaseContext()
		{
			if (Context != null) {
				VixenSystem.Contexts.ReleaseContext(Context);
			}
		}

		#region Equality

		public bool Equals(IScheduledItemStateObject other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.Id.Equals(Id);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (!(obj is IScheduledItemStateObject)) return false;
			return Equals((IScheduledItemStateObject) obj);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		#endregion

		private ISequence _Sequence
		{
			get { return _sequence ?? (_sequence = Vixen.Services.SequenceService.Instance.Load(OriginatingItem.ItemFilePath)); }
		}
	}
}