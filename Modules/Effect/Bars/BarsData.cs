using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Vixen.Module;
using VixenModules.App.ColorGradients;

namespace VixenModules.Effect.Bars
{
	[DataContract]
	public class BarsData: ModuleDataModelBase
	{

		public BarsData()
		{
			Colors = new List<ColorGradient>{new ColorGradient(Color.Red), new ColorGradient(Color.Green), new ColorGradient(Color.Blue)};
			Direction = BarDirection.Up;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public BarDirection Direction { get; set; }
		
		public override IModuleDataModel Clone()
		{
			BarsData result = new BarsData
			{
				Colors = Colors.ToList(),
				Direction = Direction
			};
			return result;
		}
	}
}
