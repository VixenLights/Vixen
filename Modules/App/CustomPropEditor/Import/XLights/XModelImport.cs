using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Xml;
using VixenModules.App.CustomPropEditor.Model;

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

                    int nodeSize;
                    int.TryParse(reader.GetAttribute("PixelSize"), out nodeSize);
                    string model = reader.GetAttribute("CustomModel");
                    var ecTask = CreatePropFromModelAsync(model, nodeSize, name);

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

                    var elementCandidates = await ecTask;

                    p = AssembleProp(elementCandidates, subModels);

                    p.Name = name;
                }
            }

            return p;
        }

        private Prop AssembleProp(List<ElementModel> elementCandidates, List<SubModel> subModels)
        {
            Prop p = new Prop();
            if (subModels.Any())
            {
                foreach (var sm in subModels)
                {
                    var ec = new ElementModel(sm.Name);
                    foreach (var smRange in sm.Ranges)
                    {
                        var nodes = Rename(
                            elementCandidates.Where(x => x.Order >= smRange.Start && x.Order <= smRange.End)
                                .OrderBy(x => x.Order), sm.Name);
                        foreach (var elementCandidate in nodes)
                        {
                            ec.Children.Add(elementCandidate);
                        }
                    }
                    p.AddElementModel(ec);
                }
            }
            else
            {
                p.AddElementModels(elementCandidates.OrderBy(ec => ec.Order));
            }

            return p;
        }

        private IEnumerable<ElementModel> Rename(IEnumerable<ElementModel> elementCandidates, string newName)
        {
            foreach (var elementCandidate in elementCandidates)
            {
                elementCandidate.Name = String.Format("{0}-{1}", newName, elementCandidate.Order);
            }

            return elementCandidates;
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

        private async Task<List<ElementModel>> CreatePropFromModelAsync(string model, int nodeSize, string name)
        {
           
            List<ElementModel> elementCandidates = new List<ElementModel>();
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
                            var ec = new ElementModel
                            {
                                Name = string.Format("{0}-{1}", name, order),
                                Order = order,
                                LightSize = nodeSize
                            };
                            ec.AddLight(new Point(x,y));
                            elementCandidates.Add(ec);
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
