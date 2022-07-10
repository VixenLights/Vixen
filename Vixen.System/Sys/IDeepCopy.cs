namespace Vixen.Sys
{
	/// <summary>
	/// Makes a deep copy of the specified source object.
	/// </summary>
	public interface IDeepCopy
	{
		/// <summary>
		/// Copies the contents of the source object into this object.
		/// </summary>
		/// <param name="source">Source to copy from</param>
		void Copy(object source);
	}
}
