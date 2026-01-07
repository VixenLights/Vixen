
#nullable enable
using AsyncAwaitBestPractices;
using Debounce.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Vixen.Attributes;
using Vixen.Sys.Managers;
using Vixen.Sys.Props;
using Vixen.Sys.Props.Components;
using Vixen.Sys.Props.Model;
using VixenModules.App.Curves;
using VixenModules.App.Props.Models;

namespace VixenModules.App.Props.Models.Arch
{
	public interface IAttributeData
	{
		int NodeCount { get; set; }
		int LightSize { get; set; }
		ArchStartLocation ArchWiringStart { get; set; }
		bool LeftRight { get; set; }
		public ObservableCollection<AxisRotationViewModel> Rotations { get; set; }
	}

	/// <summary>
	/// A class that defines an Arch Prop
	/// </summary>
	[CategoryOrder("Attributes", 1)]
	[CategoryOrder("Rotation", 40)]
	[CategoryOrder("Dimming Curve", 50)]
	[CategoryOrder("Creation", 100)]
	public class Arch : BaseLightProp<ArchModel>, IProp, IAttributeData
	{
		private readonly Debouncer _generateDebouncer;

		public Arch() : this("Arch 1", 0)
		{
		}

		public Arch(string name, int nodeCount) : this(name, nodeCount, StringTypes.Pixel)
		{

		}

		public Arch(string name, int nodeCount = 0, StringTypes stringType = StringTypes.Pixel) : base(name, PropType.Arch)
		{
			Name = name;
			NodeCount = nodeCount;
			StringType = stringType;
			PropertyChanged += Arch_PropertyChanged;

			// Create Preview model
			ArchModel model = new ArchModel();
			model.SetContext(this);
			PropModel = model;

			_generateDebouncer = new Debouncer(() =>
			{
				GenerateElementsAsync().SafeFireAndForget();
			}, 500);
		}

