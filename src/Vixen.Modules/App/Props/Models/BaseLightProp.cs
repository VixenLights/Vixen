#nullable enable
using AsyncAwaitBestPractices;
using Common.WPFCommon.Converters;
using Debounce.Core;
using System.ComponentModel;
using System.Drawing;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Props;
using Vixen.Sys.Props.Model;
using VixenModules.App.Curves;
using VixenModules.Property.Color;

namespace VixenModules.App.Props.Models
{
	public abstract class BaseLightProp<TModel> : BaseProp<TModel> 
		where TModel : BaseLightModel, IPropModel, new()	
	{
		protected bool UpdateInProgress = false;

		protected BaseLightProp(string name, PropType propType) : base(name, propType)
		{
			GenerateDebouncer = new Debouncer(() =>
			{
				GenerateElementsAsync().SafeFireAndForget();
			}, 500);

			StringType = StringTypes.ColorMixingRGB;
			Brightness = 100;
			Gamma = 1.0;
			SingleColorOption = System.Drawing.Color.RoyalBlue;				
		}

		#region Public Properties
		
		/// <summary>
		/// Gets or sets the type of string used in the light prop.
		/// </summary>
		/// <value>
		/// A value of the <see cref="StringTypes"/> enumeration that specifies the string type.
		/// </value>
		/// <remarks>
		/// The <see cref="StringType"/> property determines the behavior and configuration of the light prop:
		/// <list type="bullet">
		/// <item>
		/// <description><see cref="StringTypes.ColorMixingRGB"/> indicates that the string supports full-color pixels.</description>
		/// </item>
		/// <item>
		/// <description><see cref="StringTypes.SingleColor"/> indicates that the string supports single-color lights.</description>
		/// </item>
		/// </list>
		/// </remarks>
		private StringTypes _stringType;
		public StringTypes StringType
		{
			get => _stringType;
			set => SetProperty(ref _stringType, value);
		}
		
		public int LightSize
		{
			get => PropModel.LightSize;
			set
			{
				if (value == PropModel.LightSize)
				{
					return;
				}

				PropModel.LightSize = value;
				OnPropertyChanged(nameof(LightSize));
			}
		}

		private Curve _curve;
		public Curve Curve
		{
			get => _curve;
			set
			{
				_curve = value;
				OnPropertyChanged(nameof(Curve));
			}
		}

		private int _brightness;
		public int Brightness
		{
			get => _brightness;
			set
			{
				_brightness = value;
				OnPropertyChanged(nameof(Brightness));
			}
		}

		private double _gamma;
		public double Gamma
		{
			get => _gamma;
			set
			{
				_gamma = value;
				OnPropertyChanged(nameof(Gamma));
			}
		}

		private Color _singleColorOption;
		public Color SingleColorOption
		{
			get => _singleColorOption;
			set
			{
				_singleColorOption = value;
				OnPropertyChanged(nameof(SingleColorOption));
			}
		}

		private string _selectedColorSet;
		public string SelectedColorSet
		{
			get => _selectedColorSet;
			set
			{
				_selectedColorSet = value;
				OnPropertyChanged(nameof(SelectedColorSet));
			}
		}

		#endregion

		#region Protected Properties

		/// <summary>
		/// Utility for deferring element generation for the prop.
		/// </summary>		
		protected Debouncer GenerateDebouncer { get; set; }

		#endregion

		#region Protected Methods

		protected override void Prop_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != null)
			{
				switch (e.PropertyName)
				{
					case nameof(StringType):
						GenerateDebouncer.Debounce();
						break;
					case nameof(LightSize):
						PropModel.UpdatePropNodes();						
						break;
				}
			}
		}
		
		/// <summary>
		/// Allows derived light props to generate or update the elements associated with the prop.
		/// </summary>
		/// <returns>Task to generate elements</returns>
		protected abstract Task GenerateElementsAsync();

		/// <summary>
		/// Adds a specified number of string elements to the given element node.
		/// </summary>
		/// <param name="node">The parent <see cref="ElementNode"/> to which the string elements will be added.</param>
		/// <param name="count">The number of string elements to add.</param>
		/// <param name="nodesPerString">The number of nodes to include in each string element.</param>
		/// <param name="namingIndex">
		/// The starting index for naming the string elements. Defaults to 0.
		/// Each string element will be named using the format "<c>AutoPropStringName X</c>",
		/// where <c>X</c> is the index starting from <paramref name="namingIndex"/>.
		/// </param>
		protected void AddStringElements(ElementNode node, int count, int nodesPerString, int namingIndex = 0)
		{
			if (count == 0) return;
			for (int i = namingIndex; i < count + namingIndex; i++)
			{
				string stringName = $"{AutoPropStringName} {i + 1}";
				ElementNode stringNode = ElementNodeService.Instance.CreateSingle(node, stringName, true, false);

				if (StringType == StringTypes.ColorMixingRGB)
				{
					AddNodeElements(stringNode, nodesPerString);
				}
			}
		}

		/// <summary>
		/// Updates the number of child nodes for the specified prop node to match the given count.
		/// </summary>
		/// <param name="nodeCount">The desired number of child nodes for the prop.</param>
		/// <param name="propNode">
		/// The <see cref="ElementNode"/> representing the prop whose child nodes are to be updated. 
		/// If <c>null</c>, the method retrieves or creates the prop's element node.
		/// </param>
		/// <exception cref="InvalidOperationException">
		/// Thrown if the specified <paramref name="propNode"/> is a leaf node or has a maximum child depth greater than 1.
		/// </exception>
		/// <remarks>
		/// This method adjusts the number of child nodes by either adding or removing nodes to match the specified 
		/// <paramref name="nodeCount"/>. It ensures that color handling is updated for the new nodes. If the operation 
		/// is already in progress, it waits until the current update completes.
		/// </remarks>
		/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
		protected async Task UpdateStringNodeCount(int nodeCount, ElementNode? propNode = null)
		{
			propNode ??= GetOrCreateElementNode();

			if (propNode.IsLeaf || propNode.GetMaxChildDepth() > 2)
			{
				throw new InvalidOperationException("Prop does not have direct leaf children");
			}

			while (UpdateInProgress)
			{
				await Task.Delay(100);
			}
			UpdateInProgress = true;

			var existingNodes = propNode.Children.Count();
			if (existingNodes == nodeCount) return;

			if (existingNodes < nodeCount)
			{
				var newNodes = AddNodeElements(propNode, nodeCount - existingNodes, existingNodes);
				PropertySetupHelper.AddOrUpdateColorHandling(newNodes, GetColorConfiguration());
			}
			else
			{
				RemoveElementNodes(propNode, existingNodes - nodeCount);
			}

			UpdateInProgress = false;
		}

		/// <summary>
		/// Updates the number of nodes for each string element under the specified parent node.
		/// Ensures that the structure of the parent node adheres to the expected hierarchy: 
		/// <c>propNode -> Child Group(s) -> Leafs</c>.
		/// </summary>
		/// <param name="nodesPerString">
		/// The desired number of nodes for each string element under the parent node.
		/// </param>
		/// <param name="propNode">
		/// The parent <see cref="ElementNode"/> representing the prop whose child nodes are to be updated.
		/// If <c>null</c>, the method retrieves or creates the default prop element node.
		/// </param>
		/// <returns>
		/// A <see cref="Task"/> representing the asynchronous operation.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// Thrown if the <paramref name="propNode"/> is a leaf node or its maximum child depth exceeds 2.
		/// </exception>
		protected async Task UpdateNodesPerString(int nodesPerString, ElementNode? propNode = null)
		{
			propNode ??= GetOrCreateElementNode();

			if (propNode.IsLeaf || propNode.GetMaxChildDepth() > 3)
			{
				throw new InvalidOperationException("Prop does not have direct leaf children");
			}

			while (UpdateInProgress)
			{
				await Task.Delay(100);
			}
			UpdateInProgress = true;

			foreach (var propString in propNode.Children.ToList())
			{
				var existingNodes = propString.Children.Count();
				if (existingNodes == nodesPerString) continue;

				if (existingNodes < nodesPerString)
				{
					var newNodes = AddNodeElements(propString, nodesPerString - existingNodes, existingNodes);
					PropertySetupHelper.AddOrUpdateColorHandling(newNodes, GetColorConfiguration());
				}
				else
				{
					RemoveElementNodes(propString, existingNodes - nodesPerString);
				}
			}

			UpdateInProgress = false;

		}

		/// <summary>
		/// Updates the number of string elements in the specified <see cref="ElementNode"/> to match the desired count.
		/// </summary>
		/// <param name="strings">
		/// The desired number of string elements to be present in the <paramref name="propNode"/>.
		/// </param>
		/// <param name="propNode">
		/// The <see cref="ElementNode"/> representing the parent node whose child string elements are to be updated.
		/// If <c>null</c>, the method will retrieve or create the default prop element node.
		/// </param>
		/// <remarks>
		/// This method ensures that the number of child string elements in the <paramref name="propNode"/> matches the specified count.
		/// If the current count is less than the desired count, new string elements are added. If the count exceeds the desired count,
		/// excess string elements are removed. The method uses the <see cref="UpdateInProgress"/> flag to ensure sequential updates
		/// and avoid conflicts.
		/// </remarks>
		/// <returns>
		/// A <see cref="Task"/> representing the asynchronous operation.
		/// </returns>
		protected async Task UpdateStrings(int strings, ElementNode? propNode = null)
		{
			propNode ??= GetOrCreateElementNode();

			while (UpdateInProgress)
			{
				await Task.Delay(500);
			}
			UpdateInProgress = true;

			var existingStrings = propNode.Children.Count();
			if (existingStrings == strings) return;

			if (existingStrings < strings)
			{
				var nodesPerString = existingStrings > 0 ? propNode.Children.First().Count() : 0;
				AddStringElements(propNode, strings - existingStrings, nodesPerString, existingStrings);
			}
			else
			{
				RemoveElementNodes(propNode, existingStrings - strings);
			}

			UpdateInProgress = false;
		}

		/// <summary>
		/// Retrieves the color configuration for the current light prop.
		/// </summary>
		/// <returns>
		/// A <see cref="ColorConfiguration"/> object that specifies the color type, 
		/// full color order, and single color based on the <see cref="StringType"/> of the prop.
		/// </returns>
		/// <remarks>
		/// The color configuration is determined by the <see cref="StringType"/> property:
		/// <list type="bullet">
		/// <item>
		/// <description>If the <see cref="StringType"/> is <see cref="StringTypes.ColorMixingRGB"/>, the color type is set to <see cref="ElementColorType.FullColor"/>, and the full color order is "RGB".</description>
		/// </item>
		/// <item>
		/// <description>If the <see cref="StringType"/> is <see cref="StringTypes.SingleColor"/>, the color type is set to <see cref="ElementColorType.SingleColor"/>, and the single color is set to red.</description>
		/// </item>
		/// </list>
		/// </remarks>
		protected ColorConfiguration GetColorConfiguration()
		{
			// TODO Get color order info from the Prop properties at some point
			ColorConfiguration cf = new()
			{
				ColorType = StringType == StringTypes.ColorMixingRGB
					? ElementColorType.FullColor
					: ElementColorType.SingleColor,
				FullColorOrder = StringType == StringTypes.ColorMixingRGB ? "RGB" : String.Empty,
				SingleColor = StringType == StringTypes.SingleColor ? Color.Red : Color.Empty
			};
			return cf;
		}

		/// <summary>
		/// Adds or updates the patching order for the current prop based on the specified parameters.
		/// </summary>
		/// <param name="startLocation">
		/// The starting location for the patching order. This determines where the patching begins.
		/// </param>
		/// <param name="zigZag">
		/// A boolean value indicating whether the patching order should follow a zig-zag pattern.
		/// </param>
		/// <param name="zigZagOffset">
		/// The offset value for the zig-zag pattern. This specifies how many elements are skipped before reversing the direction.
		/// </param>
		/// <remarks>
		/// This method ensures that the patching order is updated in a thread-safe manner by preventing concurrent updates.
		/// It utilizes the <see cref="PropertySetupHelper.AddOrUpdatePatchingOrder"/> method to apply the patching order.
		/// </remarks>
		/// <returns>
		/// A task representing the asynchronous operation.
		/// </returns>
		protected virtual async Task AddOrUpdatePatchingOrder(StartLocation startLocation = StartLocation.BottomLeft, bool zigZag = false,
			int zigZagOffset = 0)

		{
			while (UpdateInProgress)
			{
				await Task.Delay(100);
			}

			UpdateInProgress = true;
			var propNode = GetOrCreateElementNode();
			PropertySetupHelper.AddOrUpdatePatchingOrder(propNode, startLocation, zigZag, zigZagOffset);

			UpdateInProgress = false;
		}

		/// <summary>
		/// Asynchronously adds or updates the color handling configuration for the specified element node.
		/// </summary>
		/// <param name="node">
		/// The <see cref="IElementNode"/> to which the color handling configuration should be applied. 
		/// If <c>null</c>, the method will retrieve or create the default property element node.
		/// </param>
		/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
		/// <remarks>
		/// This method ensures that only one update operation is performed at a time by utilizing a locking mechanism.
		/// If an exception occurs during the update process, it is logged for debugging purposes.
		/// </remarks>
		protected async Task AddOrUpdateColorHandling(IElementNode? node = null)
		{
			while (UpdateInProgress)
			{
				await Task.Delay(100);
			}

			UpdateInProgress = true;
			try
			{
				var propNode = node ?? GetOrCreateElementNode();
				PropertySetupHelper.AddOrUpdateColorHandling(propNode, GetColorConfiguration());
			}
			catch (Exception e)
			{
				Logging.Error(e, "Error occured updating the color handling");
			}

			UpdateInProgress = false;
		}

		/// <summary>
		/// Get the HTML summary of all the Dimming parameter values
		/// </summary>
		/// <returns>Returns the <see cref="string"/> summary in HTML format</returns>
		protected string GetDimmingSummary()
		{
			string summary = 
				"<h2>Brightness Level</h2>" +
				"<body>" +
				$"<b>Maximum Brightness:</b> {Brightness}%<br>" +
				$"<b>Gamma:</b> {Gamma:0.0}" +
				"</body>";

			return summary;
		}

		/// <summary>
		/// Get the HTML summary of all the Color parameter values
		/// </summary>
		/// <returns>Returns the <see cref="string"/> summary in HTML format</returns>
		protected string GetColorSummary()
		{
			string summary = "<h2>Light Coloring</h2><body>" +
							$"<b>Light Type:</b> {EnumValueTypeConverter.GetDescription(StringType)}<br>";

			if (StringType == StringTypes.SingleColor)
			{
				summary += "<b>Single Color:</b><ul style=\"margin-top: 0;\">" +
					       $"<li>Red is {SingleColorOption.R}</li>" + 
						   $"<li>Green is {SingleColorOption.G}</li>" +
						   $"<li>Blue is {SingleColorOption.B}</li></ul>";
			}

			else if (StringType == StringTypes.MultiColor)
			{
				summary += $"<b>Multiple Colors:</b> {SelectedColorSet}";
			}

			else if (StringType == StringTypes.ColorMixingRGB)
			{
				summary += $"<b>RGB Colors:</b> {SelectedColorSet}";
			}

			summary += "</body>";

			return summary;
		}

		#endregion
	}
}
