using VixenModules.App.CustomPropEditor.Import.XLights;

namespace VixenModules.App.CustomPropEditor.Import.XLights.Circle
{
	internal static class CircleXModelNodeGenerator
	{
		internal static Dictionary<int, ModelNode> CreateModelNodes(CircleXModelConfiguration configuration)
		{
			var generatedNodes = new List<ModelNode>();
			var maxLights = configuration.LayerSizes.Max();
			var maxRadius = maxLights / 2.0;
			var minRadius = configuration.CenterPercent / 100.0 * maxRadius;

			foreach (var ring in configuration.Rings)
			{
				var nodeCount = configuration.LayerSizes[ring.PhysicalLayerIndex];
				var radius = GetLayerRadius(
					ring.PhysicalLayerIndex,
					configuration.LayerSizes.Count,
					minRadius,
					maxRadius);
				var startAngle = configuration.StartSide == 'T' ? Math.PI : 0;
				var direction = configuration.Direction == 'L' ? -1 : 1;
				var angleStep = 2 * Math.PI / nodeCount;

				for (var nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
				{
					var angle = startAngle + direction * angleStep * nodeIndex;
					generatedNodes.Add(new ModelNode
					{
						Order = ring.NodeOrders[nodeIndex],
						X = (int)Math.Round(Math.Sin(angle) * radius * configuration.ScaleX, MidpointRounding.AwayFromZero),
						Y = (int)Math.Round(Math.Cos(angle) * radius * configuration.ScaleY, MidpointRounding.AwayFromZero)
					});
				}
			}

			NormalizeCoordinates(generatedNodes);
			return generatedNodes.ToDictionary(node => node.Order);
		}

		private static double GetLayerRadius(int physicalLayerIndex, int layerCount, double minRadius, double maxRadius)
		{
			if (layerCount == 1)
			{
				return maxRadius;
			}

			return minRadius + (maxRadius - minRadius) * physicalLayerIndex / (layerCount - 1);
		}

		private static void NormalizeCoordinates(IReadOnlyCollection<ModelNode> modelNodes)
		{
			if (!modelNodes.Any())
			{
				return;
			}

			var minX = modelNodes.Min(node => node.X);
			var minY = modelNodes.Min(node => node.Y);

			foreach (var modelNode in modelNodes)
			{
				modelNode.X -= minX;
				modelNode.Y -= minY;
			}
		}
	}
}
