using System;
using Vixen.Execution;
using Vixen.Services;
using Vixen.Sys;
using VixenModules.App.SimpleSchedule.Service;

namespace VixenModules.App.SimpleSchedule.StateObject
{
	internal class ScheduledProgram : IScheduledItemStateObject
	{
		private IProgram _program;

		public ScheduledProgram(IScheduledItem item)
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

		public bool ItemIsValid
		{
			get { return _program != null; }
		}

		public void RequestContext()
		{
			if (_program != null) {
				Context = VixenSystem.Contexts.CreateProgramContext(new ContextFeatures(ContextCaching.ContextLevelCaching),
				                                                    _Program);
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

		private IProgram _Program
		{
			get { return _program ?? (_program = ApplicationServices.LoadProgram(OriginatingItem.ItemFilePath)); }
		}
	}
}