using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Fan
{
	/// <summary>	
	/// Maintains the fan effect data.
	/// </summary>	
	[DataContract]
	public class FanData: EffectTypeModuleData
	{
		#region Constructor

		public FanData()
		{
			CenterHandling = CenterOptions.Centered;
			IncrementAngle = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 1.0, 5.0 }));
			PanAngle = 50;
		}

		#endregion

		#region Public Properties
		
		[DataMember]
		public Curve IncrementAngle { get; set; }
		
		[DataMember]
		public int PanAngle { get; set; }

		[DataMember]
		public CenterOptions CenterHandling { get; set; }
		
		#endregion

		#region Protected Methods

		/// <summary>
		/// Creates a clone of the fan data.
		/// </summary>
		/// <returns>Clone of the fan data</returns>
		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			FanData result = new FanData
			{
				IncrementAngle = new Curve(IncrementAngle),
				PanAngle = PanAngle,
				CenterHandling = CenterHandling,

			};
			return result;
		}

		#endregion
	}
}
