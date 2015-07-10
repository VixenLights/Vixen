using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;

namespace VixenModules.Effect.Alternating {
	[DataContract]
	public class AlternatingData : ModuleDataModelBase {

		[DataMember]
		public List<GradientLevelPair> Colors { get; set; }

		[DataMember]
		public int Interval { get; set; }

		[DataMember]
		public bool EnableStatic { get; set; }

		[DataMember]
		public int DepthOfEffect { get; set; }

		[DataMember]
		public int IntervalSkipCount { get; set; }

		private int _groupEffect = 1;

		public AlternatingData()
		{
			Colors = new List<GradientLevelPair> {new GradientLevelPair(Color.Red, CurveType.Flat100), new GradientLevelPair(Color.Lime, CurveType.Flat100)};

			EnableStatic = false;
			Interval = 500;
			DepthOfEffect = 0;
			IntervalSkipCount = 1;
		}

		public override IModuleDataModel Clone() {
			var gradientLevelList = new List<GradientLevelPair>();
			gradientLevelList.AddRange(Colors.ToList());
			var result = new AlternatingData
			{
				Colors = gradientLevelList,
				EnableStatic = EnableStatic,
				Interval = Interval,
				DepthOfEffect = DepthOfEffect,
				IntervalSkipCount = IntervalSkipCount
			};
			return result;
		}
	}
}