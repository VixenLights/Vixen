using System.Windows;
using System.Xml;
using Catel.IoC;
using Catel.Services;
using NLog;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;

namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	public class XModelImport : IModelImport
	{
		protected static Logger Logging = LogManager.GetCurrentClassLogger();
		private int _scale = 4;
		private const int Offset = 7;
		
		public async Task<Prop> ImportAsync(string filePath)
		{
			return await LoadModelFileAsync(filePath);
		}

		private async Task<Prop> LoadModelFileAsync(string filePath)
		{
			Prop p = null;
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.Async = true;
			using (XmlReader reader = XmlReader.Create(filePath, settings))
			{
				await reader.MoveToContentAsync();

				if ("custommodel".Equals(reader.Name) && reader.HasAttributes)
				{
					string name = reader.GetAttribute("name");
					p = PropModelServices.Instance().CreateProp(name);
					p.CreatedBy = @"xModel Import";

					//These are the size of the grid near as I can tell
					//We will use them to gauge a scale.
					int x, y;
					int.TryParse(reader.GetAttribute("parm1"), out x);
					int.TryParse(reader.GetAttribute("parm2"), out y);

					CalculateScale(x, y);

					int nodeSize;
					int.TryParse(reader.GetAttribute("PixelSize"), out nodeSize);
					string model = reader.GetAttribute("CustomModel");

					List<SubModel> subModels = new List<SubModel>();
					while (reader.Read())
					{
						if(reader.NodeType != XmlNodeType.Element) continue;

						if ("subModel".Equals(reader.Name))
						{
							SubModel sm = new SubModel();
							sm.Name = reader.GetAttribute("name");
							sm.Layout = reader.GetAttribute("layout");
							var type = reader.GetAttribute("type");
							switch (type)
							{
								case "ranges":
									sm.Type = SubModelType.Ranges;
                                    sm.Ranges = ProcessRanges(reader);//ParseRanges(reader.GetAttribute("line0"));
									break;
							}
							subModels.Add(sm);

						}
						else if("faceInfo".Equals(reader.Name))
						{
							var type = reader.GetAttribute("Type");
							if (!string.IsNullOrEmpty(type) && type.Equals("NodeRange"))
							{
								foreach (var attribute in FaceInfo.Attributes)
								{
									var range = reader.GetAttribute(attribute.Key);
									if (!string.IsNullOrEmpty(range))
									{
										SubModel sm = new SubModel();
										sm.Name = attribute.Key;
										sm.Type = SubModelType.Ranges;
										sm.Ranges = new List<RangeGroup>{ ParseRanges(range) };
										sm.FaceInfo = new FaceInfo(attribute.Value);
										subModels.Add(sm);
									}
								}
								
							}
							
						}
					}

					var modelNodes = await CreateModelNodesAsync(model);
					Assemble(modelNodes, subModels, nodeSize);

				}
				else
				{
					var dependencyResolver = this.GetDependencyResolver();
					var ms = dependencyResolver.Resolve<IMessageService>();
					await ms.ShowErrorAsync($"Unsupported model type: {reader.Name}. \nImport only supports custom model types at this time.", "Model import error");
				}
			}

			return p;
		}

        private List<RangeGroup> ProcessRanges(XmlReader reader)
        {
            int line = 0;
            bool found = true;
            List<RangeGroup> rangeGroups = new List<RangeGroup>();
            while (found)
            {
                var range = reader.GetAttribute($"line{line}");
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

		private void CalculateScale(int x, int y)
		{
			if (x < 100 && y < 100)
			{
				_scale = 4;
			}
			else if(x < 200 && y < 200)
			{
				_scale = 2;
			}
			else
			{
				_scale = 1;
			}
		}

		private void Assemble(Dictionary<int, ModelNode> modelNodes, List<SubModel> subModels, int nodeSize)
		{
			Dictionary<int, ElementModel> lightNodes = new Dictionary<int, ElementModel>();
			
			foreach (var subModel in subModels)
            {

                var subModelGroup = PropModelServices.Instance().CreateNode(subModel.Name);
                if (subModel.FaceInfo.FaceComponent != FaceComponent.None)
                {
                    subModelGroup.FaceComponent = subModel.FaceInfo.FaceComponent;
                }

                bool addRangeGroup = subModel.Ranges.Count > 1;
                int rangeGroupIndex = 1;

                var subModelRangeGroup = subModelGroup;
				foreach (var rangeGroup in subModel.Ranges)
                {
					
                    if (addRangeGroup)
                    {
                        subModelRangeGroup = PropModelServices.Instance().CreateNode($"{subModelGroup.Name} {rangeGroupIndex}", subModelGroup);
                    }

                    foreach (var smRange in rangeGroup.Ranges)
                    {
                        int inc = smRange.Start > smRange.End ? -1 : 1;
                        for (int i = smRange.Start; ;i=i+inc)
                        {
                            if (inc > 0 && i > smRange.End)
                            {
                                break;
                            }
                            if (inc < 0 && i < smRange.End)
                            {
                                break;
                            }

                            if (modelNodes.ContainsKey(i))
                            {
                                var modelNode = modelNodes[i];
                                modelNodes.Remove(i);
                                var lightNode = PropModelServices.Instance().AddLightNode(subModelRangeGroup, new Point(modelNode.X + Offset, modelNode.Y + Offset), modelNode.Order, nodeSize);
                                if (!lightNodes.ContainsKey(modelNode.Order))
                                {
                                    lightNodes.Add(modelNode.Order, lightNode);
                                }
                            }
                            else
                            {
                                //We have probably already associated it with another group so look it up
                                ElementModel lightNode;
                                if (lightNodes.TryGetValue(i, out lightNode))
                                {
                                    PropModelServices.Instance().AddToParent(lightNode, subModelRangeGroup);
                                }
                            }
                        }
                    }

                    rangeGroupIndex++;
				}

                
            }

            if (modelNodes.Any())
			{
				ElementModel em = null;
				if (subModels.Any())
				{
					//Create a group to hold the stuff that was not included in the submodels
					em = PropModelServices.Instance().CreateNode("Other");
				}

				foreach (var modelNode in modelNodes.OrderBy(x => x.Value.Order))
				{
					PropModelServices.Instance().AddLightNode(em, new Point(modelNode.Value.X+Offset, modelNode.Value.Y+Offset),
						modelNode.Value.Order, nodeSize);
				}
			}
		}



		private RangeGroup ParseRanges(string rangeLines)
		{
			List<Range> rangeList = new List<Range>();
			var ranges = rangeLines.Split(',');
			foreach (var range in ranges)
			{
				var trimmedRange = range.Trim();
				if (!string.IsNullOrEmpty(trimmedRange))
				{
					Range r = new Range();
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

		private async Task<Dictionary<int, ModelNode>> CreateModelNodesAsync(string model)
		{

			var elementCandidates = new Dictionary<int, ModelNode>();
			await Task.Factory.StartNew(() =>
			{

				string[] rows = model.Split(';');
				int y = 1;
				foreach (var row in rows)
				{
					string[] nodes = row.Split(',');
					int x = 1;
					foreach (var node in nodes)
					{
						if (!string.IsNullOrEmpty(node))
						{
							int order;
							int.TryParse(node, out order);
							var modelNode = new ModelNode()
							{
								Order = order,
								X = x,
								Y = y
							};

							elementCandidates[order] = modelNode;
						}

						x += _scale;
					}

					y += _scale;
				}

			});

			return elementCandidates;

		}
	}
}