		private void Arch_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			try
			{
				if (e.PropertyName != null)
				{
					switch (e.PropertyName)
					{
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Error(ex, $"An error occured handling Arch property {e.PropertyName} changed");
			}

		}

		private int _nodeCount;
		[Category("Attributes")]
		[DisplayName("Nodes Count")]
		[PropertyOrder(10)]
		public int NodeCount
		{
			get => _nodeCount;
			set
			{
				_nodeCount = value;
				_generateDebouncer?.Debounce();
				PropModel?.UpdatePropNodes();
				OnPropertyChanged(nameof(NodeCount));
			}
		}

		private int _lightSize;
		[Category("Attributes")]
		[DisplayName("Light Size")]
		[PropertyOrder(11)]
		public int LightSize
		{
			get => _lightSize;
			set
			{
				_lightSize = value;
				PropModel?.UpdatePropNodes();
				OnPropertyChanged(nameof(LightSize));
			}
		}

		private ArchStartLocation _archWiringStart;
		[Category("Attributes")]
		[DisplayName("Wiring Start")]
		[PropertyOrder(12)]
		public ArchStartLocation ArchWiringStart
		{
			get => _archWiringStart;
			set
			{
				_archWiringStart = value;
				UpdatePatchingOrder().SafeFireAndForget(); ;
				OnPropertyChanged(nameof(ArchWiringStart));
			}
		}

		private ObservableCollection<AxisRotationViewModel> _rotations;
		[Browsable(false)]
		public ObservableCollection<AxisRotationViewModel> Rotations
		{
			get => _rotations;
			set
			{
				_rotations = value;
				PropModel?.UpdatePropNodes();
				OnPropertyChanged(nameof(Rotations));
			}
		}

		[Category("Rotation")]
		[DisplayName("X\u00B0")]
		[PropertyOrder(1)]
		public int XRotation
		{
			get
			{
				var rotation = Rotations?.FirstOrDefault(x => x.Axis == "X");
				if (rotation != null)
				{
					return rotation.RotationAngle;
				}
				return 0;
			}
			set
			{
				var _rotations = Rotations;
				_rotations[(int)Axis.XAxis].RotationAngle = value;
				Rotations = _rotations;
			}
		}

		[Category("Rotation")]
		[DisplayName("Y\u00B0")]
		[PropertyOrder(2)]
		public int YRotation
		{
			get
			{
				var rotation = Rotations?.FirstOrDefault(y => y.Axis == "Y");
				if (rotation != null)
				{
					return rotation.RotationAngle;
				}
				return 0;
			}
			set
			{
				var _rotations = Rotations;
				_rotations[(int)Axis.YAxis].RotationAngle = value;
				Rotations = _rotations;
			}
		}

		[Category("Rotation")]
		[DisplayName("Z\u00B0")]
		[PropertyOrder(3)]
		public int ZRotation
		{
			get
			{
				var rotation = Rotations?.FirstOrDefault(z => z.Axis == "Z");
				if (rotation != null)
				{
					return rotation.RotationAngle;
				}
				return 0;
			}
			set
			{
				var _rotations = Rotations;
				_rotations[(int)Axis.ZAxis].RotationAngle = value;
				Rotations = _rotations;
			}
		}

		private bool _leftRight;
		[Browsable(false)]
		public bool LeftRight
		{
			get => _leftRight;
			set
			{
				_leftRight = value;
				OnPropertyChanged(nameof(LeftRight));
			}
		}

		private Curve _curve;
		[Browsable(false)]
		public Curve Curve
		{
			get { return _curve; }
			set { _curve = value; }
		}

		[Category("Dimming Curve")]
		[DisplayName("Curve")]
		[PropertyOrder(30)]
		public string CurveName
		{
			get
			{
				string result = "None Specified";
				if (_curve != null)
				{
					if (_curve.CustomReferenceName != string.Empty)
					{
						result = _curve.CustomReferenceName;
					}
					else if (_curve.LibraryReferenceName != string.Empty)
					{
						result = _curve.LibraryReferenceName;
					}
					else if (_curve.Points.Count > 0)
					{
						result = "Custom";
					}
				}

				return result;
			}
		}

		protected async Task GenerateElementsAsync()
		{
			await Task.Factory.StartNew(async () =>
			{
				bool hasUpdated = false;

				var propNode = GetOrCreateElementNode();
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
			await AddOrUpdatePatchingOrder(ArchWiringStart == ArchStartLocation.Left ? Props.StartLocation.BottomLeft : Props.StartLocation.BottomRight);
		}

		#region PropComponents

		private void UpdateDefaultPropComponents()
		{
			var head = GetOrCreateElementNode();
			if (!PropComponents.Any())
			{
				var propComponent = PropComponentManager.CreatePropComponent($"{Name} Nodes", Id, PropComponentType.PropDefined);
				foreach (var stringNode in head.Children)
				{
					propComponent.TryAdd(stringNode);
				}
				PropComponents.Add(propComponent);
			}

			//Add new nodes, if any
			else if (head.Count() > PropComponents.First().TargetNodes.Count())
			{
				foreach (var node in head.Children.Except(PropComponents.First().TargetNodes))
				{
					PropComponents.First().TryAdd(node);
				}
			}
			else
			{
				//Remove nodes that no longer exist, if any
				foreach (var node in PropComponents.First().TargetNodes.Except(head.Children).ToList())
				{
					PropComponents.First().TryRemove(node.Id, out _);
				}
			}

			if (LeftRight == true)
			{
				//Update the left and right to match the new node count
				var propComponentLeft = PropComponents.FirstOrDefault(x => x.Name == $"{Name} Left");
				var propComponentRight = PropComponents.FirstOrDefault(x => x.Name == $"{Name} Right");

				if (propComponentLeft == null)
				{
					propComponentLeft = PropComponentManager.CreatePropComponent($"{Name} Left", Id, PropComponentType.PropDefined);
					PropComponents.Add(propComponentLeft);
				}
				else
				{
					propComponentLeft.Clear();
				}

				if (propComponentRight == null)
				{
					propComponentRight = PropComponentManager.CreatePropComponent($"{Name} Right", Id, PropComponentType.PropDefined);
					PropComponents.Add(propComponentRight);
				}
				else
				{
					propComponentRight.Clear();
				}

				int middle = (int)Math.Round(NodeCount / 2d, MidpointRounding.AwayFromZero);
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
		}
		#endregion
	}

	public enum ArchStartLocation
	{
		Left,
		Right
	}
}