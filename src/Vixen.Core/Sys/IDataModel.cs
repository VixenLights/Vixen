namespace Vixen.Sys
{
	/// <summary>
	/// Maintains a data model that can copy state from another data model of the same type.
	/// </summary>
	public interface IDataModel
	{
		/// <summary>
		/// Copies the contents of the source object into this object.
		/// </summary>
		/// <param name="source">Source to copy from</param>		
		void CopyInto(IDataModel source);
	}
}
