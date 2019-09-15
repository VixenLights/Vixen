using System.Collections.Generic;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	public class FaceInfo
	{
		public static Dictionary<string,FaceComponent> Attributes = new Dictionary<string, FaceComponent>()
		{
			{ "Eyes-Closed", Model.FaceComponent.EyesClosed},
			{ "Eyes-Open", Model.FaceComponent.EyesOpen},
			{ "FaceOutline", Model.FaceComponent.Outlines },
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

		public FaceInfo()
		{
			FaceComponent = Model.FaceComponent.None;
		}

		public FaceInfo(FaceComponent faceComponent)
		{
			FaceComponent = faceComponent;
		}

		public FaceComponent FaceComponent { get;}

	}
}
