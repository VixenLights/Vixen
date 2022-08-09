using System.Collections.Generic;
using System.Runtime.Serialization;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Fixture
{
	/// <summary>
	/// Maintains the fixture effect data.
	/// </summary>
	[DataContract]
	public class FixtureData : EffectTypeModuleData
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public FixtureData()
		{			
			// Create the collection of fixture function data
			FunctionData = new List<FixtureFunctionData>();								
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Maintains a collection fixture function data associated with the effect.
		/// </summary>
		[DataMember]
		public List<FixtureFunctionData> FunctionData { get; set; }

		/// <summary>
		/// Collection of fixture functions from the fixture specification.
		/// </summary>
		[DataMember]
		public List<App.Fixture.FixtureFunction> FixtureFunctions { get; set; }

		#endregion

		#region Protected Methods

		/// <summary>
		/// Makes a clone of the fixture data.
		/// </summary>
		/// <returns>Clone of the fixture data</returns>
		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			// Create a fixture data instance
			FixtureData clone = new FixtureData();
						
			// Loop over the fixture function data items
			for (int index = 0; index < FunctionData.Count; index++)
			{
				// Make a clone of the specified fixture function
				clone.FunctionData[index] = FunctionData[index].CreateInstanceForClone();
			}

			return clone;
		}

		#endregion
	}
}