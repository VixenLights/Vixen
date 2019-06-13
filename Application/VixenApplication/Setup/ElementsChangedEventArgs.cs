using System;

namespace VixenApplication.Setup
{
	public class ElementsChangedEventArgs: EventArgs
	{
		public ElementsChangedEventArgs(ElementsChangedAction action)
		{
			Action = action;
		}

		public ElementsChangedAction Action { get; }

		public enum ElementsChangedAction
		{
			Add,
			Edit,
			Remove,
			Rename
		}
	}

	
}
