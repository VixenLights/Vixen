using System;
using System.Collections.Generic;

namespace VixenModules.Property.Position
{
	// Already marked as [Serializable] since it's a dictionary, so [DataContract] is not needed.
	public class PositionMap : Dictionary<Guid, PositionValue>
	{
		public void AddRange(PositionMap values)
		{
			foreach (var kvp in values) {
				this[kvp.Key] = kvp.Value;
			}
		}
	}
}