#nullable disable

using Dataweb.NShape.Advanced;

namespace VixenApplication.GraphicalPatching
{
	public static class NShapeLibraryInitializer
	{
		public static void Initialize(IRegistrar registrar)
		{
			registrar.RegisterLibrary(namespaceName, preferredRepositoryVersion);
			registrar.RegisterShapeType(
				new ShapeType("ElementNodeShape", namespaceName, namespaceName,
				              ElementNodeShape.CreateInstance, ElementNodeShape.GetPropertyDefinitions)
				);
			registrar.RegisterShapeType(
				new ShapeType("ControllerShape", namespaceName, namespaceName,
				              ControllerShape.CreateInstance, ControllerShape.GetPropertyDefinitions)
				);
			registrar.RegisterShapeType(
				new ShapeType("OutputShape", namespaceName, namespaceName,
				              OutputShape.CreateInstance, OutputShape.GetPropertyDefinitions)
				);
			registrar.RegisterShapeType(
				new ShapeType("FilterShape", namespaceName, namespaceName,
				              FilterShape.CreateInstance, FilterShape.GetPropertyDefinitions)
				);
			registrar.RegisterShapeType(
				new ShapeType("DataFlowConnectionLine", namespaceName, namespaceName,
				              DataFlowConnectionLine.CreateInstance, DataFlowConnectionLine.GetPropertyDefinitions)
				);
		}

		private const string namespaceName = "VixenFilterShapes";
		private const int preferredRepositoryVersion = 3;
	}
}