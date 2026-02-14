
#nullable enable
using AsyncAwaitBestPractices;
using Debounce.Core;
using Microsoft.VisualBasic;
using NLog;
using System.ComponentModel;
using System.Runtime.Serialization;
using Vixen.Attributes;
using Vixen.Sys.Managers;
using Vixen.Sys.Props;
using Vixen.Sys.Props.Components;
using VixenModules.App.Props.Models.IntelligentFixture;
using VixenModules.App.Props.Models.IntellligentFixture;
using VixenModules.App.Props.Models.Tree;
using VixenModules.Editor.FixtureGraphics;

namespace VixenModules.App.Props.Models.Arch
{
	/// <summary>
	/// A class that defines an Intelligent Fixture Prop
	/// </summary>
	public class IntelligentFixtureProp : BaseProp<IntelligentFixtureModel>, IProp
	{				
		//private ArchStartLocation _startLocation;
		//private readonly Debouncer _generateDebouncer;

		public IntelligentFixtureProp() : base("Fixture 1", PropType.IntelligentFixture) //, 0)
		{
			GetOrCreateElementNode();

			PropModel = new IntelligentFixtureModel();			
		}

		public override string GetSummary()
		{
			return null;
		}

		/*

		public Arch(string name, int nodeCount) : this(name, nodeCount, StringTypes.Pixel)
		{

		}

		public Arch(string name, int nodeCount = 0, StringTypes stringType = StringTypes.Pixel) : base(name, PropType.Arch)
		{
            StringType = stringType;
			ArchModel model = new ArchModel(nodeCount);
			PropModel = model;
			PropModel.PropertyChanged += PropModel_PropertyChanged;
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
			get => PropModel;
			protected set => SetProperty(ref PropModel, value);
		}

		[DisplayName("Nodes Count")]
		[PropertyOrder(10)]
		public int NodeCount
		{
			get => PropModel.NodeCount;
			set
			{
				if (value == PropModel.NodeCount)
				{
					return;
				}

				PropModel.NodeCount = value;
				OnPropertyChanged(nameof(NodeCount));
			}
		}

		[DisplayName("Nodes Size")]
		[PropertyOrder(11)]
		public int NodeSize
		{
			get => PropModel.NodeSize;
			set
			{
				if (value == PropModel.NodeSize)
				{
					return;
				}

				PropModel.NodeSize = value;
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
	*/

		/// <summary>
		/// Minimum Strobe Rate display name.
		/// </summary>
		private const string MinimumStrobeRateDisplayName = "Minimum Strobe Rate (ms)";

		/// <summary>
		/// Maximum Strobe Rate display name.
		/// </summary>
		private const string MaximumStrobeRateDisplayName = "Maximum Strobe Rate (Hz)";

		/// <summary>
		/// Pan Start Position display name.
		/// </summary>
		private const string PanStartDisplayName = "Pan Start Position (Degrees)";

		/// <summary>
		/// Pan Stop Position display name.
		/// </summary>
		private const string PanStopDisplayName = "Pan Stop Position (Degrees)";

		/// <summary>
		/// Tilt Start display name.
		/// </summary>
		private const string TiltStartDisplayName = "Tilt Start Position (Degrees)";

		/// <summary>
		/// Tilt Stop display name.
		/// </summary>
		private const string TiltStopDisplayName = "Tilt Stop Position (Degrees)";

		/// <summary>
		/// Minimum color wheel rotation speed in seconds.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Color Wheel"),
		 Description("The time it takes the color wheel to make a complete rotation in seconds."),
		 DisplayName("Color Wheel Rotation Speed Minimum (s)")]
		public double MinColorWheelRotationSpeed 
		{ 
			get
			{
				return PropModel.MinColorWheelRotationSpeed;
			}
			set
			{
				PropModel.MinColorWheelRotationSpeed = value;

				OnPropertyChanged(nameof(MinColorWheelRotationSpeed));
			}
		}

