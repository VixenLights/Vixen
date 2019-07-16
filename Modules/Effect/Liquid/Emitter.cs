using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Vixen.Attributes;
using Vixen.Marks;
using Vixen.TypeConverters;
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
	public class Emitter : IEmitter, INotifyPropertyChanged
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
			Color = new ColorGradient(System.Drawing.Color.Blue);
			Brightness = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			Lifetime = new Curve(CurveType.Flat100);			
			ParticleVelocity = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			Animate = false;
			EdgeHandling = EdgeHandling.Bounce;
			VelocityX = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			VelocityY = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			// X and Y are used when the emitter is not animated
			X = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			Y = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 90.0, 90.0 }));

			FlowMatchesMusic = false;
			Flow = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			SourceSize = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
			Angle = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 75.0, 75.0 }));

			Oscillate = false;
			OscillateStartAngle = 0;
			OscillateEndAngle = 90;
			OscillationSpeed = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 5.0, 5.0 }));

			OnOff = false;
			UseMarks = false;
			MarkNameCollection = new ObservableCollection<string>();

			OnTime = 2;  // Seconds
			OffTime = 2; // Seconds

			// The following properties are not persisted and are used for rendering
			VelX = 1;
			VelY = 1;
			On = true;
			IntAngle = 5;
			AngleOscillationDelta = 5;
			OscillationClockwise = true;
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
				EdgeHandling = EdgeHandling,
				VelocityX = new Curve(VelocityX),
				VelocityY = new Curve(VelocityY),
				X = new Curve(X),
				Y = new Curve(Y),
				FlowMatchesMusic = FlowMatchesMusic,
				Flow = new Curve(Flow),
				SourceSize = new Curve(SourceSize),
				NozzleAngle = NozzleAngle,
				Angle = new Curve(Angle),
				Oscillate = Oscillate,
				OscillateStartAngle = OscillateStartAngle,
				OscillateEndAngle = OscillateEndAngle,
				OscillationSpeed = OscillationSpeed,

				FlowControl = FlowControl,
				OnOff = OnOff,
				UseMarks = UseMarks,
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
		[ProviderDisplayName(@"Particle Type")]
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
		[ProviderDisplayName(@"Use Color List")]
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
				OnPropertyChanged();
				OnPropertyChanged("UseColor");				
			}
		}

		/// <summary>
		/// This property is the opposite of UseColorArray and is used for binding.		
		/// Property is used by XAML.
		/// </summary>
		[Browsable(false)]
		public bool UseColor
		{
			get
			{
				return !UseColorArray;
			}
			set
			{
				UseColorArray = !value;
				OnPropertyChanged();
			}
		}

		private int _framesPerColor;

		/// <summary>
		/// Controls how many frames of each color in the color array are emitted.
		/// </summary>
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
		public ColorGradient Color { get; set; }

		/// <summary>
		/// Brightness of the emitter color.
		/// </summary>
		public Curve Brightness { get; set; }

		/// <summary>
		/// Lifetime of the emitter particles.
		/// </summary>
		public Curve Lifetime { get; set; }

		/// <summary>
		/// Velocity of the particles.
		/// </summary>
		public Curve ParticleVelocity { get; set; }

		private bool _animate = false;

		/// <summary>
		/// Whether the emitter is animated.
		/// </summary>
		public bool Animate
		{
			get
			{
				return _animate;
			}
			set
			{
				_animate = value;
				OnPropertyChanged();
				OnPropertyChanged("ManualPosition");				
			}
		}

		/// <summary>
		/// Whether the emitter is positioned manually via the X and Y curves.		
		/// Property is used by XAML.
		/// </summary>
		public bool ManualPosition
		{
			get
			{
				return !Animate;
			}
			set
			{
				Animate = !value;
				OnPropertyChanged();
			}
		}

		private EdgeHandling _edgeHandling;

		/// <summary>
		/// How the emitter behaves when animated and it reachs the edge of the element.
		/// </summary>
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
		public Curve VelocityX { get; set; }

		/// <summary>
		/// Velocity of the emittter in the Y direction.
		/// </summary>
		public Curve VelocityY { get; set; }

		/// <summary>
		/// Manual position of the emitter in the X direction.
		/// </summary>
		public Curve X { get; set; }

		/// <summary>
		/// Manual position of the emitter in the Y direction.
		/// </summary>
		public Curve Y { get; set; }

		private bool _flowMatchesMusic = false;

		/// <summary>
		/// Determines whether the emitter flow is determined by the associated music volume.		
		/// Property is used by XAML. 
		/// </summary>
		public bool FlowMatchesMusic
		{
			get
			{
				return _flowMatchesMusic;
			}
			set
			{
				_flowMatchesMusic = value;
				OnPropertyChanged();
				OnPropertyChanged("ManualFlow");
			}
		}

		/// <summary>
		/// Whether the emitter flow is controlled manually by a curve.
		/// </summary>
		public bool ManualFlow
		{
			get
			{
				return !FlowMatchesMusic;
			}
			set
			{
				FlowMatchesMusic = !value;
				OnPropertyChanged();		
			}
		}

		/// <summary>
		/// Manual flow of the emitter.
		/// </summary>
		public Curve Flow { get; set; }

		/// <summary>
		/// Source size of the emitter.
		/// </summary>
		public Curve SourceSize { get; set; }

		private NozzleAngle _nozzleAngle = NozzleAngle.FixedAngle;

		/// <summary>
		/// Controls how the nozzle (angle) is positioned.
		/// </summary>
		public NozzleAngle NozzleAngle
		{
			get
			{
				return _nozzleAngle;
			}
			set
			{
				_nozzleAngle = value;
				OnPropertyChanged();
				OnPropertyChanged("Oscillate");
				OnPropertyChanged("FixedNozzleAngle");
				OnPropertyChanged("RotateNozzle");
			}
		}

		/// <summary>
		/// Angle of the emitter's nozzle.
		/// </summary>
		public Curve Angle { get; set; }

		/// <summary>
		/// True when the nozzle angle is controlled manually via the curve.
		/// </summary>
		public bool FixedNozzleAngle
		{
			get
			{
				return _nozzleAngle == NozzleAngle.FixedAngle;
			}
		}

		/// <summary>
		/// True when the emitter oscillates between a start angle and an end angle.
		/// </summary>
		public bool Oscillate
		{
			get
			{
				return _nozzleAngle == NozzleAngle.Oscillate;
			}
			set
			{			
			}
		}

		/// <summary>
		/// True when the emitter nozzle is rotating.
		/// </summary>
		public bool RotateNozzle
		{
			get
			{
				return (_nozzleAngle == NozzleAngle.SpinClockwise ||
					     _nozzleAngle == NozzleAngle.SpinCounterClockwise ||
						  _nozzleAngle == NozzleAngle.Oscillate);
			}			
		}

		private int _oscillateStartAngle;

		/// <summary>
		/// Start angle in degrees of the emitter oscillation.
		/// </summary>
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
		public Curve OscillationSpeed { get; set; }

		private FlowControl _flowControl = FlowControl.Continuous;

		/// <summary>
		/// Determines how the flow is controlled for the emitter (Continuous, Pulsating, Use Marks).
		/// </summary>
		public FlowControl FlowControl
		{
			get
			{
				return _flowControl;
			}
			set
			{
				_flowControl = value;
				OnPropertyChanged();
				OnPropertyChanged("OnOff");
				OnPropertyChanged("SliderOnOff");
				OnPropertyChanged("UseMarks");
			}
		}

		/// <summary>
		/// True when the emitter toggles On and Off.
		/// </summary>
		public bool OnOff
		{
			get
			{ 
				return FlowControl != FlowControl.Continuous;
			}
			set
			{			
			}
		}

		/// <summary>
		/// True when a mark collection controls when the emitter is On and Off.
		/// </summary>
		public bool UseMarks
		{
			get
			{
				return FlowControl == FlowControl.UseMarks;
			}
			set
			{				
			}
		}

		/// <summary>
		/// True when a slider controls when the emitter is On vs Off.
		/// </summary>
		public bool SliderOnOff
		{
			get
			{
				return !UseMarks && OnOff;
			}
			set
			{
				UseMarks = !value;
				OnPropertyChanged();				
			}
		}

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
		}

		/// <summary>
		/// Name of the selected mark collection.
		/// </summary>
		[ProviderDisplayName(@"MarkCollection")]
		[ProviderDescription(@"MarkCollection")]
		[TypeConverter(typeof(Emitters.EmitterMarkCollectionNameConverter))]
		[PropertyEditor("SelectionEditor")]
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
		/// This property is used by the Marks Combo Box.  This property gives the parent
		/// effect access to the IEmitter that is bound to the drop down.
		/// </summary>
		[Browsable(false)]
		public IEmitter InEdit { get; set; }

		#endregion

		#region Public Properties For Rendering

		/// <summary>
		/// Angle of the emitter in integer precision.
		/// </summary>
		public int IntAngle { get; set; }

		/// <summary>
		/// True when the emitter is rotating clockwise.
		/// </summary>
		public bool OscillationClockwise { get; set; }

		/// <summary>
		/// Degrees to move when the emitter is rotating.
		/// </summary>
		public int AngleOscillationDelta { get; set; }

		/// <summary>
		/// Current X location of the emitter.
		/// </summary>
		public double LocationX { get; set; }

		/// <summary>
		/// Current Y location of the emitter.
		/// </summary>
		public double LocationY { get; set; }

		/// <summary>
		/// Velocity of the emitter in the X axis.
		/// </summary>
		public double VelX { get; set; }

		/// <summary>
		/// Velocity of the emitter in the Y axis.
		/// </summary>
		public double VelY { get; set; }

		/// <summary>
		/// True when the emitter is On.
		/// </summary>
		public bool On { get; set; }

		/// <summary>
		/// Number frames the emitter has been On.
		/// </summary>
		public int OnTimer { get; set; }

		/// <summary>
		/// Number of frames the emitter has been Off.
		/// </summary>
		public int OffTimer { get; set; }

		/// <summary>
		/// Frame counter for the color array.
		/// This counter determines when it is time to move to the next color in the array.
		/// </summary>
		public int FrameColorCounter { get; set; }

		/// <summary>
		/// Current index into the color array.
		/// </summary>
		public int ColorArrayIndex { get; set; }

		#endregion

		#region INotifyPropertyChanged

		[Browsable(false)]
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		/// <param name="propertyName"></param>
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion
	}
}