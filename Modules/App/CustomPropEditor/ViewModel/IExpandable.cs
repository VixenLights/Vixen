using System;
using System.Collections.Generic;

namespace VixenModules.App.CustomPropEditor.ViewModel
{
	/// <summary>
	/// Interface for items that can be expanded
	/// </summary>
	public interface IExpandable
	{
		bool IsExpanded { get; set; }

		IEnumerable<Guid> GetParentIds();

		IEnumerable<Guid> GetChildrenIds();

	}
}
