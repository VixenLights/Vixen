
#nullable enable
using AsyncAwaitBestPractices;
using Debounce.Core;
using NLog;
using System.ComponentModel;
using Vixen.Attributes;
using Vixen.Sys.Props;
using Vixen.Sys.Props.Components;

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
		private readonly Debouncer _generateDebouncer;

		public Arch() : this("Arch 1", 0)
		{

		}

		public Arch(string name, int nodeCount) : this(name, nodeCount, StringTypes.Pixel)
		{

		}

		public Arch(string name, int nodeCount = 0, StringTypes stringType = StringTypes.Pixel) : base(name, PropType.Arch)
		{
            StringType = stringType;
			ArchModel model = new ArchModel(nodeCount);
			_propModel = model;
			_propModel.PropertyChanged += PropModel_PropertyChanged;
			PropertyChanged += Arch_PropertyChanged;
			
			_generateDebouncer = new Debouncer(() =>
			{
				GenerateElementsAsync().SafeFireAndForget();
			}, 500);
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
                            _generateDebouncer.Debounce();
                            break;
                        case nameof(StartLocation):
                            await UpdatePatchingOrder();
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
		[PropertyOrder(10)]
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
		[PropertyOrder(11)]
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
		[PropertyOrder(12)]
		public ArchStartLocation StartLocation
		{
			get => _startLocation;
			set
			{
				SetProperty(ref _startLocation, value);
			}
		}

		protected async Task GenerateElementsAsync()
		{
			await Task.Factory.StartNew(async () =>
			{
				bool hasUpdated = false;

				var propNode = GetOrCreatePropElementNode();
				if (propNode.IsLeaf)
				{
					AddNodeElements(propNode, NodeCount);
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

					UpdateDefaultPropComponents();
				}

				return true;

			});
		}

		private async Task UpdatePatchingOrder()
		{
			await AddOrUpdatePatchingOrder(StartLocation == ArchStartLocation.Left ? Props.StartLocation.BottomLeft : Props.StartLocation.BottomRight);
		}

		#region PropComponents

		private void UpdateDefaultPropComponents()
		{
			var head = GetOrCreatePropElementNode();
			
			//Update the left and right to match the new node count
			var propComponentLeft = PropComponents.FirstOrDefault(x => x.Name == $"{Name} Left");
			var propComponentRight = PropComponents.FirstOrDefault(x => x.Name == $"{Name} Right");

			if (propComponentLeft == null)
			{
				propComponentLeft = new PropComponent($"{Name} Left", PropComponentType.PropDefined);
				PropComponents.Add(propComponentLeft);
			}
			else
			{
				propComponentLeft.Clear();
			}

			if (propComponentRight == null)
			{
				propComponentRight = new PropComponent($"{Name} Right", PropComponentType.PropDefined);
				PropComponents.Add(propComponentRight);
			}
			else
			{
				propComponentRight.Clear();
			}

			int middle =  (int)Math.Round(NodeCount / 2d, MidpointRounding.AwayFromZero);
			int i = 0;
			foreach (var stringNode in head.Children)
			{
				if (i < middle)
				{
					propComponentLeft.TryAdd(stringNode);
				}
				else
				{
					propComponentRight.TryAdd(stringNode);
				}

				i++;
			}
		}

		#endregion

	}

	public enum ArchStartLocation
	{
		Left,
		Right
	}
}