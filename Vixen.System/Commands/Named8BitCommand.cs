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
		/// Type of index command value.  This property allows the preview to recognize commands.
		/// </summary>
        public T IndexType { get; set; }

		/// <summary>
		/// Tag associated with the command.
		/// </summary>
		/// <remarks>For intelligent fixtures this is the function name.</remarks>
		public string Tag { get; set; }

		/// <summary>
		/// Preview representation of the type.
		/// </summary>
		/// <remarks>For intelligent fixtures this is helps form a string to indicate what functions are active.</remarks>
		public string Preview { get; set; }

		#endregion
	}
}
