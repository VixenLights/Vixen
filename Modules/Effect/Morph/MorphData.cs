using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.NorthStar
{
	/// <summary>
	/// Maintains and serializes the settings of the Morph effect.
	/// </summary>
	[DataContract]
	[KnownType(typeof(MorphData))]
	class MorphData : EffectTypeModuleData
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public MorphData()
		{
			Orientation = StringOrientation.Horizontal;			
						
			Colors = new List<ColorGradient>
			{
				new ColorGradient(System.Drawing.Color.Blue),
				new ColorGradient(System.Drawing.Color.White),
				new ColorGradient(System.Drawing.Color.Purple)
			};
		}

		#endregion

		#region Public Properties

		[DataMember]
		public StringOrientation Orientation { get; set; }
										
		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		#endregion

		#region Protected Methods

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			MorphData result = new MorphData
			{
				Orientation = Orientation,					
			};
			
			return result;
		}

		#endregion
	}	
}
