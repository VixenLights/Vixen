using System.Drawing;
using NLog;
using Vixen.Data.Flow;
using Vixen.Module.OutputFilter;
using Vixen.Services;
using Vixen.Sys;
using VixenModules.OutputFilter.ColorBreakdown;
using VixenModules.Property.Color;
using VixenModules.Property.Order;

namespace VixenModules.App.Props.Models
{
	internal class PropertySetupHelper
	{
        private static readonly Logger Logging = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Adds or Updates color handling for the top level node provided
		/// </summary>
		/// <param name="node"></param>
		/// <param name="colorConfiguration"></param>
		/// <returns></returns>
		public static void AddOrUpdateColorHandling(IElementNode node, ColorConfiguration colorConfiguration)
        {
            var leafElements = node.GetLeafEnumerator().Distinct().ToList();
            AddOrUpdateColorHandling(leafElements, colorConfiguration);
        }

        /// <summary>
        /// Adds or Updates color handling on the specified leaf elements.
        /// </summary>
        /// <param name="leafElementList">The leaf elements to add color handling to</param>
        /// <param name="colorConfiguration"></param>
        public static void AddOrUpdateColorHandling(IEnumerable<IElementNode> leafElementList, ColorConfiguration colorConfiguration)
        {
           
            if (!colorConfiguration.IsValid())
            {
                Logging.Error("Invalid color config");
                throw new ArgumentOutOfRangeException(nameof(colorConfiguration));
            }
            // PROPERTY SETUP
            // go through all elements, making a color property for each one.
            // (If any has one already, check with the user as to what they want to do.)
           
            foreach (IElementNode leafElement in leafElementList)
            {
                ColorModule existingProperty = null;

                if (leafElement.Properties.Contains(ColorDescriptor.ModuleId))
                {
                    existingProperty = leafElement.Properties.Get(ColorDescriptor.ModuleId) as ColorModule;
                }
                else
                {
                    existingProperty = leafElement.Properties.Add(ColorDescriptor.ModuleId) as ColorModule;
                }


                if (existingProperty == null)
                {
                    Logging.Error("Null color property for element " + leafElement.Name);
                }
                else
                {
                    existingProperty.ColorType = colorConfiguration.ColorType;
                    existingProperty.SingleColor = colorConfiguration.SingleColor;
                    existingProperty.ColorSetName = colorConfiguration.ColorSetName;
                }
            }

			PerformPatching(leafElementList, colorConfiguration);

		}

