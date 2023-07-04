namespace Vixen.Export.FPP
{
    /// <summary>
    /// Vixen Player Extended Data header.  This header adds additional data to the FSEQ files to determine
    /// when the file needs to be regenerated.  It also provides additional controller information to know
    /// how to pull apart the channel data.
    /// </summary>
    public class VixenPlayerVariableHeader : ExtendedDataVariableHeaderBase
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="getExtendedData">Delegate to retrieve extended data buffer</param>
        public VixenPlayerVariableHeader(Func<byte[]> getExtendedData)
	    {
		    _getExtendedData = getExtendedData;
	    }

        #endregion

        #region Fields

        /// <summary>
        /// Delegate to method that returns extended data.
        /// </summary>
        private Func<byte[]> _getExtendedData;

		#endregion

		#region Public Properties

		/// <inheritdoc />
		public uint LengthOfHeader { get; set; } = 18;

		#endregion

		#region Public Methods

		/// <inheritdoc />
		public override int HeaderLength
        {
            get
            {
                return 18;
            }
        }

		/// <inheritdoc />
		public override byte[] GetHeaderBytes()
        {
            var bytes = new byte[LengthOfHeader];
            bytes[0] = (byte)LengthOfHeader;
            bytes[1] = 00;
            bytes[2] = (byte)'E';
            bytes[3] = (byte)'D';
            bytes[4] = (byte)'V';
            bytes[5] = (byte)'P';
            bytes[6] = (byte)(OffsetToVariableHeader & 0xff);
            bytes[7] = (byte)(OffsetToVariableHeader >> 08 & 0xff);
            bytes[8] = (byte)(OffsetToVariableHeader >> 16 & 0xff);
            bytes[9] = (byte)(OffsetToVariableHeader >> 24 & 0xff);
            bytes[10] = (byte)(OffsetToVariableHeader >> 32 & 0xff);
            bytes[11] = (byte)(OffsetToVariableHeader >> 40 & 0xff);
            bytes[12] = (byte)(OffsetToVariableHeader >> 48 & 0xff);
            bytes[13] = (byte)(OffsetToVariableHeader >> 56 & 0xff);

            // Initialize the length of the extended data header
            LengthOfHeader = (uint)GetExtendedData().Length;

            bytes[14] = (byte)(LengthOfHeader & 0xff);
            bytes[15] = (byte)(LengthOfHeader >> 08 & 0xff);
            bytes[16] = (byte)(LengthOfHeader >> 16 & 0xff);
            bytes[17] = (byte)(LengthOfHeader >> 24 & 0xff);

            return bytes;
        }

        /// <inheritdoc />
		public override byte[] GetExtendedData()
        {
	        return _getExtendedData();
        }

        #endregion
    }
}
