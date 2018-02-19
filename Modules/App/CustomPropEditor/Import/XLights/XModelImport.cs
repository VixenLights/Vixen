using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;

namespace VixenModules.App.CustomPropEditor.Import.XLights
{
    public class XModelImport: IModelImport
    {
        private const int Scale = 4;
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
                    int nodeSize;
                    int.TryParse(reader.GetAttribute("PixelSize"), out nodeSize);
                    string model = reader.GetAttribute("CustomModel");

                    List<SubModel> subModels = new List<SubModel>();
                    while (reader.Read())
                    {
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
                                    sm.Ranges = ParseRanges(reader.GetAttribute("line0"));
                                    break;
                            }
                            subModels.Add(sm);

                        }
                    }

                    var modelNodes = await CreateModelNodesAsync(model);
                    Assemble(modelNodes, subModels, nodeSize);
                   
                }
            }

            return p;
        }

        private void Assemble(Dictionary<int, ModelNode> modelNodes, List<SubModel> subModels, int nodeSize)
        {
            foreach (var subModel in subModels)
            {
                var group = PropModelServices.Instance().CreateNode(subModel.Name);
                foreach (var smRange in subModel.Ranges)
                {
                    for (int i = smRange.Start; i <= smRange.End; i++)
                    {
                        if (modelNodes.ContainsKey(i))
                        {
                            var modelNode = modelNodes[i];
                            modelNodes.Remove(i);
                            PropModelServices.Instance().AddLightNode(group, new Point(modelNode.X, modelNode.Y), modelNode.Order, nodeSize);
                        }
                    }
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
                    PropModelServices.Instance().AddLightNode(em, new Point(modelNode.Value.X, modelNode.Value.Y),
                        modelNode.Value.Order, nodeSize);
                }
            }
        }

        

        private List<Range> ParseRanges(string rangeLines)
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
                    else if(startEnd.Length == 1)
                    {
                        r.Start = r.End = Convert.ToInt32(startEnd[0]);
                        rangeList.Add(r);
                    }
                }
            }

            return rangeList;

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
                                X=x,
                                Y=y
                            };
                            
                            elementCandidates[order] = modelNode;
                        }

                        x+=Scale;
                    }

                    y+=Scale;
                }

            });

            return elementCandidates;

        }
    }
}
