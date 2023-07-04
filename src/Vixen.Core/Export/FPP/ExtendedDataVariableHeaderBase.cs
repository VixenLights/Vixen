namespace Vixen.Export.FPP
{
	/// <summary>
	/// Maintains an Extended Data variable header.
	/// </summary>
	public abstract class ExtendedDataVariableHeaderBase : VariableHeaderBase
    {
		#region Public Methods

		/// <summary>
		/// Gets the extended data associated with the header.
		/// </summary>
		/// <returns>Extended data associated with the header</returns>
		public abstract byte[] GetExtendedData();

		/// <summary>
		/// Offset from the beginning of the FSEQ file to the extended data.
		/// </summary>
	    public ulong OffsetToVariableHeader { get; set; }

		#endregion
	}
}
