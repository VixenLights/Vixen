
#nullable enable
using NLog;
using System.ComponentModel;
using Vixen.Attributes;
using Vixen.Sys.Props;

namespace VixenModules.App.Props.Models.Arch
{
	/// <summary>
	/// A class that defines an Arch Prop
	/// </summary>
	public class Arch : BaseLightProp, IProp
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private ArchModel _propModel;
		private ArchStartLocation _startLocation;

		public Arch() : this("Arch 1", 25)
		{

		}

		public Arch(string name, int nodeCount) : this(name, nodeCount, StringTypes.Pixel)
		{

		}

		public Arch(string name, int nodeCount = 25, StringTypes stringType = StringTypes.Pixel) : base(name, PropType.Arch)
		{
            StringType = stringType;
			ArchModel model = new ArchModel(nodeCount);
			_propModel = model;
			_propModel.PropertyChanged += PropModel_PropertyChanged;
			PropertyChanged += Arch_PropertyChanged;
			// Create default element structure
			_ = GenerateElementsAsync();

			//TODO Map element structure to model nodes
		}

		private async void Arch_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName != null)
                {
                    switch (e.PropertyName)
                    {
                        case nameof(StringType):
                            await GenerateElementsAsync();
                            break;
                        case nameof(StartLocation):
                            await UpdatePatchingOrder(); //The defaults are fine for an Arch
							break;
                    }
                }
            }
            catch (Exception ex)
            {
				Logging.Error(ex, $"An error occured handling Arch property {e.PropertyName} changed");
			}
        }

		private async void PropModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName != null)
                {
                    switch (e.PropertyName)
                    {
                        case nameof(NodeCount):
	                        await GenerateElementsAsync();
                            break;
                        case nameof(StartLocation):
                            await UpdatePatchingOrder(); //The defaults are fine for an Arch
							break;
                    }
                }
            }
            catch (Exception ex)
            {
				Logging.Error(ex, $"An error occured handling model property {e.PropertyName} changed");
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

		[DisplayName("Wiring Start")]
		[PropertyOrder(2)]
		public ArchStartLocation StartLocation
		{
			get => _startLocation;
			set
			{
				SetProperty(ref _startLocation, value);
			}
		}

		protected async Task<bool> GenerateElementsAsync()
		{
			bool hasUpdated = false;

			var propNode = GetOrCreatePropElementNode();
			if (propNode.IsLeaf)
			{
				AddNodeElements(propNode,NodeCount);
				hasUpdated = true;
			}
			else if (propNode.Children.Count() != NodeCount)
			{
				await UpdateStringNodeCount(NodeCount);
				hasUpdated = true;
			}

			if (hasUpdated)
			{
				await UpdatePatchingOrder();

				await AddOrUpdateColorHandling();
			}

			return true;

		}

		private async Task UpdatePatchingOrder()
		{
			await AddOrUpdatePatchingOrder(StartLocation == ArchStartLocation.Left ? Props.StartLocation.BottomLeft : Props.StartLocation.BottomRight);
		}
	}

	public enum ArchStartLocation
	{
		Left,
		Right
	}
}