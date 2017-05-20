using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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
	/// <summary>
	/// Class to manipulate elements.
	/// </summary>
	internal class ElementsHelper
	{
		/// <summary>
		/// Searches for any elements that start with the requested string ignoring case
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Gets a list of all elements
		/// </summary>
		/// <returns></returns>
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

		/// <summary>
		/// Gets the immediate child elements for a node id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Gets the direct parents of a given node. Nodes can have multiple parents.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
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
				throw new ArgumentException(@"Element id does not exist.", "id");
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

		/// <summary>
		/// Turns off the element for a given node id. Looks for any active effects in the Live Context we 
		/// are using and clears them on that node. Does not distinguish between effects or who originated them.
		/// </summary>
		/// <param name="id"></param>
		/// <returns>Status</returns>
		public static Status TurnOffElement(Guid id)
		{
			var status = new Status();
			ElementNode node = VixenSystem.Nodes.GetElementNode(id);
			if (node != null)
			{
				Module.LiveContext.TerminateNode(node.Id);
				status.Message = string.Format("{0} turned off.", node.Name);
			}
			else
			{
				throw new ArgumentException(@"Element id is invalid.", "id");
			}
			
			return status;
		}

		/// <summary>
		/// Turns off all the elements for a given list of nodes. Looks for any active effects in the Live Context we 
		/// are using and clears them on that node. Does not distinguish between effects or who originated them.
		/// </summary>
		/// <param name="states"></param>
		/// <returns>Status</returns>
		public static Status TurnOffElements(IEnumerable<ElementState> states)
		{
			var status = new Status();
			
			List<Guid> nodesToRemove = new List<Guid>();
			foreach (var elementState in states)
			{
				ElementNode node = VixenSystem.Nodes.GetElementNode(elementState.Id);
				if (node == null)
				{
					status.Details.Add(string.Format("Element not found. {0}", elementState.Id));
					continue;
				}

				nodesToRemove.Add(node.Id);
				status.Details.Add(string.Format("{0} turned off.", node.Name));
			}

			Module.LiveContext.TerminateNodes(nodesToRemove);
			status.Message = string.Format("{0} elements turned off.", nodesToRemove.Count);

			return status;
		}

		/// <summary>
		/// Clears all current effects on all elements in the live context
		/// </summary>
		/// <returns>Status</returns>
		public static Status ClearActiveEffects()
		{
			var status = new Status();

			Module.LiveContext.Clear();
			status.Message = "Clear active live context effects requested.";

			return status;
		}

		/// <summary>
		/// Turns on a given set of elements with a SetLevel effect given the parameters. If the seconds are zero, the 
		/// effect will be given a duration of 30 days to simulate the element staying on. 
		/// </summary>
		/// <param name="states"></param>
		/// <returns></returns>
		public static Status TurnOnElements(IEnumerable<ElementState> states)
		{
			var status = new Status();
			var effectNodes = new List<EffectNode>();
			var count = 0;
			foreach (var elementState in states)
			{
				ElementNode node = VixenSystem.Nodes.GetElementNode(elementState.Id);
				if (node == null)
				{
					status.Details.Add(string.Format("Element not found. {0}", elementState.Id));
					continue;
				}

				if (elementState.Intensity > 100 || elementState.Intensity < 0)
				{
					elementState.Intensity = 100;
				}

				var effect = CreateEffect(node, elementState.Duration, elementState.Intensity, elementState.Color);

				effectNodes.Add(new EffectNode(effect, TimeSpan.Zero));
				if (elementState.Duration == 0)
				{
					status.Details.Add(string.Format("{0} turned on at {1}% intensity.", node.Name, elementState.Intensity));
				}
				else
				{
					status.Details.Add(string.Format("{0} turned on for {1} seconds at {2}% intensity.", node.Name, elementState.Duration, elementState.Intensity));
				}

				count++;
			}

			Module.LiveContext.Execute(effectNodes);
			status.Message = string.Format("{0} elements turned on.", count);
			return status;
		}

		/// <summary>
		/// Turns on a given element with a SetLevel effect given the parameters. If the seconds are zero, the 
		/// effect will be given a duration of 30 days to simulate the element staying on. 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="seconds"></param>
		/// <param name="intensity"></param>
		/// <param name="color"></param>
		/// <returns></returns>
		public static Status TurnOnElement(Guid id, int seconds, double intensity, string color)
		{
			var status = new Status();
			ElementNode node = VixenSystem.Nodes.GetElementNode(id);
			if (node == null)
			{
				throw new ArgumentException(@"Invalid element id.", "id");
			}

			if (intensity > 100 || intensity < 0)
			{
				intensity = 100;
			}

			var effect = CreateEffect(node, seconds, intensity, color);

			Module.LiveContext.Execute(new EffectNode(effect, TimeSpan.Zero));
			if (seconds == 0)
			{
				status.Message = string.Format("{0} turned on at {1}% intensity.",
				node.Name, intensity);	
			}
			else
			{
				status.Message = string.Format("{0} turned on for {1} seconds at {2}% intensity.",
				node.Name, seconds, intensity);	
			}
			
			return status;
		}

		private static IEffectModuleInstance CreateEffect(ElementNode node, int seconds, double intensity, string color)
		{
			
			if (!string.IsNullOrEmpty(color) && (color.Length != 7 || !color.StartsWith("#")))
			{
				throw new ArgumentException(@"Invalid color", color);
			}

			var effect = new SetLevel
			{
				TimeSpan = seconds > 0 ? TimeSpan.FromSeconds(seconds) : TimeSpan.FromDays(30),
				IntensityLevel = intensity / 100,
				TargetNodes = new[] {node}
			};

			//TODO check the passed color against the element to see if the element supports it
			if (!string.IsNullOrEmpty(color))
			{
				Color elementColor = ColorTranslator.FromHtml(color);
				effect.Color = elementColor;
			}
			return effect;
		}

		/// <summary>
		/// Experimental method to attempt to execute any type of effect. Unused at the moment and needs work.
		/// </summary>
		/// <param name="elementEffect"></param>
		/// <returns></returns>
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
							var cg = new ColorGradient();
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

								var pointPairList = new PointPairList();
								foreach (KeyValuePair<double, double> keyValuePair in elementEffect.LevelCurve)
								{
									pointPairList.Add(keyValuePair.Key, keyValuePair.Value);
								}
								var curve = new Curve(pointPairList);
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
				index++;
			}
				
			
			effectNode.Effect.ParameterValues = newValues;
		}

		private static EffectNode CreateEffectNode(IEffectModuleInstance effectInstance, ElementNode targetNode, TimeSpan timeSpan)
		{
			
			// populate the given effect instance with the appropriate target node and times, and wrap it in an effectNode
			effectInstance.TargetNodes = new[] { targetNode };
			effectInstance.TimeSpan = timeSpan;
			effectInstance.StartTime = TimeSpan.Zero;
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
