using System.Drawing;
using System.Xml.Linq;
using NLog;
using VixenModules.App.CustomPropEditor.Import.XLights.Faces;
using VixenModules.App.CustomPropEditor.Import.XLights.Ranges;
using Range = VixenModules.App.CustomPropEditor.Import.XLights.Ranges.Range;

namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	internal sealed class XModelChildElementImporter
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();

		public void ImportChildElements(XModelImportModel importModel, XElement modelElement)
		{
			foreach (var childElement in modelElement.Elements())
			{
				if (XModelElementMetadata.ElementNameEquals(childElement, "subModel"))
				{
					ImportSubModel(importModel, childElement);
				}
				else if (XModelElementMetadata.ElementNameEquals(childElement, "faceInfo"))
				{
					ImportFaceInfo(importModel, childElement);
				}
				else if (XModelElementMetadata.ElementNameEquals(childElement, "stateInfo"))
				{
					ImportStateInfo(importModel, childElement);
				}
				else if (XModelElementMetadata.ElementNameEquals(childElement, "modelGroup"))
				{
					Logging.Info(
						"Skipping xModel modelGroup {ModelGroupName} in model {ModelName}; modelGroup import is not supported.",
						XModelElementMetadata.GetAttributeValue(childElement, "name"),
						importModel.Name);
				}
			}
		}

		private static void ImportSubModel(XModelImportModel importModel, XElement subModelElement)
		{
			var subModel = new SubModel(XModelElementMetadata.GetAttributeValue(subModelElement, "name"))
			{
				Layout = XModelElementMetadata.GetAttributeValue(subModelElement, "layout")
			};
			var type = XModelElementMetadata.GetAttributeValue(subModelElement, "type");
			switch (type)
			{
				case "ranges":
					subModel.Type = ModelType.Ranges;
					subModel.Ranges = ProcessRanges(subModelElement);
					importModel.SubModels.Add(subModel);
					break;
				case "subbuffer":
					break;
			}
		}

		private static void ImportFaceInfo(XModelImportModel importModel, XElement faceInfoElement)
		{
			var type = XModelElementMetadata.GetAttributeValue(faceInfoElement, "Type");
			if (string.IsNullOrEmpty(type) || !type.Equals("NodeRange"))
			{
				return;
			}

			var faceInfo = new FaceInfo(XModelElementMetadata.GetAttributeValue(faceInfoElement, "Name"));
			foreach (var attribute in FaceInfo.Attributes)
			{
				var range = XModelElementMetadata.GetAttributeValue(faceInfoElement, attribute.Key);
				if (string.IsNullOrEmpty(range))
				{
					continue;
				}

				var faceItem = new FaceItem
				{
					FaceComponent = attribute.Value,
					RangeGroup = ParseRanges(range)
				};

				var color = XModelElementMetadata.GetAttributeValue(faceInfoElement, attribute.Key + "-Color");
				if (!string.IsNullOrEmpty(color))
				{
					faceItem.Color = ColorTranslator.FromHtml(color);
				}

				faceItem.Name = attribute.Key;
				faceInfo.FaceDefinitions.Add(faceItem);
			}

			importModel.FaceInfos.Add(faceInfo);
		}

		private static void ImportStateInfo(XModelImportModel importModel, XElement stateInfoElement)
		{
			if (!stateInfoElement.HasAttributes)
			{
				return;
			}

			var type = XModelElementMetadata.GetAttributeValue(stateInfoElement, "Type");
			if (string.IsNullOrEmpty(type) || !type.Equals("NodeRange") && !type.Equals("SingleNode"))
			{
				return;
			}

			var stateInfo = new StateInfo(
				XLightsStateNameNormalizer.NormalizeStateName(
					XModelElementMetadata.GetAttributeValue(stateInfoElement, "Name"),
					importModel.StateInfos.Count + 1));

			Dictionary<string, StateItem> states = new();

			foreach (var attribute in stateInfoElement.Attributes())
			{
				switch (attribute.Name.LocalName)
				{
					case "Type":
					case "NodeRange":
					case "Name":
					case "CustomColors":
						continue;
				}

				if (!attribute.Name.LocalName.StartsWith("s"))
				{
					continue;
				}

				var parts = attribute.Name.LocalName.Split('-');
				if (parts.Length <= 0)
				{
					continue;
				}

				if (!states.ContainsKey(parts[0]))
				{
					if (int.TryParse(parts[0].Substring(1), out var index))
					{
						var stateItem = new StateItem(
							index,
							XLightsStateNameNormalizer.NormalizeStateItemName(null, parts[0]));
						states.Add(parts[0], stateItem);
					}
				}

				if (parts.Length == 1)
				{
					var ranges = attribute.Value;
					if (states.ContainsKey(parts[0]))
					{
						if (type == "NodeRange")
						{
							states[parts[0]].RangeGroup = ParseRanges(ranges);
						}
						else if (type == "SingleNode")
						{
							var nodeInfo = ranges.Split(' ');
							if (nodeInfo.Length == 2)
							{
								if (int.TryParse(nodeInfo[1], out _))
								{
									states[parts[0]].RangeGroup = ParseRanges(nodeInfo[1]);
								}
							}
						}
					}
				}
				else if (parts.Length == 2)
				{
					if ("Color".Equals(parts[1]))
					{
						var color = attribute.Value;
						if (!string.IsNullOrEmpty(color))
						{
							if (states.ContainsKey(parts[0]))
							{
								states[parts[0]].Color = ColorTranslator.FromHtml(color);
							}
						}
					}

					if ("Name".Equals(parts[1]))
					{
						var stateItemName = XLightsStateNameNormalizer.NormalizeStateItemName(
							attribute.Value,
							parts[0]);

						if (states.ContainsKey(parts[0]))
						{
							states[parts[0]].Name = stateItemName;
						}
					}
				}
			}

			foreach (var stateItem in states)
			{
				if (stateItem.Value.RangeGroup == null)
				{
					continue;
				}
				stateInfo.StateItems.Add(stateItem.Value);
			}

			importModel.StateInfos.Add(stateInfo);
		}

		private static List<RangeGroup> ProcessRanges(XElement element)
		{
			var line = 0;
			var found = true;
			List<RangeGroup> rangeGroups = [];
			while (found)
			{
				var range = XModelElementMetadata.GetAttributeValue(element, $"line{line}");
				if (!string.IsNullOrEmpty(range))
				{
					rangeGroups.Add(ParseRanges(range));
					line++;
				}
				else
				{
					found = false;
				}
			}

			return rangeGroups;
		}

		private static RangeGroup ParseRanges(string rangeLines)
		{
			List<Range> rangeList = [];
			var ranges = rangeLines.Split(',');
			foreach (var range in ranges)
			{
				var trimmedRange = range.Trim();
				if (!string.IsNullOrEmpty(trimmedRange))
				{
					Range r = new();
					var startEnd = trimmedRange.Split('-');
					if (startEnd.Length == 2)
					{
						r.Start = Convert.ToInt32(startEnd[0]);
						r.End = Convert.ToInt32(startEnd[1]);
						rangeList.Add(r);
					}
					else if (startEnd.Length == 1)
					{
						r.Start = r.End = Convert.ToInt32(startEnd[0]);
						rangeList.Add(r);
					}
				}
			}

			return new RangeGroup(rangeList);
		}
	}
}
