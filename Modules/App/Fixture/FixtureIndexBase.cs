using System.Runtime.Serialization;
using Vixen.Commands;

namespace VixenModules.App.Fixture
{
	/// <summary>
	/// Maintains a fixture index item.
	/// </summary>
	[DataContract]
	public abstract class FixtureIndexBase : FixtureItem
	{
		#region Constructor 

		/// <summary>
		/// Constructor
		/// </summary>
		public FixtureIndexBase()
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
		public virtual bool UseCurve { get; set; }

		/// <summary>
		/// Type of index.  This property supports the preview.
		/// </summary>
		[DataMember]
		public FixtureIndexType IndexType { get; set; }

		#endregion
	}
}
