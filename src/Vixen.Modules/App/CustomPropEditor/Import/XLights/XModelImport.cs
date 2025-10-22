using Catel.IoC;
using Catel.Services;
using NLog;
using System.Drawing;
using System.Xml;
using VixenModules.App.CustomPropEditor.Import.XLights.Faces;
using VixenModules.App.CustomPropEditor.Import.XLights.Ranges;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;
using Point = System.Windows.Point;
using Range = VixenModules.App.CustomPropEditor.Import.XLights.Ranges.Range;

namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	public class XModelImport : IModelImport
	{
		protected static Logger Logging = LogManager.GetCurrentClassLogger();
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

					CustomModel cm = new CustomModel(name);
					
					//These are the size of the grid near as I can tell
					//We will use them to gauge a scale.
					int.TryParse(reader.GetAttribute("parm1"), out var x);
					int.TryParse(reader.GetAttribute("parm2"), out var y);

					cm.X = x;  
					cm.Y = y;

					if (x < 800 && y < 600)
					{
						//Ensure a minimum size by using the default
						p = PropModelServices.Instance().CreateProp($"{name} {{1}}");
					}
					else
					{
						p = PropModelServices.Instance().CreateProp($"{name} {{1}}", x + 20, y + 20);
					}

					p.CreatedBy = @"xModel Import";

					int.TryParse(reader.GetAttribute("PixelSize"), out var nodeSize);
					cm.PixelSize = nodeSize;

					cm.StringType = reader.GetAttribute("StringType");
					cm.StrandNames = reader.GetAttribute("StrandNames");
					cm.NodeNames = reader.GetAttribute("NodeNames");
					string model = reader.GetAttribute("CustomModel");  //TODO Clean up
					cm.ModelDefinition = model;

					while (reader.Read())
					{
						if(reader.NodeType != XmlNodeType.Element) continue;

						if ("subModel".Equals(reader.Name))
						{
							SubModel sm = new SubModel(reader.GetAttribute("name"))
							{
								Layout = reader.GetAttribute("layout")
							};
							var type = reader.GetAttribute("type");
							switch (type)
							{
								case "ranges":
									sm.Type = ModelType.Ranges;
                                    sm.Ranges = ProcessRanges(reader);//ParseRanges(reader.GetAttribute("line0"));
									cm.SubModels.Add(sm);
									break;
								case "subbuffer":
									//There is currently no equivalent for this option
									break;
							}
						}
						else if("faceInfo".Equals(reader.Name))
						{
							var type = reader.GetAttribute("Type");
							if (!string.IsNullOrEmpty(type) && type.Equals("NodeRange"))
							{
								var fi = new FaceInfo(reader.GetAttribute("Name"));
								
								foreach (var attribute in FaceInfo.Attributes)
								{
									var range = reader.GetAttribute(attribute.Key);
									if (!string.IsNullOrEmpty(range))
									{
										FaceItem fd = new FaceItem
										{
											FaceComponent = attribute.Value,
											RangeGroup = ParseRanges(range)
										};
										
										var color = reader.GetAttribute(attribute.Key + "-Color");
										if (!String.IsNullOrEmpty(color))
										{
											fd.Color = ColorTranslator.FromHtml(color);
										}

										fd.Name = attribute.Key;
										fi.FaceDefinitions.Add(fd);
									}
								}
								cm.FaceInfos.Add(fi);
								
							}
							
						}
						else if ("stateInfo".Equals(reader.Name))
						{
							if (reader.HasAttributes)
							{
								var type = reader.GetAttribute("Type");
								if (!string.IsNullOrEmpty(type) && type.Equals("NodeRange") || type.Equals("SingleNode"))
								{
									var stateInfo = new StateInfo(reader.GetAttribute("Name"));
									
									//Parse the state values
									Dictionary<string, StateItem> states = new ();

									while (reader.MoveToNextAttribute())
									{
										switch (reader.Name)
										{
											case "Type":
											case "NodeRange":
											case "Name":
											case "CustomColors":
												continue;
										}

										if (reader.Name.StartsWith("s"))
										{
											var parts = reader.Name.Split('-');
											if (parts.Length > 0)
											{
												if (!states.ContainsKey(parts[0]))
												{
													if (int.TryParse(parts[0].Substring(1), out var index))
													{
														StateItem si = new StateItem(index);
														states.Add(parts[0], si);
													}
												}
												
												if (parts.Length == 1)
												{
													var ranges = reader.Value;
													if (states.ContainsKey(parts[0]))
													{
														if (type == "NodeRange")
														{
															states[parts[0]].RangeGroup = ParseRanges(ranges);
														}
														else if(type == "SingleNode")
														{
															var nodeInfo = ranges.Split(' ');
															if (nodeInfo.Length == 2)
															{
																if (int.TryParse(nodeInfo[1],  out var nodeNumber))
																{
																	//we have a valid node, so send it to the range parser to create a range of one.
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
														var color = reader.Value;
														if (!String.IsNullOrEmpty(color))
														{
															if (states.ContainsKey(parts[0]))
															{
																states[parts[0]].Color = ColorTranslator.FromHtml(color);
															}
														}
													}

													if ("Name".Equals(parts[1]))
													{
														var stateItemName = reader.Value;
														
														if (states.ContainsKey(parts[0]))
														{
															states[parts[0]].Name = stateItemName;
														}
														
													}
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
									
									cm.StateInfos.Add(stateInfo);
								}
							}
							
						}
					}

					
					await Assemble(cm);

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

        private async Task Assemble(CustomModel cm)
		{
			//Create the list of light Node candidates.
			var modelNodes = await cm.CreateModelNodesAsync();

			Dictionary<int, ElementModel> lightNodes = new Dictionary<int, ElementModel>();

			if (cm.SubModels.Any())
			{
				AssembleSubModels(cm, modelNodes, lightNodes);
			}
			
			if (cm.FaceInfos.Any())
			{
				AssembleFaces(cm, modelNodes, lightNodes);
			}

			if (cm.StateInfos.Any())
			{
				AssembleStates(cm, modelNodes, lightNodes);
			}

			if (modelNodes.Any())
			{
				ElementModel em = null;
				if (cm.SubModels.Any() || cm.FaceInfos.Any() || cm.StateInfos.Any())
				{
					//Create a group to hold the stuff that was not included in the submodels
					em = PropModelServices.Instance().CreateNode("Other");
				}

				foreach (var modelNode in modelNodes.OrderBy(x => x.Value.Order))
				{
					PropModelServices.Instance().AddLightNode(em, new Point(modelNode.Value.X + Offset, modelNode.Value.Y + Offset),
						modelNode.Value.Order, cm.PixelSize, $"{cm.Name} {{1}} Px {modelNode.Value.Order}");
				}
			}
		}

		private void AssembleSubModels(CustomModel cm, Dictionary<int, ModelNode> modelNodes, Dictionary<int, ElementModel> lightNodes)
		{
			foreach (var subModel in cm.SubModels)
			{
				if(subModel.Type == ModelType.Ranges && !subModel.Ranges.Any()) continue; //Skip sub models with empty ranges
				var subModelGroup = PropModelServices.Instance().CreateNode($"{cm.Name} {{1}} - {subModel.Name}");

                bool addRangeGroup = subModel.Ranges.Count > 1;
                int rangeGroupIndex = 1;

                var subModelRangeGroup = subModelGroup;
				foreach (var rangeGroup in subModel.Ranges)
                {
	                if (!rangeGroup.Ranges.Any())
	                {
						continue;
	                }
                    if (addRangeGroup)
                    {
	                    subModelRangeGroup = PropModelServices.Instance().CreateNode($"{cm.Name} {{1}} - {subModel.Name} - {subModelGroup.Name} {rangeGroupIndex}", subModelGroup);
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
                                var lightNode = PropModelServices.Instance().AddLightNode(subModelRangeGroup, new Point(modelNode.X + Offset, modelNode.Y + Offset), modelNode.Order, cm.PixelSize, $"{cm.Name} {{1}} Px {modelNode.Order}");
                                if (!lightNodes.ContainsKey(modelNode.Order))
                                {
                                    lightNodes.Add(modelNode.Order, lightNode);
                                }
                            }
                            else
                            {
                                //We have probably already associated it with another group so look it up
                                if (lightNodes.TryGetValue(i, out var lightNode))
                                {
                                    PropModelServices.Instance().AddToParent(lightNode, subModelRangeGroup);
                                }
                            }
                        }
                    }

                    rangeGroupIndex++;
				}

                
            }
		}

		private void AssembleFaces(CustomModel cm, Dictionary<int, ModelNode> modelNodes, Dictionary<int, ElementModel> lightNodes)
		{
			if (!cm.FaceInfos.Any())
			{
				return;
			}

			var parentFaceGroup = PropModelServices.Instance().CreateNode($"{cm.Name} {{1}} - Faces ");

			foreach (var faceInfo in cm.FaceInfos)
			{
				var faceGroup = PropModelServices.Instance().CreateNode($"{cm.Name} {{1}} - {faceInfo.Name} ", parentFaceGroup);

				foreach (var faceDefinition in faceInfo.FaceDefinitions)
				{
					var faceItemGroup = PropModelServices.Instance().CreateNode($"{cm.Name} {{1}} - {faceInfo.Name} - {faceDefinition.Name}", faceGroup);

					faceItemGroup.FaceDefinition = new FaceDefinition()
					{
						DefaultColor = faceDefinition.Color.Name != "ff000000"?faceDefinition.Color:Color.White,
						FaceComponent = faceDefinition.FaceComponent
					};

					var subModelRangeGroup = faceItemGroup;
					
					foreach (var smRange in faceDefinition.RangeGroup.Ranges)
					{
						int inc = smRange.Start > smRange.End ? -1 : 1;
						for (int i = smRange.Start; ; i = i + inc)
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
								var lightNode = PropModelServices.Instance().AddLightNode(subModelRangeGroup, new Point(modelNode.X + Offset, modelNode.Y + Offset), modelNode.Order, cm.PixelSize, $"{cm.Name} {{1}} Px {modelNode.Order}");
								if (!lightNodes.ContainsKey(modelNode.Order))
								{
									lightNodes.Add(modelNode.Order, lightNode);
								}
							}
							else
							{
								//We have probably already associated it with another group so look it up
								if (lightNodes.TryGetValue(i, out var lightNode))
								{
									PropModelServices.Instance().AddToParent(lightNode, subModelRangeGroup);
								}
							}
						}
					}
				}
			}

		}

		private void AssembleStates(CustomModel cm, Dictionary<int, ModelNode> modelNodes, Dictionary<int, ElementModel> lightNodes)
		{
			if (!cm.StateInfos.Any())
			{
				return;
			}

			var parentStateGroup = PropModelServices.Instance().CreateNode($"{cm.Name} {{1}} - States ");

			foreach (var stateInfo in cm.StateInfos)
			{
				var stateGroup = PropModelServices.Instance().CreateNode($"{cm.Name} {{1}} - {stateInfo.Name} ", parentStateGroup);

				foreach (var stateItem in stateInfo.StateItems)
				{
					var stateItemGroup = PropModelServices.Instance().CreateNode($"{cm.Name} {{1}} - {stateInfo.Name} - S{stateItem.Index} - {stateItem.Name}", stateGroup);

					stateItemGroup.StateDefinition = new StateDefinition{StateDefinitionName = stateInfo.Name, DefaultColor = stateItem.Color, Name = stateItem.Name, Index = stateItem.Index};
					
					var subModelRangeGroup = stateItemGroup;

					foreach (var smRange in stateItem.RangeGroup.Ranges)
					{
						int inc = smRange.Start > smRange.End ? -1 : 1;
						for (int i = smRange.Start; ; i = i + inc)
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
								var lightNode = PropModelServices.Instance().AddLightNode(subModelRangeGroup, new Point(modelNode.X + Offset, modelNode.Y + Offset), modelNode.Order, cm.PixelSize, $"{cm.Name} {{1}} Px {modelNode.Order}");
								if (!lightNodes.ContainsKey(modelNode.Order))
								{
									lightNodes.Add(modelNode.Order, lightNode);
								}
							}
							else
							{
								//We have probably already associated it with another group so look it up
								if (lightNodes.TryGetValue(i, out var lightNode))
								{
									PropModelServices.Instance().AddToParent(lightNode, subModelRangeGroup);
								}
							}
						}
					}
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

		
	}
}
