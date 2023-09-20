using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.Import.XLights.Faces
{
	internal class FaceInfo
	{
		public static Dictionary<string,FaceComponent> Attributes = new Dictionary<string, FaceComponent>()
		{
			{ "Eyes-Closed", Model.FaceComponent.EyesClosed},
			{ "Eyes-Open", Model.FaceComponent.EyesOpen},
			{ "Eyes-Closed2", Model.FaceComponent.EyesClosed2},
			{ "Eyes-Open2", Model.FaceComponent.EyesOpen2},
			{ "Eyes-Closed3", Model.FaceComponent.EyesClosed3},
			{ "Eyes-Open3", Model.FaceComponent.EyesOpen3},
			{ "FaceOutline", Model.FaceComponent.Outlines },
			{ "FaceOutline2", Model.FaceComponent.Outlines },
			{ "Mouth-AI", Model.FaceComponent.AI},
			{ "Mouth-FV", Model.FaceComponent.FV},
			{ "Mouth-MBP",Model.FaceComponent.MBP},
			{ "Mouth-E", Model.FaceComponent.E},
			{ "Mouth-L", Model.FaceComponent.L},
			{ "Mouth-O", Model.FaceComponent.O},
			{ "Mouth-U", Model.FaceComponent.U},
			{ "Mouth-WQ", Model.FaceComponent.WQ},
			{ "Mouth-etc", Model.FaceComponent.ETC},
			{ "Mouth-rest", Model.FaceComponent.REST}
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
