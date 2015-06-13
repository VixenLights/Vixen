using System;

namespace VixenModules.Editor.EffectEditor.PropertyEditing
{
	public class CollectionChangedEventArgs: EventArgs
	{
		public CollectionChangedEventArgs(int index)
		{
			Index = index;
		}

		public int Index { get; private set; }
	}
}
