using System;
using System.Runtime.Serialization;
using Vixen.Data.Value;
using VixenModules.App.Curves;
using VixenModules.App.Fixture;
using ZedGraph;

namespace VixenModules.Effect.Fixture
{
	/// <summary>
	/// Serialized data for a fixture function.
	/// </summary>
	[DataContract]
	public class FixtureFunctionData
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public FixtureFunctionData()
		{
			FunctionIdentity = FunctionIdentity.Custom;
			FunctionName = String.Empty;
			Range = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates a clone of the fixture function.
		/// </summary>		
		public FixtureFunctionData CreateInstanceForClone()
		{
			// Make a clone of the fixture function data
			FixtureFunctionData clone = new FixtureFunctionData
			{
				FunctionName = FunctionName,
				FunctionType = FunctionType,
				FunctionIdentity = FunctionIdentity,
				Range = new Curve(Range),				
				IndexValue = IndexValue,
				ColorIndexValue = ColorIndexValue,				
			};

			return clone;
		}

		#endregion

		#region Public Properties
				
		/// <summary>
		/// Name of the fixture function.
		/// </summary>
		[DataMember]
		public string FunctionName { get; set; }

		/// <summary>
		/// Type of the fixture function.
		/// </summary>
		[DataMember]
		public FixtureFunctionType FunctionType { get; set; }

		/// <summary>
		/// Identity of the function.		
		/// </summary>
		[DataMember]
		public FunctionIdentity FunctionIdentity { get; set; }

		/// <summary>
		/// Range curve associated with the function.
		/// </summary>
		[DataMember]
		public Curve Range { get; set; }

		/// <summary>
		/// Selected index associated with the function.
		/// </summary>
		[DataMember]
		public string IndexValue { get; set; }

		/// <summary>
		/// Selected colorwheel index associated with the function.
		/// </summary>
		[DataMember]
		public string ColorIndexValue { get; set; }
				
		#endregion
	}
}