		/// <summary>
		/// Maximum color wheel rotation speed in seconds.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Color Wheel"),
		 Description("The time it takes the color wheel to make a complete rotation in seconds."),
		 DisplayName("Color Wheel Rotation Speed Maximum (s)")]
		public double MaxColorWheelRotationSpeed 
		{ 
			get
			{
				return PropModel.MaxColorWheelRotationSpeed;
			}
			set
			{
				PropModel.MaxColorWheelRotationSpeed = value;
				OnPropertyChanged(nameof(MaxColorWheelRotationSpeed));
			}
		}

		/// <summary>
		/// Maximum pan travel time in seconds.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Pan"),
		 Description("The time it takes of the intelligent fixture to pan from the starting position to the maximum stop position."),
		 DisplayName("Maximum Pan Travel Time (s)")]
		public double MaxPanTravelTime 
		{ 
			get
			{
				return PropModel.MaxPanTravelTime;
			}
			set
			{
				PropModel.MaxPanTravelTime = value;
				OnPropertyChanged(nameof(MaxPanTravelTime));
			}
		}

		/// <summary>
		/// Maximum tilt travel time in seconds.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Tilt"),
		 Description("The time it takes of the intelligent fixture to tilt from the starting position to the maximum stop position."),
		 DisplayName("Maximum Tilt Travel Time (s)")]
		public double MaxTiltTravelTime 
		{ 
			get
			{
				return PropModel.MaxTiltTravelTime;
			}
			set
			{
				PropModel.MaxTiltTravelTime = value;
				OnPropertyChanged(nameof(MaxTiltTravelTime));
			}
		}

		/// <summary>
		/// Strobe rate minimum in Hz.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		Category("Strobe"),
		Description("The strobe rate minimum (in Hz)."),
		DisplayName(MinimumStrobeRateDisplayName)]
		public int StrobeRateMinimum 
		{ 
			get
			{
				return PropModel.StrobeRateMinimum;
			}
			set
			{
				PropModel.StrobeRateMinimum = value;
				OnPropertyChanged(nameof(StrobeRateMinimum));
			}
		}

		/// <summary>
		/// Strobe rate maximum in Hz.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Strobe"),
		 Description("The strobe rate maximum (in Hz)."),
		 DisplayName(MaximumStrobeRateDisplayName)]
		public int StrobeRateMaximum 
		{ 
			get
			{
				return PropModel.StrobeRateMaximum;
			}
			set
			{
				PropModel.StrobeRateMaximum = value;
				OnPropertyChanged(nameof(StrobeRateMaximum));
			}
		}

		/// <summary>
		/// Strobe duration in ms.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Strobe"),
		 Description("The maximum strobe duration in ms."),
		 DisplayName("Maximum Strobe Duration (ms)")]
		public int MaximumStrobeDuration 
		{ 
			get
			{
				return PropModel.MaximumStrobeDuration;
			}
			set
			{
				PropModel.MaximumStrobeDuration = value;
				OnPropertyChanged(nameof(MaximumStrobeDuration));
			}
		}

		/// <summary>
		/// Pan start position in degrees.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Pan"),
		 Description("The pan starting point angle (in degrees)."),
		 DisplayName(PanStartDisplayName)]
		public int PanStartPosition 
		{ 
			get
			{
				return PropModel.PanStartPosition;
			}
			set
			{
				PropModel.PanStartPosition = value;
				OnPropertyChanged(nameof(PanStartPosition));
			}
		}

		/// <summary>
		/// Pan stop position in degrees.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Pan"),
		 Description("The pan stopping point angle (in degrees)."),
		 DisplayName("Pan Stop Position (Degrees)")]
		public int PanStopPosition 
		{ 
			get
			{
				return PropModel.PanStopPosition;
			}
			set
			{
				PropModel.PanStopPosition = value;
				OnPropertyChanged(nameof(PanStopPosition));
			}
		}

		/// <summary>
		/// Tilt start position in degrees.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Tilt"),
		 Description("The tilt starting point angle (in degrees)."),
		 DisplayName(TiltStartDisplayName)]
		public int TiltStartPosition 
		{ 
			get
			{
				return PropModel.TiltStartPosition;
			}
			set
			{
				PropModel.TiltStartPosition = value;
				OnPropertyChanged(nameof(TiltStartPosition));
			}
		}

		/// <summary>
		/// Tilt stop position in degrees.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Tilt"),
		 Description("The tilt stopping point angle (in degrees)."),
		 DisplayName(TiltStopDisplayName)]
		public int TiltStopPosition 
		{ 
			get
			{
				return PropModel.TiltStopPosition;
			}
			set
			{
				PropModel.TiltStopPosition = value;
				OnPropertyChanged(nameof(TiltStopPosition));
			}
		}

		/// <summary>
		/// Beam length scale factor (1-100%).
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Beam"),
		 Description("Length of the beam as a percentage of the background height."),
		 DisplayName("Beam Length")]
		public int BeamLength
		{
			get
			{
				return PropModel.BeamLength;
			}
			set
			{
				PropModel.BeamLength = value;
				OnPropertyChanged(nameof(BeamLength));	
			}
		}

		/// <summary>
		/// Beam transparecny (1-100%).
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Beam"),
		 Description("Determines the transparency of the light beam. 0% is completely opaque.  100% is completely transparent.  Note at 100% transparency the beam is not visible."),
		 DisplayName("Beam Transparency (%)")]
		public int BeamTransparency
		{
			get
			{
				return PropModel.BeamTransparency;
			}
			set
			{
				PropModel.BeamTransparency = value;
				OnPropertyChanged(nameof(BeamTransparency));
			}
		}

		/// <summary>
		/// Beam width multiplier.  Determines the width at the top of the beam.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Beam"),
		 Description("Beam Width Multiplier"),
		 DisplayName("Beam Width Multiplier")]
		public int BeamWidthMultiplier
		{
			get
			{
				return PropModel.BeamWidthMultiplier;
			}
			set
			{
				PropModel.BeamWidthMultiplier = value;
				OnPropertyChanged(nameof(BeamWidthMultiplier));
			}
		}

		/// <summary>
		/// Flag that indicates if zoom increases from narrow to wide.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Settings"),
		 Description("Indicates whether the fixture zooms from a narrow beam to a wide beam or vice-versa"),
		 DisplayName("Zoom Narrow To Wide")]
		public bool ZoomNarrowToWide
		{
			get
			{
				return PropModel.ZoomNarrowToWide;
			}
			set
			{
				PropModel.ZoomNarrowToWide = value;
				OnPropertyChanged(nameof(ZoomNarrowToWide));
			}
		}

		/// <summary>
		/// Flag that controls whether the function index legend is displayed.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Settings"),
		 Description("When true enables a legend that will show a function label and the corresponding channel value.  The legend is only applicable to index and range functions that were populated with a Preview Legend character."),
		 DisplayName("Show Legend")]
		public bool ShowLegend
		{
			get
			{
				return PropModel.ShowLegend;
			}
			set
			{
				PropModel.ShowLegend = value;
				OnPropertyChanged(nameof(ShowLegend));
			}
		}
		
		[DataMember(EmitDefaultValue = false),
		 Category("Pan"),
		 Description("Changes the start point of the pan by 180 degrees and inverts the direction of movement.  This setting is often used with 'Top' (upside down) mounting position."),
		 DisplayName("Invert Pan Direction")]
		public YesNoType InvertPanDirection
		{
			get
			{
				return PropModel.InvertPanDirection;
			}
			set
			{
				PropModel.InvertPanDirection = value;
				OnPropertyChanged(nameof(InvertPanDirection));
			}
		}
		
		[DataMember(EmitDefaultValue = false),
		 Category("Tilt"),
		 Description("Swaps the start position with the stop position and inverts the direction of movement.  This setting is often used with 'Top' (upside down) mounting position."),
		 DisplayName("Invert Tilt Direction")]
		public YesNoType InvertTiltDirection
		{
			get
			{
				return PropModel.InvertTiltDirection;
			}
			set
			{
				PropModel.InvertTiltDirection = value;
				OnPropertyChanged(nameof(InvertTiltDirection));
			}
		}

		[DataMember(EmitDefaultValue = false),
		 Category("Settings"),
		 Description("Selects the mounting position of the fixture.  This property allows for simulating the fixture being mounted upside down."),
		 DisplayName("Mounting Position")]
		public MountingPositionType MountingPosition
		{
			get
			{
				return PropModel.MountingPosition;
			}
			set
			{
				PropModel.MountingPosition = value;
				OnPropertyChanged(nameof(MountingPosition));
			}
		}
	}
}