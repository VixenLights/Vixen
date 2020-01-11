using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Vixen.Attributes;
using Vixen.Marks;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using ZedGraph;

namespace VixenModules.Effect.Liquid
{
	/// <summary>
	/// Maintains an emitter.
	/// </summary>
	[ExpandableObject]
	public class Emitter : ExpandoObjectBase, IEmitter
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public Emitter()
		{
			ParticleType = ParticleType.Water;
			UseColorArray = false;
			FramesPerColor = 3;
			FrameColorCounter = 0;
			ColorArrayIndex = 0;
			Color = new ColorGradient(System.Drawing.Color.White);
			Brightness = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			Lifetime = new Curve(CurveType.Flat100);			
			ParticleVelocity = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			Animate = false;
			RandomStartingPosition = true;
			AnimateXStart = 0;
			AnimateYStart = 0;
			EdgeHandling = EdgeHandling.Bounce;
			VelocityX = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			VelocityY = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			// X and Y are used when the emitter is not animated
			X = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			Y = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			
			Flow = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			SourceSize = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
			NozzleAngle = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 75.0, 75.0 }));
			
			OscillateStartAngle = 0;
			OscillateEndAngle = 90;
			OscillationSpeed = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 5.0, 5.0 }));

			FlowControl = FlowControl.Continuous;			
			MarkNameCollection = new ObservableCollection<string>();

			OnTime = 2000;  // Milliseconds
			OffTime = 2000; // Milliseconds

			// The following properties are not persisted and are used for rendering
			VelX = 1;
			VelY = 1;
			On = true;
			IntAngle = 5;
			AngleOscillationDelta = 5;
			OscillationClockwise = true;

			InitAllAttributes();
		}

		#endregion
		
		#region Private Methods
		
		/// <summary>
		/// Gets the mark collection with the specified Guid.
		/// </summary>		
		private IMarkCollection GetMarkCollection(Guid markCollectionGuid)
		{
			IMarkCollection markCollection = null;

			// If the mark collection contains the specified Guid then...
			if (MarkCollections.Any(item => item.Id == markCollectionGuid))
			{
				// Find the specified mark collection
				markCollection = MarkCollections.First(item => item.Id == markCollectionGuid);
			}

			return markCollection;
		}

		/// <summary>
		/// Updates the browseable state of the color list.
		/// </summary>
		private void UpdateColorListAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
			{
				{nameof(FramesPerColor), UseColorArray},
				{nameof(Color), !UseColorArray }
			};
			SetBrowsable(propertyStates);
		}

		/// <summary>
		/// Updates the browseable state of properties related to emitter animation.
		/// </summary>
		private void UpdateAnimateAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(5)
			{
				{nameof(VelocityX), Animate},
				{nameof(VelocityY), Animate},
				{nameof(EdgeHandling), Animate},
				{nameof(X), (!Animate)},
				{nameof(Y), (!Animate)},
				{nameof(AnimateXStart), (Animate && !RandomStartingPosition)},
				{nameof(AnimateYStart), (Animate && !RandomStartingPosition)},
				{nameof(RandomStartingPosition), Animate}
			};
			SetBrowsable(propertyStates);
		}

		/// <summary>
		/// Updates the browseable state of properties related to whether the emitter's
		/// starting position is random or not.
		/// </summary>
		private void UpdateRandomStartingPositionAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(5)
			{
				{ nameof(AnimateXStart), !RandomStartingPosition},
				{ nameof(AnimateYStart), !RandomStartingPosition},
			};
			SetBrowsable(propertyStates);
		}

			/// <summary>
			/// Updates the browseable state of properties related to the nozzle angle.
			/// </summary>
			private void UpdateNozzleAngleAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{nameof(NozzleAngle), NozzleMovement == NozzleMovement.FixedAngle},
				{nameof(OscillateStartAngle), NozzleMovement == NozzleMovement.Oscillate},
				{nameof(OscillateEndAngle), NozzleMovement == NozzleMovement.Oscillate},
				{nameof(OscillationSpeed), (NozzleMovement == NozzleMovement.Oscillate ||
													 NozzleMovement == NozzleMovement.SpinClockwise ||
													 NozzleMovement == NozzleMovement.SpinCounterClockwise) }
			};
			SetBrowsable(propertyStates);
		}

		/// <summary>
		/// Updates the browseable state of properties related to the emitter flow.
		/// </summary>
		private void UpdateEmitterFlowAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(4)
			{
				{nameof(OnTime), FlowControl == FlowControl.Pulsating},
				{nameof(OffTime), FlowControl == FlowControl.Pulsating},
				{nameof(Flow), (FlowControl == FlowControl.Pulsating ||
									 FlowControl == FlowControl.Continuous) },
				{nameof(MarkCollectionName), FlowControl == FlowControl.UseMarks}
			};
			SetBrowsable(propertyStates);
		}

		#endregion

		#region IEmitter

		/// <summary>
		/// Returns a clone of the emitter.
		/// </summary>		
		public IEmitter CreateInstanceForClone()
		{
			IEmitter result = new Emitter
			{
				Parent = Parent,
				ParticleType = ParticleType,
				UseColorArray = UseColorArray,
				FramesPerColor = FramesPerColor,
				FrameColorCounter = FrameColorCounter,
				Color = new ColorGradient(Color),
				Brightness = new Curve(Brightness),
				Lifetime = new Curve(Lifetime),
				ParticleVelocity = new Curve(ParticleVelocity),
				Animate = Animate,
				RandomStartingPosition = RandomStartingPosition,
				AnimateXStart = AnimateXStart,
				AnimateYStart = AnimateYStart,
				EdgeHandling = EdgeHandling,
				VelocityX = new Curve(VelocityX),
				VelocityY = new Curve(VelocityY),
				X = new Curve(X),
				Y = new Curve(Y),				
				Flow = new Curve(Flow),
				SourceSize = new Curve(SourceSize),
				NozzleMovement = NozzleMovement,
				NozzleAngle = new Curve(NozzleAngle),				
				OscillateStartAngle = OscillateStartAngle,
				OscillateEndAngle = OscillateEndAngle,
				OscillationSpeed = OscillationSpeed,

				FlowControl = FlowControl,				
				MarkCollections = MarkCollections,
				MarkCollectionId = MarkCollectionId,				
				MarkNameCollection = MarkNameCollection,				
				OnTime = OnTime,
				OffTime = OffTime,
				On = On,
				IntAngle = IntAngle,
				AngleOscillationDelta = AngleOscillationDelta,				
			};

			return result;
		}

		/// <summary>
		/// The Emitter needs the Parent reference to register for Mark events.
		/// </summary>
		[Browsable(false)]
		public BaseEffect Parent { get; set; }

		private ParticleType _particleType;

		/// <summary>
		/// Type of particles emitted by the emitter.
		/// </summary>
		[ProviderDisplayName(@"ParticleType")]
		[ProviderDescription(@"ParticleType")]
		[PropertyOrder(0)]
		public ParticleType ParticleType
		{
			get
			{
				return _particleType;
			}
			set
			{
				_particleType = value;
				OnPropertyChanged();
			}
		}

		private bool _useColorArray = false;

		/// <summary>
		/// Controls whether the color array is used to color the emitter particles.
		/// </summary>
		[ProviderDisplayName(@"UseColorList")]
		[ProviderDescription(@"UseColorList")]
		[PropertyOrder(1)]
		public bool UseColorArray
		{
			get
			{
				return _useColorArray;
			}
			set
			{
				_useColorArray = value;	
				UpdateColorListAttributes();
				OnPropertyChanged();
			}
		}

		private int _framesPerColor;

		/// <summary>
		/// Controls how many frames of each color in the color array are emitted.
		/// </summary>
		[ProviderDisplayName(@"FramesPerColor")]
		[ProviderDescription(@"FramesPerColor")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1,60,1)]
		[PropertyOrder(2)]
		public int FramesPerColor
		{
			get
			{
				return _framesPerColor;
			}
			set
			{
				_framesPerColor = value;
				OnPropertyChanged();				
			}
		}

		/// <summary>
		/// Color of the emitter.
		/// </summary>
		[ProviderDisplayName(@"ParticleColor")]
		[ProviderDescription(@"ParticleColor")]
		[PropertyOrder(3)]
		public ColorGradient Color { get; set; }

		/// <summary>
		/// Brightness of the emitter color.
		/// </summary>
		[ProviderDisplayName(@"EmitterBrightness")]
		[ProviderDescription(@"EmitterBrightness")]
		[PropertyOrder(4)]
		public Curve Brightness { get; set; }

		/// <summary>
		/// Lifetime of the emitter particles.
		/// </summary>
		[ProviderDisplayName(@"ParticleLifetime")]
		[ProviderDescription(@"ParticleLifetime")]
		[PropertyOrder(5)]
		public Curve Lifetime { get; set; }

		/// <summary>
		/// Velocity of the particles.
		/// </summary>
		[ProviderDisplayName(@"ParticleVelocity")]
		[ProviderDescription(@"ParticleVelocity")]
		[PropertyOrder(6)]
		public Curve ParticleVelocity { get; set; }

		private bool _animate = false;

		/// <summary>
		/// Whether the emitter is animated.
		/// </summary>
		[ProviderDisplayName(@"Animate")]
		[ProviderDescription(@"Animate")]
		[PropertyOrder(7)]
		public bool Animate
		{
			get
			{
				return _animate;
			}
			set
			{
				_animate = value;
				UpdateAnimateAttributes();
				OnPropertyChanged();
			}
		}

		private bool _randomStartingPosition = true;

		/// <summary>
		/// Whether the starting position of an animated emitter is random.
		/// </summary>
		[ProviderDisplayName(@"RandomStartingPosition")]
		[ProviderDescription(@"RandomStartingPosition")]
		[PropertyOrder(8)]
		public bool RandomStartingPosition 
		{ 
			get
			{
				return _randomStartingPosition;
			}
			set
			{
				_randomStartingPosition = value;
				OnPropertyChanged();
				UpdateRandomStartingPositionAttributes();
			}
		}

		private int _animateXStart;

		/// <summary>
		/// Starting X position of the emitter.  Only applicable to animated emitters.
		/// </summary>
		[ProviderDisplayName(@"AnimateXStart")]
		[ProviderDescription(@"AnimateXStart")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(9)]
		public int AnimateXStart 
		{ 
			get
			{
				return _animateXStart;
			}
			set
			{
				_animateXStart = value;
				OnPropertyChanged();
			}
		}

		private int _animateYStart;

		/// <summary>
		/// Starting Y position of the emitter.  Only applicable to animated emitters.
		/// </summary>
		[ProviderDisplayName(@"AnimateYStart")]
		[ProviderDescription(@"AnimateYStart")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(10)]
		public int AnimateYStart
		{ 
			get
			{
				return _animateYStart;
			}
			set
			{
				_animateYStart = value;
				OnPropertyChanged();
			}
		}

		private EdgeHandling _edgeHandling;

		/// <summary>
		/// How the emitter behaves when animated and it reachs the edge of the element.
		/// </summary>
		[ProviderDisplayName(@"EdgeHandling")]
		[ProviderDescription(@"EdgeHandling")]
		[PropertyOrder(11)]
		public EdgeHandling EdgeHandling
		{
			get
			{
				return _edgeHandling;
			}
			set
			{
				_edgeHandling = value;
				OnPropertyChanged();			
			}
		}

		/// <summary>
		/// Velocity of the emitter in the X direction.
		/// </summary>
		[ProviderDisplayName(@"VelocityX")]
		[ProviderDescription(@"VelocityX")]
		[PropertyOrder(12)]
		public Curve VelocityX { get; set; }

		/// <summary>
		/// Velocity of the emittter in the Y direction.
		/// </summary>
		[ProviderDisplayName(@"VelocityY")]
		[ProviderDescription(@"VelocityY")]
		[PropertyOrder(13)]
		public Curve VelocityY { get; set; }

		/// <summary>
		/// Manual position of the emitter in the X direction.
		/// </summary>
		[ProviderDisplayName(@"XPosition")]
		[ProviderDescription(@"XPosition")]
		[PropertyOrder(14)]
		public Curve X { get; set; }

		/// <summary>
		/// Manual position of the emitter in the Y direction.
		/// </summary>
		[ProviderDisplayName(@"YPosition")]
		[ProviderDescription(@"YPosition")]
		[PropertyOrder(15)]
		public Curve Y { get; set; }
				
		/// <summary>
		/// Source size of the emitter.
		/// </summary>
		[ProviderDisplayName(@"NozzleSize")]
		[ProviderDescription(@"NozzleSize")]
		[PropertyOrder(16)]
		public Curve SourceSize { get; set; }

		private NozzleMovement _nozzleMovement = NozzleMovement.FixedAngle;

		/// <summary>
		/// Controls how the nozzle (angle) is positioned.
		/// </summary>
		[ProviderDisplayName(@"NozzleMovement")]
		[ProviderDescription(@"NozzleMovement")]
		[PropertyOrder(17)]
		public NozzleMovement NozzleMovement
		{
			get
			{
				return _nozzleMovement;
			}
			set
			{
				_nozzleMovement = value;
				UpdateNozzleAngleAttributes();
				OnPropertyChanged();				
			}
		}

		/// <summary>
		/// Angle of the emitter's nozzle.
		/// </summary>
		[ProviderDisplayName(@"NozzleAngle")]
		[ProviderDescription(@"NozzleAngle")]
		[PropertyOrder(18)]
		public Curve NozzleAngle { get; set; }
				
		private int _oscillateStartAngle;

		/// <summary>
		/// Start angle in degrees of the emitter oscillation.
		/// </summary>
		[ProviderDisplayName(@"OscillateStartAngle")]
		[ProviderDescription(@"OscillateStartAngle")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 360, 1)]
		[PropertyOrder(19)]
		public int OscillateStartAngle
		{
			get
			{
				return _oscillateStartAngle;
			}
			set
			{
				_oscillateStartAngle = value;
				OnPropertyChanged();
			}
		}

		private int _oscillateEndAngle;

		/// <summary>
		/// Start angle in degrees of the emitter oscillation.
		/// </summary>
		[ProviderDisplayName(@"OscillateEndAngle")]
		[ProviderDescription(@"OscillateEndAngle")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 360, 1)]
		[PropertyOrder(20)]
		public int OscillateEndAngle
		{
			get
			{
				return _oscillateEndAngle;
			}
			set
			{
				_oscillateEndAngle = value;
				OnPropertyChanged();				
			}
		}

		/// <summary>
		/// Speed of rotation of the emitter nozzle when rotating.
		/// </summary>
		[ProviderDisplayName(@"NozzleSpeed")]
		[ProviderDescription(@"NozzleSpeed")]
		[PropertyOrder(21)]
		public Curve OscillationSpeed { get; set; }

		private FlowControl _flowControl = FlowControl.Continuous;

		/// <summary>
		/// Determines how the flow is controlled for the emitter (Continuous, Pulsating, Use Marks).
		/// </summary>
		[ProviderDisplayName(@"FlowControl")]
		[ProviderDescription(@"FlowControl")]
		[PropertyOrder(22)]
		public FlowControl FlowControl
		{
			get
			{
				return _flowControl;
			}
			set
			{
				_flowControl = value;				
				UpdateEmitterFlowAttributes();
				OnPropertyChanged();				
			}
		}

		/// <summary>
		/// Manual flow of the emitter.
		/// </summary>
		[ProviderDisplayName(@"Flow")]
		[ProviderDescription(@"Flow")]
		[PropertyOrder(23)]
		public Curve Flow { get; set; }
				
		/// <summary>
		/// Updates the selected mark collection drop down.
		/// This method allows for the associated mark collection to be renamed.
		/// </summary>
		public void UpdateSelectedMarkCollectionName()
		{	
			// If the selected mark collection no longer exists then...
			if (MarkCollections != null &&
				!MarkCollections.Any(mc => mc.Id == _markCollectionId))
			{				
				// Make the mark collection drop down blank
				MarkCollectionId = new Guid();
			}

			OnPropertyChanged("MarkCollections");
			OnPropertyChanged("MarkCollectionName");
		}

		/// <summary>
		/// Name of the selected mark collection.
		/// </summary>
		[ProviderDisplayName(@"MarkCollection")]
		[ProviderDescription(@"EmitterMarkCollection")]
		[TypeConverter(typeof(Emitters.EmitterMarkCollectionNameConverter))]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(24)]
		public string MarkCollectionName
		{
			get
			{
				// Default the mark collection name to the empty string
				string markNameCollection = string.Empty;

				if (MarkCollections != null)
				{
					IMarkCollection selectedMarkcollection = GetMarkCollection(_markCollectionId);
					
					if (selectedMarkcollection != null)
					{
						markNameCollection = selectedMarkcollection.Name;
					}

				}
				return markNameCollection;
			}
			set
			{
				// Find the selected mark collection
				IMarkCollection selectedMarkCollection = MarkCollections.FirstOrDefault(mc => mc.Name == value);

				// If the mark collection was found then...
				if (selectedMarkCollection != null)
				{
					// Save off the Guid of the selected mark collection
					_markCollectionId = selectedMarkCollection.Id;
				}
				else
				{
					// Otherwise set the selected Guid to the emtpy Guid
					_markCollectionId = new Guid();
				}

				OnPropertyChanged();
			}
		}
		
		private ObservableCollection<string> _markNameCollection = null;

		/// <summary>
		/// Collection of the mark collection names.
		/// </summary>
		[Browsable(false)]
		public ObservableCollection<string> MarkNameCollection
		{
			get
			{
				return _markNameCollection;
			}
			set
			{
				_markNameCollection = value;
				OnPropertyChanged();				
			}
		}

		private ObservableCollection<IMarkCollection> _markCollections;

		/// <summary>
		/// Collection of the mark collections.
		/// </summary>
		[Browsable(false)]
		public ObservableCollection<IMarkCollection> MarkCollections
		{
			get
			{
				return _markCollections;
			}
			set
			{
				_markCollections = value;
								
				OnPropertyChanged();				
			}
		}
		
		Guid _markCollectionId = new Guid();

		/// <summary>
		/// Guid of the selected mark collection.
		/// </summary>
		[Browsable(false)]
		public Guid MarkCollectionId
		{
			get
			{
				return _markCollectionId;			
			}
			set
			{
				if (MarkCollections != null)
				{
					IMarkCollection oldSelectedMarkCollection = GetMarkCollection(_markCollectionId);

					if (oldSelectedMarkCollection != null)
					{
						// Unregister for events
						Parent.RemoveMarkCollectionListeners(oldSelectedMarkCollection);
					}
				}

				// Store off the new selected mark collection
				_markCollectionId = value;

				if (MarkCollections != null)
				{
					IMarkCollection selectedMarkCollection = GetMarkCollection(_markCollectionId);

					if (selectedMarkCollection != null)
					{
						// Register for events from the selected mark collection
						Parent.AddMarkCollectionListeners(selectedMarkCollection);
					}
				}

				OnPropertyChanged();
				OnPropertyChanged("MarkCollectionName");
			}
		}
		
		private int _onTime = 0;

		/// <summary>
		/// On Time for the emitter in seconds.
		/// </summary>
		[ProviderDisplayName(@"EmitterOnTime")]
		[ProviderDescription(@"EmitterOnTime")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(50, 10000, 50)]
		[PropertyOrder(22)]
		public int OnTime
		{
			get
			{
				return _onTime;
			}
			set
			{
				_onTime = value;
				OnPropertyChanged();				
			}
		}

		private int _offTime = 0;

		/// <summary>
		/// Off time for the emitter in seconds.
		/// </summary>
		[ProviderDisplayName(@"EmitterOffTime")]
		[ProviderDescription(@"EmitterOffTime")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(50, 10000, 50)]
		[PropertyOrder(23)]
		public int OffTime
		{
			get
			{
				return _offTime;
			}
			set
			{
				_offTime = value;
				OnPropertyChanged();				
			}
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public void InitAllAttributes()
		{
			UpdateColorListAttributes();
			UpdateAnimateAttributes();			
			UpdateNozzleAngleAttributes();			
			UpdateEmitterFlowAttributes();
			TypeDescriptor.Refresh(this);
		}
				
		#endregion

		#region Public Properties For Rendering

		/// <summary>
		/// Angle of the emitter in integer precision.
		/// </summary>
		[Browsable(false)]
		public int IntAngle { get; set; }

		/// <summary>
		/// True when the emitter is rotating clockwise.
		/// </summary>
		[Browsable(false)]
		public bool OscillationClockwise { get; set; }

		/// <summary>
		/// Degrees to move when the emitter is rotating.
		/// </summary>
		[Browsable(false)]
		public int AngleOscillationDelta { get; set; }

		/// <summary>
		/// Current X location of the emitter.
		/// </summary>
		[Browsable(false)]
		public double LocationX { get; set; }

		/// <summary>
		/// Current Y location of the emitter.
		/// </summary>
		[Browsable(false)]
		public double LocationY { get; set; }

		/// <summary>
		/// Velocity of the emitter in the X axis.
		/// </summary>
		[Browsable(false)]
		public double VelX { get; set; }

		/// <summary>
		/// Velocity of the emitter in the Y axis.
		/// </summary>
		[Browsable(false)]
		public double VelY { get; set; }

		/// <summary>
		/// True when the emitter is On.
		/// </summary>
		[Browsable(false)]
		public bool On { get; set; }

		/// <summary>
		/// Number frames the emitter has been On.
		/// </summary>
		[Browsable(false)]
		public int OnTimer { get; set; }

		/// <summary>
		/// Number of frames the emitter has been Off.
		/// </summary>
		[Browsable(false)]
		public int OffTimer { get; set; }

		/// <summary>
		/// Frame counter for the color array.
		/// This counter determines when it is time to move to the next color in the array.
		/// </summary>
		[Browsable(false)]
		public int FrameColorCounter { get; set; }

		/// <summary>
		/// Current index into the color array.
		/// </summary>
		[Browsable(false)]
		public int ColorArrayIndex { get; set; }

		#endregion

		#region Implementation of ICloneable

		/// <inheritdoc />
		public object Clone()
		{
			return CreateInstanceForClone();
		}

		#endregion
	}
}