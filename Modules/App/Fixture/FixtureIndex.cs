using System.Runtime.Serialization;
using Vixen.Commands;

namespace VixenModules.App.Fixture
{
	/// <summary>
	/// Maintains a fixture index (enumeration) entry.
	/// </summary>
    [DataContract]
	public class FixtureIndex : FixtureIndexBase
	{        
        #region Public Properties
				
		/// <summary>
		/// Images associated with index.
		/// </summary>
		[DataMember]
		public string Image { get; set; }

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
				Image = Image,
			};

			return indexEntry;
		}

		#endregion
	}
}
