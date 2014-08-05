using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;

namespace VixenModules.Effect.Alternating {
	[DataContract]
	internal class AlternatingData : ModuleDataModelBase {

		[DataMember]
		public RGB Color1 { get; set; }

		[DataMember]
		public RGB Color2 { get; set; }


		[DataMember]
		public double Level1 { get; set; }

		[DataMember]
		public double Level2 { get; set; }


		[DataMember]
		public int Interval { get; set; }

		[DataMember]
		public bool Enable { get; set; }

		[DataMember]
		public int DepthOfEffect { get; set; }

		[DataMember]
		public int GroupEffect {
			get { return groupEffect < 1 ? 1 : groupEffect; }
			set { groupEffect = value < 0 ? 1 : value; }
		}

		private int groupEffect = 1;

		[DataMember]
		public bool StaticColor1 { get; set; }

		[DataMember]
		public bool StaticColor2 { get; set; }

		[DataMember]
		public ColorGradient ColorGradient1 { get; set; }

		[DataMember]
		public ColorGradient ColorGradient2 { get; set; }

		Curve curve1 = null;
		Curve curve2 = null;
		[DataMember]
		public Curve Curve1 {
			get {
				if (curve1 == null)
					curve1 = new Curve();
				return curve1;
			}
			set { curve1 = value; }
		}
		[DataMember]
		public Curve Curve2 {
			get {

				if (curve2 == null)
					curve2 = new Curve();
				return curve2;
			}
			set { curve2 = value; }
		}

		public AlternatingData() {
			Level1 = 1;
			Level2 = 1;
			Color1 = Color.White;
			Color2 = Color.Red;
			Enable = true;
			Interval = 500;
			DepthOfEffect = 0;
			GroupEffect = 1;
			StaticColor1 = StaticColor2 = true;
			ColorGradient1 = new ColorGradient(Color.White);
			ColorGradient2 = new ColorGradient(Color.Red);
			Curve1 = new Curve();
			Curve2 = new Curve();
		}

		public override IModuleDataModel Clone() {
			AlternatingData result = new AlternatingData();
			result.Level1 = Level1;
			result.Level2 = Level2;
			result.Color1 = Color1;
			result.Color2 = Color2;
			result.Enable = Enable;
			result.Interval = Interval;
			result.DepthOfEffect = DepthOfEffect;
			result.GroupEffect = GroupEffect;
			result.StaticColor2 = StaticColor2;
			result.StaticColor1 = StaticColor1;
			result.Curve2 = Curve2;
			result.Curve1 = Curve1;
			result.ColorGradient1 = new ColorGradient(ColorGradient1);
			result.ColorGradient2 = new ColorGradient(ColorGradient2);
		
			return result;
		}
	}
}