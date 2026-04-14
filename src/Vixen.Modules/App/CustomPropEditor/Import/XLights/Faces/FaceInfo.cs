using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.Import.XLights.Faces
{
	internal class FaceInfo
	{
		public static Dictionary<string,FaceComponent> Attributes = new Dictionary<string, FaceComponent>()
		{
			{ "Eyes-Closed", FaceComponent.EyesClosed},
			{ "Eyes-Open", FaceComponent.EyesOpen},
			{ "Eyes-Closed2", FaceComponent.EyesClosed2},
			{ "Eyes-Open2", FaceComponent.EyesOpen2},
			{ "Eyes-Closed3", FaceComponent.EyesClosed3},
			{ "Eyes-Open3", FaceComponent.EyesOpen3},
			{ "FaceOutline", FaceComponent.Outlines },
			{ "FaceOutline2", FaceComponent.Outlines },
			{ "Mouth-AI", FaceComponent.AI},
			{ "Mouth-FV", FaceComponent.FV},
			{ "Mouth-MBP",FaceComponent.MBP},
			{ "Mouth-E", FaceComponent.E},
			{ "Mouth-L", FaceComponent.L},
			{ "Mouth-O", FaceComponent.O},
			{ "Mouth-U", FaceComponent.U},
			{ "Mouth-WQ", FaceComponent.WQ},
			{ "Mouth-etc", FaceComponent.ETC},
			{ "Mouth-rest", FaceComponent.REST}
		};

		public FaceInfo(string name)
		{
			Name = name;
			FaceDefinitions = new List<FaceItem>();
			ModelType = ModelType.NodeRanges;
		}

		public string Name { get; }

		public bool CustomColors { get; set; }

		public ModelType ModelType { get; set; }

		public List<FaceItem> FaceDefinitions { get;}

	}
}
