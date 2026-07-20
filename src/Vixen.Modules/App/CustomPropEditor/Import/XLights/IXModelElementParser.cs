using System.Xml.Linq;

namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	internal interface IXModelElementParser
	{
		bool CanImport(XElement modelElement);

		string ResolveModelType(XElement modelElement);

		Task<XModelParsedModel> ParseAsync(XElement modelElement);
	}
}
