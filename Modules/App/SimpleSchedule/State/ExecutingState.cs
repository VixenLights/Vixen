using System;
using System.Collections.Generic;
using Common.StateMach;
using VixenModules.App.SimpleSchedule.Transition;

namespace VixenModules.App.SimpleSchedule.State
{
	internal class ExecutingState : IState<IScheduledItemStateObject>
	{
		private ITransition<IScheduledItemStateObject>[] _transitions;

		public ExecutingState()
		{
			_transitions = new ITransition<IScheduledItemStateObject>[]
			               	{
			               		new ExecutingToPostExecute()
			               	};
		}

		public string Name
		{
			get { return "Executing"; }
		}

		public IEnumerable<ITransition<IScheduledItemStateObject>> Transitions
		{
			get { return _transitions; }
		}

		public void Entering(IScheduledItemStateObject obj)
		{
			obj.Context.Start();
		}

		public void Leaving(IScheduledItemStateObject obj)
		{
			// In case it's forced out of this state before it finishes executing.
			obj.Context.Stop();
		}
	}
}