        private static void PerformPatching(IEnumerable<IElementNode> leafElementList, ColorConfiguration colorConfiguration)
        {
			List<IDataFlowComponentReference> leafOutputs = new List<IDataFlowComponentReference>();
			foreach (IElementNode leafElement in leafElementList.Where(x => x.Element != null))
			{
				leafOutputs.AddRange(ColorSetupHelper.FindLeafOutputsOrBreakdownFilters(VixenSystem.DataFlow.GetComponent(leafElement.Element.Id)));
			}

			bool overrideExistingFilters = true;
			ColorBreakdownModule breakdown = null;

			foreach (IDataFlowComponentReference leaf in leafOutputs)
			{
				bool skip = false;

				if (leaf.Component is ColorBreakdownModule module)
				{
                    skip = !overrideExistingFilters;
					breakdown = module;
				}
				else if (leaf.Component.OutputDataType == DataFlowType.None)
				{
					// if it's a dead-end -- ie. most likely a controller output -- skip it
					skip = true;
				}
				else
				{
					// doesn't exist? make a new module and assign it
					breakdown =
						ApplicationServices.Get<IOutputFilterModuleInstance>(ColorBreakdownDescriptor.ModuleId) as ColorBreakdownModule;
					VixenSystem.DataFlow.SetComponentSource(breakdown, leaf);
					VixenSystem.Filters.AddFilter(breakdown);
				}

				if (!skip)
				{
					List<ColorBreakdownItem> newBreakdownItems = new List<ColorBreakdownItem>();
					bool mixColors = false;
					ColorBreakdownItem cbi;

					switch (colorConfiguration.ColorType)
					{
						case ElementColorType.FullColor:
							mixColors = true;

							foreach (var color in colorConfiguration.FullColorOrder.ToCharArray())
							{
								switch (color)
								{
									case 'R':
										cbi = new ColorBreakdownItem
                                        {
                                            Color = System.Drawing.Color.Red,
                                            Name = ColorSetupHelper.Red
                                        };
                                        newBreakdownItems.Add(cbi);
										break;
									case 'G':
										cbi = new ColorBreakdownItem
                                        {
                                            Color = System.Drawing.Color.Lime,
                                            Name = ColorSetupHelper.Green
                                        };
                                        newBreakdownItems.Add(cbi);
										break;
									case 'B':
										cbi = new ColorBreakdownItem
                                        {
                                            Color = System.Drawing.Color.Blue,
                                            Name = ColorSetupHelper.Blue
                                        };
                                        newBreakdownItems.Add(cbi);
										break;
									case 'W':
										cbi = new ColorBreakdownItem
                                        {
                                            Color = System.Drawing.Color.White,
                                            Name = ColorSetupHelper.White
                                        };
                                        newBreakdownItems.Add(cbi);
										break;
								}
							}
							break;

						case ElementColorType.MultipleDiscreteColors:
							mixColors = false;

							ColorStaticData csd = ApplicationServices.GetModuleStaticData(ColorDescriptor.ModuleId) as ColorStaticData;

							if (!csd.ContainsColorSet(colorConfiguration.ColorSetName))
							{
								Logging.Error("Color sets doesn't contain " + colorConfiguration.ColorSetName);
							}
							else
							{
								ColorSet cs = csd.GetColorSet(colorConfiguration.ColorSetName);
								foreach (var c in cs)
								{
									cbi = new ColorBreakdownItem
                                    {
                                        Color = c,
                                        // heh heh, this can be.... creative.
                                        Name = c.Name
                                    };
                                    newBreakdownItems.Add(cbi);
								}
							}

							break;

						case ElementColorType.SingleColor:
							mixColors = false;
							cbi = new ColorBreakdownItem
                            {
                                Color = colorConfiguration.SingleColor
                            };
                            newBreakdownItems.Add(cbi);
							break;
					}

					breakdown.MixColors = mixColors;
					breakdown.BreakdownItems = newBreakdownItems;

				}
			}

		}
        public static void AddOrUpdatePatchingOrder(ElementNode propNode, StartLocation startLocation, bool zigZag, int zigZagOffset = 0)
        {
            IEnumerable<ElementNode> leafNodes = [];

            if (startLocation == StartLocation.BottomLeft)
            {
                leafNodes = propNode.GetLeafEnumerator();
                if (zigZag)
                {
                    OrderModule.AddUpdatePatchingOrder(leafNodes, zigZagOffset);
                    return;
                }
            }

            if (startLocation == StartLocation.BottomRight)
            {
                leafNodes = propNode.Children.SelectMany(x => x.GetLeafEnumerator()).Reverse();
            }
            else if (startLocation == StartLocation.TopLeft)
            {
                leafNodes = propNode.Children.Reverse().SelectMany(x => x.GetLeafEnumerator());
            }
            else if (startLocation == StartLocation.TopRight)
            {
                leafNodes = propNode.GetLeafEnumerator().Reverse();
            }

            if (zigZag)
            {
                OrderModule.AddUpdatePatchingOrder(leafNodes, zigZagOffset);
            }
            else
            {
                OrderModule.AddUpdatePatchingOrder(leafNodes);
            }

        }
	}

    public class ColorConfiguration
    {
        private static char[] _colorSet = ['R', 'G', 'B', 'W'];
        /// <summary>
        /// The color handling approach.
        /// </summary>
        public ElementColorType ColorType { get; set; }

        /// <summary>
        /// Color set name to use for multiple discrete colors.
        /// Required when ColorType is MultipleDiscreteColors
        /// </summary>
        public string ColorSetName { get; set; } = string.Empty;

        /// <summary>
        /// Color to use for single discrete color
        /// Required when ColorType is SingleColor
        /// </summary>
        public Color SingleColor { get; set; } = Color.Empty;

        /// <summary>
        /// String value of the primitive color orders.
        /// Required when ColorType is FullColor.
        /// ex. RGB, RGBW, GRB, BRG, etc.
        /// </summary>
        public string FullColorOrder { get; set; }

        public bool IsValid()
        {
            if (ColorType == ElementColorType.FullColor)
            {
                if (FullColorOrder == String.Empty) return false;
                return FullColorOrder.All(x => char.IsLetter(x) && _colorSet.Contains(x));
            }

            if (ColorType == ElementColorType.SingleColor && SingleColor == Color.Empty)
            {
                return false;
            }

            if (ColorType == ElementColorType.MultipleDiscreteColors)
            {
                if (ColorSetName == string.Empty) return false;
                ColorStaticData csd =
                    ApplicationServices.GetModuleStaticData(ColorDescriptor.ModuleId) as ColorStaticData;

                if (csd != null && !csd.ContainsColorSet(ColorSetName))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
