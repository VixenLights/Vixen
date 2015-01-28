using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.Script.Serialization;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Services;
using Vixen.Sys;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.App.WebServer.Model;
using VixenModules.Effect.Nutcracker;
using VixenModules.Effect.SetLevel;
using VixenModules.Property.Color;
using ZedGraph;
using Element = VixenModules.App.WebServer.Model.Element;

namespace VixenModules.App.WebServer.Service
{
	internal class ElementsHelper
	{

		public IEnumerable<Element> SearchElements(string query)
		{
			if (string.IsNullOrEmpty(query))
			{
				throw new ArgumentNullException("query");
			}
			var elements = new List<Element>();
			IEnumerable<ElementNode> elementNodes = VixenSystem.Nodes.GetAllNodes().Where(x => x.Name.ToLower().StartsWith(query.ToLower()));
			foreach (var elementNode in elementNodes)
			{
				AddNodes(elements,elementNode);
			}
			
			return elements.OrderBy(x => x.Name);
		}

		public IEnumerable<Element> GetElements()
		{
			IEnumerable<ElementNode> elementNodes = VixenSystem.Nodes.GetRootNodes();
			var elements = new List<Element>();
			foreach (var elementNode in elementNodes)
			{
				AddNodes(elements, elementNode);
			}
			return elements;

		}

		public IEnumerable<Element> GetChildElements(string id)
		{
			var elements = new List<Element>();
			Guid elementId;
			if (!Guid.TryParse(id, out elementId))
			{
				throw new ArgumentException(@"Invalid element id.", "id");
			}
			if (!VixenSystem.Nodes.ElementNodeExists(elementId))
			{
				return elements;
			}

			ElementNode parentElement = VixenSystem.Nodes.GetElementNode(elementId);
			foreach (var elementNode in parentElement.Children)
			{
				AddNodes(elements, elementNode, false);
			}

			return elements;
		}

		public IEnumerable<Element> GetParentElements(string id)
		{

			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentNullException(@"id");
			}
			
			Guid elementId;
			if (!Guid.TryParse(id, out elementId))
			{
				throw new ArgumentException("Invalid Element id.");
			}

			if (!VixenSystem.Nodes.ElementNodeExists(elementId))
			{
				throw new ArgumentException(@"Invalid Element id.", "id");
			}

			var elements = new List<Element>();
			ElementNode childElement = VixenSystem.Nodes.GetElementNode(elementId);
			foreach (var elementNode in childElement.Parents)
			{
				AddNodes(elements, elementNode, false);
			}

			return elements;
		}
		
		private void AddNodes(List<Element> elements, ElementNode elementNode, bool addChildren = true)
		{
			var element = new Element
			{
				Id = elementNode.Id,
				Name = elementNode.Name,
				Colors = ColorModule.getValidColorsForElementNode(elementNode, true).Select(ColorTranslator.ToHtml).ToList()
			};

			elements.Add(element);
			if (addChildren)
			{
				if (!elementNode.IsLeaf)
				{
					var children = new List<Element>();
					element.Children = children;
					foreach (var childNode in elementNode.Children)
					{
						AddNodes(children, childNode);
					}
				}
			}

		}

		public static Status TurnOnElement(Guid id, int seconds, double intensity, string color)
		{
			//if (string.IsNullOrEmpty(id))
			//{
			//	throw new ArgumentNullException(id);
			//}
			var status = new Status();

			if (!string.IsNullOrEmpty(color) && (color.Length != 7 || !	color.StartsWith("#")))
			{
				throw new ArgumentException(@"Invalid color", color);
			}

			if (intensity > 100 || intensity < 0)
			{
				intensity = 100;
			}
			
			//Guid elementId = Guid.Empty;
			//bool allElements = false;
			
			//if ("all".Equals(id))
			//{
			//	allElements = true;
			//}
			//else
			//{
			//	if (!Guid.TryParse(id, out elementId))
			//	{
			//		throw new ArgumentException(@"Invalid Element id", id);
			//	}
			//}
		
			//TODO the following logic for all does not properly deal with discrete color elements when turning all on
			//TODO they will not respond to turning on white if they are set up with a filter.
			//TODO enhance this to figure out what colors there are and turn them all on when we are turning all elements on.

			var effect = new SetLevel
			{
				TimeSpan = TimeSpan.FromSeconds(seconds),
				IntensityLevel = intensity/100,
				TargetNodes = new[] { VixenSystem.Nodes.GetElementNode(id) }
					//allElements ? VixenSystem.Nodes.GetRootNodes().ToArray() : new[] { VixenSystem.Nodes.GetElementNode(elementId) }
			};

			if (!string.IsNullOrEmpty(color))
			{
				Color elementColor = ColorTranslator.FromHtml(color);
				effect.Color = elementColor;	
			}
			
			Module.LiveContext.Execute(new EffectNode(effect, TimeSpan.Zero));
			status.Message = string.Format("{0} element(s) turned on for {1} seconds at 100% intensity.",
				VixenSystem.Nodes.GetElementNode(id).Name, seconds);

			return status;
		}

