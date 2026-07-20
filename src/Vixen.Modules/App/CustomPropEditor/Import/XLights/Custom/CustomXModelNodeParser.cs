using VixenModules.App.CustomPropEditor.Import.XLights;

namespace VixenModules.App.CustomPropEditor.Import.XLights.Custom
{
	internal static class CustomXModelNodeParser
	{
		internal static Dictionary<int, ModelNode> ParseCustomModel(string modelDefinition, int scale)
		{
			return ParseCustomModel(modelDefinition, new XModelCoordinateScale(scale, scale));
		}

		internal static Dictionary<int, ModelNode> ParseCustomModel(
			string modelDefinition,
			XModelCoordinateScale scale)
		{
			if (string.IsNullOrWhiteSpace(modelDefinition))
			{
				throw new FormatException("CustomModel is empty.");
			}

			var elementCandidates = new Dictionary<int, ModelNode>();
			var rows = modelDefinition.Split(';');
			var y = 1;
			foreach (var row in rows)
			{
				var nodes = row.Split(',');
				var zeroBasedX = 0;
				foreach (var node in nodes)
				{
					if (!string.IsNullOrWhiteSpace(node))
					{
						if (!int.TryParse(node, out var order))
						{
							throw new FormatException($"CustomModel node '{node}' is not a valid number.");
						}

						elementCandidates[order] = new ModelNode
						{
							Order = order,
							X = scale.ApplyX(zeroBasedX) + 1,
							Y = scale.ApplyY(y - 1) + 1
						};
					}

					zeroBasedX++;
				}

				y++;
			}

			return elementCandidates;
		}

		internal static Dictionary<int, ModelNode> ParseCustomModelCompressed(
			string compressedModelDefinition,
			int scale)
		{
			return ParseCustomModelCompressed(compressedModelDefinition, new XModelCoordinateScale(scale, scale));
		}

		internal static Dictionary<int, ModelNode> ParseCustomModelCompressed(
			string compressedModelDefinition,
			XModelCoordinateScale scale)
		{
			if (string.IsNullOrWhiteSpace(compressedModelDefinition))
			{
				throw new FormatException("CustomModelCompressed is empty.");
			}

			var elementCandidates = new Dictionary<int, ModelNode>();
			var nodes = compressedModelDefinition.Split(';', StringSplitOptions.RemoveEmptyEntries);
			foreach (var node in nodes)
			{
				var parts = node.Split(',');
				if (parts.Length != 3)
				{
					throw new FormatException($"CustomModelCompressed entry '{node}' must contain node, x, and y values.");
				}

				if (!int.TryParse(parts[0], out var order) ||
					!int.TryParse(parts[1], out var compressedRow) ||
					!int.TryParse(parts[2], out var compressedColumn))
				{
					throw new FormatException($"CustomModelCompressed entry '{node}' contains an invalid number.");
				}

				elementCandidates[order] = new ModelNode
				{
					Order = order,
					X = scale.ApplyX(compressedColumn) + 1,
					Y = scale.ApplyY(compressedRow) + 1
				};
			}

			return elementCandidates;
		}
	}
}
