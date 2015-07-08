using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;

namespace VixenModules.Effect.MultiAlternating {
	[DataContract]
	public class MultiAlternatingData : ModuleDataModelBase {

		[DataMember]
		public List<GradientLevelPair> Colors { get; set; }

		[DataMember]
		public int Interval { get; set; }

		[DataMember]
		public bool EnableStatic { get; set; }

		[DataMember]
		public int DepthOfEffect { get; set; }

		[DataMember]
		public int GroupEffect {
			get { return _groupEffect < 1 ? 1 : _groupEffect; }
			set { _groupEffect = value < 0 ? 1 : value; }
		}

		[DataMember]
		public int IntervalSkipCount { get; set; }

		private int _groupEffect = 1;

		public MultiAlternatingData()
		{
			Colors = new List<GradientLevelPair> {new GradientLevelPair(Color.Red), new GradientLevelPair(Color.Lime)};

			EnableStatic = true;
			Interval = 500;
			DepthOfEffect = 0;
			GroupEffect = 1;
			IntervalSkipCount = 1;
		}

		public override IModuleDataModel Clone() {
			var gradientLevelList = new List<GradientLevelPair>();
			gradientLevelList.AddRange(Colors.ToList());
			var result = new MultiAlternatingData
			{
				Colors = gradientLevelList,
				EnableStatic = EnableStatic,
				Interval = Interval,
				DepthOfEffect = DepthOfEffect,
				GroupEffect = GroupEffect,
				IntervalSkipCount = IntervalSkipCount
			};
			return result;
		}
	}
}