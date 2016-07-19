using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vixen.Sys;

namespace Vixen.Execution
{
	//T = type of state being added to a element
	//U = type of the state retrieved for a element
	//Could be a multiple command (T) instances going in and a single command (U) coming out
	//Could be multiple intent (T) instances going in and multiple intents (U) coming out
	internal class IntentStateBuilder : IElementStateBuilder<IIntentState, IIntentStates>
	{
		private readonly Dictionary<Guid, IIntentStates> _elementStates;

		public IntentStateBuilder()
		{
			_elementStates = new Dictionary<Guid, IIntentStates>(VixenSystem.Elements.Count());//4 * Environment.ProcessorCount, VixenSystem.Elements.Count());
		}

		public void Clear()
		{
			lock (_elementStates)
			{
				foreach (var value in _elementStates.Values)
				{
					value.Clear();
				}
			}	
		}

		public void AddElementState(Guid elementId, IIntentState state)
		{
			IIntentStates elementIntentList = _GetElementIntentList(elementId);
			lock (elementIntentList)
			{
				elementIntentList.AddIntentState(state);
			}
		}

		public IIntentStates GetElementState(Guid elementId)
		{
			return _GetElementIntentList(elementId);
		}

		private IIntentStates _GetElementIntentList(Guid elementId)
		{
			IIntentStates elementIntentList;
			lock (_elementStates)
			{
				if (!_elementStates.TryGetValue(elementId, out elementIntentList))
				{
					elementIntentList = new IntentStateList(4);
					_elementStates[elementId] = elementIntentList;
				}
			}
			
			return elementIntentList;
		}
	}
}