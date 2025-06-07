
#nullable enable
using System.ComponentModel;
using System.Drawing;
using NLog;
using Vixen.Attributes;
using Vixen.Sys;
using Vixen.Sys.Props;
using VixenModules.Property.Color;

namespace VixenModules.App.Props.Models.Arch
{
	/// <summary>
	/// A class that defines an Arch Prop
	/// </summary>
	public class Arch : BaseProp, IProp
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private ArchModel _propModel;

		public Arch() : this("Arch 1", 25)
		{

		}

		public Arch(string name, int nodeCount) : this(name, nodeCount, StringTypes.Pixel)
		{

		}

		public Arch(string name, int nodeCount = 25, StringTypes stringType = StringTypes.Pixel) : base(name, PropType.Arch)
		{
			//TODO create default element structure
			StringType = stringType;
			ArchModel model = new ArchModel(nodeCount);
			_propModel = model;
			_propModel.PropertyChanged += PropModel_PropertyChanged;
			PropertyChanged += Arch_PropertyChanged;
			// Create default element structure
			_ = GenerateElementsAsync();
		}

		private async void Arch_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != null)
			{
				switch (e.PropertyName)
				{
					case nameof(StringType):
						await GenerateElementsAsync();
						break;
					case nameof(StartLocation):
						AddOrUpdatePatchingOrder();
						break;
				}
			}
		}

		private async void PropModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != null)
			{
				switch (e.PropertyName)
				{
					case nameof(NodeCount):
						await UpdateNodesPerString();
						break;
					case nameof(StartLocation):
						AddOrUpdatePatchingOrder();
						break;
				}
			}
		}

		[Browsable(false)]
		IPropModel IProp.PropModel => PropModel;

		[Browsable(false)]
		public new ArchModel PropModel
		{
			get => _propModel;
			protected set => SetProperty(ref _propModel, value);
		}

		[DisplayName("Nodes Count")]
		[PropertyOrder(0)]
		public int NodeCount
		{
			get => _propModel.NodeCount;
			set
			{
				if (value == _propModel.NodeCount)
				{
					return;
				}

				_propModel.NodeCount = value;
				OnPropertyChanged(nameof(NodeCount));
			}
		}

		[DisplayName("Nodes Size")]
		[PropertyOrder(1)]
		public int NodeSize
		{
			get => _propModel.NodeSize;
			set
			{
				if (value == _propModel.NodeSize)
				{
					return;
				}

				_propModel.NodeSize = value;
				OnPropertyChanged(nameof(NodeSize));
			}
		}

		protected async Task<bool> GenerateElementsAsync()
		{
			ElementNode? head = GetOrCreatePropElementNode();

			AddNodeElements(head, NodeCount);

			PropertySetupHelper.AddOrUpdatePatchingOrder(head, StartLocation.BottomLeft, false);

			await AddOrUpdateColorHandling();

			return true;

		}

		private async Task AddOrUpdateColorHandling(IElementNode? node = null)
		{
			try
			{
				var propNode = node ?? GetOrCreatePropElementNode();
				// TODO Get this info from the Tree properties at some point
				var cf = GetColorConfiguration();
				await PropertySetupHelper.AddOrUpdateColorHandling(propNode, cf);
			}
			catch (Exception e)
			{
				Logging.Error(e, "Error occured updating the color handling");
			}
		}

        private ColorConfiguration GetColorConfiguration()
        {
            // TODO Get this info from the Arch properties at some point
			ColorConfiguration cf = new()
            {
                ColorType = StringType == StringTypes.Pixel
                    ? ElementColorType.FullColor
                    : ElementColorType.SingleColor,
                FullColorOrder = StringType == StringTypes.Pixel ? "RGB" : String.Empty,
                SingleColor = StringType == StringTypes.Standard ? Color.Red : Color.Empty
            };
            return cf;
        }

        private void AddOrUpdatePatchingOrder()
		{
			var propNode = GetOrCreatePropElementNode();
			PropertySetupHelper.AddOrUpdatePatchingOrder(propNode, StartLocation.BottomLeft, false);
		}

		private async Task UpdateNodesPerString()
		{
			var propNode = GetOrCreatePropElementNode();

			var existingNodes = propNode.Children.Count();
			if (existingNodes == NodeCount) return;

			if (existingNodes < NodeCount)
			{
				var newNodes = AddNodeElements(propNode, NodeCount - existingNodes, existingNodes);
                await PropertySetupHelper.AddOrUpdateColorHandling(newNodes, GetColorConfiguration());
            }
			else
			{
				RemoveElements(propNode, existingNodes - NodeCount);
			}

            PropertySetupHelper.AddOrUpdatePatchingOrder(propNode, StartLocation.BottomLeft, false);
		}
	}
}