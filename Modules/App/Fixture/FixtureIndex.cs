using System.Runtime.Serialization;
using Vixen.Commands;

namespace VixenModules.App.Fixture
{
	/// <summary>
	/// Maintains a fixture index (enumeration) entry.
	/// </summary>
    [DataContract]
	public class FixtureIndex : FixtureItem
	{
        #region Constructor 

		public FixtureIndex()
        {
			// Default to custom index type
			IndexType = FixtureIndexType.Custom;
		}

        #endregion

        #region Public Properties

		/// <summary>
		/// DMX start value of the entry.
		/// </summary>
		[DataMember]
		public int StartValue { get; set; }

		/// <summary>
		/// DMX end value of the entry.
		/// </summary>
		[DataMember]
		public int EndValue { get; set; }

		/// <summary>
		/// Indicates that this index entry selection needs to be represented by a curve.
		/// </summary>
		[DataMember]
		public bool UseCurve { get; set; }

		/// <summary>
		/// Type of index.  This property supports the preview.
		/// </summary>
		[DataMember]
		public FixtureIndexType IndexType { get; set; }

        #endregion

        #region Public Methods

		/// <summary>
		/// Creates a clone of the index entry.
		/// </summary>
		/// <returns>Clone of the index entry</returns>
        public FixtureIndex CreateInstanceForClone()
		{
			// Clone the index entry
			FixtureIndex indexEntry = new FixtureIndex
			{
				Name = Name,
				StartValue = StartValue,
				EndValue = EndValue,
				UseCurve = UseCurve,
				IndexType = IndexType,
			};

			return indexEntry;
		}

		#endregion
	}
}
