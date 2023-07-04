namespace Vixen.Export.FPP
{
    /// <summary>
    /// Maintains common properties and method of an FSEQ variable header.
    /// </summary>
    public abstract class VariableHeaderBase
    {
        #region Public Methods

        /// <summary>
        /// Gets the variable header bytes.
        /// </summary>
        /// <returns>Variable header bytes</returns>
        public abstract byte[] GetHeaderBytes();

        #endregion

        #region Public Properties

        /// <summary>
        /// Length of the header.
        /// </summary>
        public abstract int HeaderLength { get; }

        #endregion
    }
}
