namespace Vixen.Commands
{
    /// <summary>
    /// Maintains a named 8 bit command.
    /// </summary>
    /// <typeparam name="T">Type of index command</typeparam>
    public class Named8BitCommand<T> : _8BitCommand
		where T : System.Enum
	{
		#region Constructors 

		public Named8BitCommand(byte value) :
			base(value)
		{
		}

		public Named8BitCommand(short value) : 
			this((byte)value)
		{
		}

		public Named8BitCommand(int value) : 
			this((byte)value)
		{
		}

		public Named8BitCommand(long value)
			: this((byte)value)
		{
		}

		public Named8BitCommand(float value)
			: this((byte)value)
		{
		}

		public Named8BitCommand(double value)
			: this((byte)value)
		{
		}

        #endregion

        #region Public Properties

		/// <summary>
		/// Type of index command value.  This property allows the system to recognize certain commands.
		/// </summary>
        public T IndexType { get; set; }

		/// <summary>
		/// Tag associated with the command.
		/// </summary>		
		public string Tag { get; set; }

		/// <summary>
		/// Provides a customizable descriptive string field that labels or describes the intent of the command.
		/// </summary>		
		public string Label { get; set; }

		/// <summary>
		/// Minimum value of the index type range.
		/// </summary>
		/// <remarks>This property only applies if the index type is part of a range</remarks>
		public byte RangeMinimum { get; set; }

		/// <summary>
		/// Maximum value of the index type range.
		/// </summary>
		/// <remarks>This property only applies if the index type is part of a range</remarks>
		public byte RangeMaximum { get; set; }

		#endregion
	}
}
