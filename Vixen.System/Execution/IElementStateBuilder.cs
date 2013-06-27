using System;

namespace Vixen.Execution
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">Type of object contributing to a element's state.</typeparam>
	/// <typeparam name="U">Type of object representing the element's state.</typeparam>
	internal interface IElementStateBuilder<T, U>
	{
		void Clear();
		void AddElementState(Guid elementId, T state);
		U GetElementState(Guid elementId);
	}
}