		public static Status Effect(ElementEffect elementEffect)
		{
			if (elementEffect == null)
			{
				throw new ArgumentNullException("elementEffect");
			}

			ElementNode node = VixenSystem.Nodes.GetElementNode(elementEffect.Id);
			if ( node == null)
			{
				throw new ArgumentException(@"Invalid element id", @"elementEffect");
			}
			var status = new Status();

			IModuleDescriptor effectDescriptor = ApplicationServices.GetModuleDescriptors<IEffectModuleInstance>().Cast<IEffectModuleDescriptor>().FirstOrDefault(x => x.EffectName.Equals(elementEffect.EffectName));

			if (effectDescriptor != null)
			{
				var effect = ApplicationServices.Get<IEffectModuleInstance>(effectDescriptor.TypeId);
				EffectNode effectNode = CreateEffectNode(effect, node, TimeSpan.FromMilliseconds(elementEffect.Duration));

				Object[] newValues = effectNode.Effect.ParameterValues;
				int index = 0;
				foreach (var sig in effectNode.Effect.Parameters)
				{
					if (sig.Type == typeof(Color))
					{
						newValues[index] = ColorTranslator.FromHtml(elementEffect.Color.First().Key);
					}
					else
					{
						if (sig.Type == typeof(ColorGradient))
						{
							ColorGradient cg = new ColorGradient();
							cg.Colors.Clear();
							foreach (var d in elementEffect.Color)
							{
								cg.Colors.Add(new ColorPoint(ColorTranslator.FromHtml(d.Key), d.Value));	
							}				
							newValues[index] = cg;
						}
						else
						{
							if (sig.Type == typeof(Curve))
							{

								PointPairList pointPairList = new PointPairList();
								foreach (KeyValuePair<double, double> keyValuePair in elementEffect.LevelCurve)
								{
									pointPairList.Add(keyValuePair.Key, keyValuePair.Value);
								}
								Curve curve = new Curve(pointPairList);
								newValues[index] = curve;
							}

						}
					}
					index++;
				}
				effectNode.Effect.ParameterValues = newValues;
				ApplyOptionParameters(effectNode, elementEffect.Options);
				Module.LiveContext.Execute(effectNode);
				status.Message = string.Format("{0} element(s) turned effect {1} for {2} milliseconds.",
				VixenSystem.Nodes.GetElementNode(elementEffect.Id).Name, elementEffect.EffectName, elementEffect.Duration);
			}
			
			return status;
		}

		private static void ApplyOptionParameters(EffectNode effectNode, Dictionary<string, string> options)
		{
			Object[] newValues = effectNode.Effect.ParameterValues;

			int index = 0;
			foreach (var sig in effectNode.Effect.Parameters)
			{
				if (options.ContainsKey(sig.Name))
				{
					//determine the type, we need to handle a few special cases ourselves
					//This is a bit messy, but will work for now.
					
							
					if (sig.Type == typeof(string)) //might already be the right type
					{
						newValues[index] = options[sig.Name];
					}
					else //try to convert it to the right type
					{
						newValues[index] = Convert.ChangeType(options[sig.Name], sig.Type);
					}
							
				}
			}
				index++;
			
			effectNode.Effect.ParameterValues = newValues;
		}

		private static EffectNode CreateEffectNode(IEffectModuleInstance effectInstance, ElementNode targetNode, TimeSpan timeSpan)
		{
			
			// populate the given effect instance with the appropriate target node and times, and wrap it in an effectNode
			effectInstance.TargetNodes = new[] { targetNode };
			effectInstance.TimeSpan = timeSpan;
			return new EffectNode(effectInstance, TimeSpan.Zero);

		}

		public static Status TextOnElement(string id, string text, string color, int position, int direction, int seconds)
		{
			var status = new Status();
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("id");
			}

			Guid elementId;
			if (!Guid.TryParse(id, out elementId))
			{
				throw new ArgumentException(@"Invalid element id", "id");
			}

			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentException(@"Text to display is null", "text");
			}

			if (string.IsNullOrEmpty(color))
			{
				throw new ArgumentException(@"Invalid color", "color");
			}

			if (color.Length != 7 || !color.StartsWith("#"))
			{
				throw new ArgumentException(@"Invalid color. Must be Hex.", "color");
			}
			Color elementColor = ColorTranslator.FromHtml(color);
			
			if (position > 100)
			{
				throw new ArgumentException(@"Position must be 0 - 100.", "position");
			}

			if (direction > 4)
			{
				throw new ArgumentException(@"Direction must be 0 - 4.", "direction");
			}

			var nutcrackerEffect = new Nutcracker
			{
				TimeSpan = TimeSpan.FromSeconds(seconds),
				NutcrackerData = new NutcrackerData
				{
					CurrentEffect = NutcrackerEffects.Effects.Text,
					Text_Top = position,
					Text_Direction = direction,
					Text_Line1 = text
				},

				TargetNodes = new[] { VixenSystem.Nodes.GetElementNode(elementId) }
			};
			nutcrackerEffect.NutcrackerData.Palette.SetColor(1, elementColor);

			Module.LiveContext.Execute(new EffectNode(nutcrackerEffect, TimeSpan.Zero));
			status.Message = string.Format("Text \"{0}\" applied to element {1} for {2} seconds.",
				VixenSystem.Nodes.GetElementNode(elementId).Name, text, seconds);

			return status;
		}
	}
}
