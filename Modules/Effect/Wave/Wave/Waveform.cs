using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Vixen.Attributes;
using Vixen.Marks;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using ZedGraph;

namespace VixenModules.Effect.Wave
{
	/// <summary>
	/// Maintains a waveform for the wave effect.
	/// </summary>
	[ExpandableObject]
	public class Waveform : ExpandoObjectBase, IWaveform
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public Waveform()
		{			
			WaveType = WaveType.Sine;
			Speed = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			Thickness = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 5.0, 5.0 }));
			Frequency = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 26.0, 26.0 }));
			Height = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			YOffset = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			Direction = DirectionType.LeftToRight;
			Color = new ColorGradient(System.Drawing.Color.Blue);
			ColorHandling = WaveColorHandling.Along;
			PhaseShift = 0;
			PrimeWave = false;
			MovementType = WaveMovementType.Continuous;
			WindowPercentage = 25;
									
			// The following properties are not persisted but 
			// are used to help render the waveform.
			Shrink = false;
			DecayFactor = 0;
			WindowStart = 0;
			WindowStop = 0;
			TriangleCounter = 0;

			// Create the pixel container
			// Storing off the Y position and the color
			Pixels = new Queue<List<Tuple<Color, int>>>();

			IveyYValues = new List<int>();

			InitAllAttributes();
		}

		#endregion

		#region IWaveform

		private WaveType _waveType = WaveType.Sine;

		[ProviderDisplayName(@"WaveType")]
		[ProviderDescription(@"WaveType")]
		[PropertyOrder(1)]
		public WaveType WaveType
		{
			get
			{
				return _waveType;
			}
			set
			{
				_waveType = value;
				UpdateWaveTypeAttributes();
			}
		}

		private bool _useMarks = false;

		[ProviderDisplayName(@"UseMarks")]
		[ProviderDescription(@"UseMarks")]
		[PropertyOrder(2)]
		public bool UseMarks
		{
			get
			{
				return _useMarks;
			}
			set
			{
				_useMarks = value;
				UpdateUseMarkAttributes();
			}
		}
		
		/// <summary>
		/// Name of the selected mark collection.
		/// </summary>
		[ProviderDisplayName(@"MarkCollection")]
		[ProviderDescription(@"WaveMarkCollection")]
		[TypeConverter(typeof(ExpandoMarkCollectionNameConverter))]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(3)]
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
				if (MarkCollections != null)
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
		}

		private WaveMovementType _movementType;

		[ProviderDisplayName(@"WaveMovementType")]
		[ProviderDescription(@"WaveMovementType")]
		[PropertyOrder(4)]
		public WaveMovementType MovementType
		{
			get
			{
				return _movementType;
			}
			set
			{
				_movementType = value;

				if (_movementType == WaveMovementType.Snake)
				{
					PrimeWave = true;
				}

				UpdateMovementTypeAttributes();
			}
		}

		[ProviderDisplayName(@"SnakeLength")]
		[ProviderDescription(@"SnakeLength")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 50, 1)]
		[PropertyOrder(5)]
		public int WindowPercentage { get; set; }

		[ProviderDisplayName(@"PrimeWave")]
		[ProviderDescription(@"PrimeWave")]
		[PropertyOrder(6)]
		public bool PrimeWave { get; set; }

		[ProviderDisplayName(@"Direction")]
		[ProviderDescription(@"WaveDirection")]
		[PropertyOrder(7)]
		public DirectionType Direction { get; set; }
		
		[ProviderDisplayName(@"Mirror")]
		[ProviderDescription(@"Mirror")]
		[PropertyOrder(8)]
		public bool Mirror { get; set; }

		[ProviderDisplayName(@"Frequency")]
		[ProviderDescription(@"Frequency")]
		[PropertyOrder(9)]
		public Curve Frequency { get; set; }

		[ProviderDisplayName(@"PhaseShift")]
		[ProviderDescription(@"PhaseShift")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 180, 1)]
		[PropertyOrder(10)]
		public int PhaseShift { get; set; }

		[ProviderDisplayName(@"Thickness")]
		[ProviderDescription(@"WaveThickness")]
		[PropertyOrder(11)]
		public Curve Thickness { get; set; }
						
		[ProviderDisplayName(@"Amplitude")]
		[ProviderDescription(@"Amplitude")]
		[PropertyOrder(12)]
		public Curve Height { get; set; }
		
		[ProviderDisplayName(@"YOffset")]
		[ProviderDescription(@"WaveYOffset")]
		[PropertyOrder(13)]
		public Curve YOffset { get; set; }

		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"WaveSpeed")]
		[PropertyOrder(14)]
		public Curve Speed { get; set; }
		
		[ProviderDisplayName(@"Color")]
		[ProviderDescription(@"WaveColor")]
		[PropertyOrder(15)]
		public ColorGradient Color { get; set; }

		[ProviderDisplayName(@"ColorHandling")]
		[ProviderDescription(@"WaveColorHandling")]
		[PropertyOrder(16)]
		public WaveColorHandling ColorHandling { get; set; }

		// The following properties are used to support rendering
		[Browsable(false)]
		public int BufferHt { get; set; }

		[Browsable(false)]
		public int BufferWi { get; set;  }

		[Browsable(false)]
		public List<int> IveyYValues { get; private set; }

		[Browsable(false)]
		public int DecayFactor { get; set; }

		[Browsable(false)]
		public int SawTooth { get; set; }

		[Browsable(false)]
		public int SawToothX { get; set; }

		[Browsable(false)]
		public int SawToothMaxY { get; set; }

		[Browsable(false)]
		public int SawToothMinY { get; set; }

		[Browsable(false)]
		public int LastSawToothY { get; set; }

		[Browsable(false)]
		public int LastLastSawToothY { get; set; }

		[Browsable(false)]
		public double Degrees { get; set; }

		[Browsable(false)]
		public bool Shrink { get; set; }

		[Browsable(false)]
		public int WindowStart { get; set; }

		[Browsable(false)]
		public int WindowStop { get; set; }

		[Browsable(false)]
		public int TriangleCounter { get; set; }

		[Browsable(false)]
		public double YC
		{
			get
			{
				return BufferHt / 2.0;
			}
		}
			
		[Browsable(false)]
		public int LastWaveThickness { get; set; }

		/// <summary>
		/// The Emitter needs the Parent reference to register for Mark events.
		/// </summary>
		[Browsable(false)]
		public BaseEffect Parent { get; set; }

		private Guid _markCollectionId = new Guid();

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
		
		/// <summary>
		/// Returns a clone of the emitter.
		/// </summary>		
		public IWaveform CreateInstanceForClone()
		{
			IWaveform result = new Waveform
			{
				WaveType = WaveType,
				UseMarks = UseMarks,
				MarkNameCollection = MarkNameCollection,
				MovementType = MovementType,
				WindowPercentage = WindowPercentage,
				PrimeWave = PrimeWave,
				Direction = Direction,
				Mirror = Mirror,
				Frequency = new Curve(Frequency),
				PhaseShift = PhaseShift,
				Thickness = new Curve(Thickness),
				Height = new Curve(Height),
				YOffset = new Curve(YOffset),
				Speed = new Curve(Speed),
				Color = new ColorGradient(Color),
				ColorHandling = ColorHandling,
				BufferHt = BufferHt,
				BufferWi = BufferWi,
				TriangleCounter = TriangleCounter,
				DecayFactor = DecayFactor,
				SawTooth = SawTooth,
				SawToothX = SawToothX,
				Degrees = Degrees,
				Shrink = Shrink,
				WindowStart = WindowStart,
				WindowStop = WindowStop,
				LastWaveThickness = LastWaveThickness,
				Parent = Parent,
				MarkCollections = MarkCollections,
				MarkCollectionId = MarkCollectionId,
				MarkCollectionName = MarkCollectionName,				
				IveyYValues = IveyYValues,		
				Pixels = Pixels,
			};

			return result;
		}

		[Browsable(false)]
		public Queue<List<Tuple<Color, int>>> Pixels { get; private set; }

		/// <summary>
		/// Gets the degrees per x coordinate.
		/// </summary>		
		public double GetDegreesPerX(int numberOfWaves)
		{
			return ((double)numberOfWaves) / BufferWi;
		}

		#endregion

		#region Implementation of ICloneable

		/// <inheritdoc />
		public object Clone()
		{
			return CreateInstanceForClone();
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

			OnPropertyChanged("MarkCollections");
			OnPropertyChanged("MarkCollectionName");
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
		/// Updates the browseable state of properties related to emitter animation.
		/// </summary>
		private void UpdateWaveTypeAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(7)
			{
				{nameof(Frequency), WaveType != WaveType.FractalIvey },
				{nameof(Height), WaveType != WaveType.FractalIvey},
				{nameof(YOffset), WaveType != WaveType.FractalIvey},
				{nameof(PrimeWave), WaveType != WaveType.FractalIvey && 
				                    WaveType != WaveType.DecayingSine},
				{nameof(MovementType), WaveType != WaveType.FractalIvey &&
				                       WaveType != WaveType.DecayingSine},
				{nameof(PhaseShift), WaveType != WaveType.FractalIvey &&
				                     WaveType != WaveType.DecayingSine},

				{nameof(UseMarks), WaveType == WaveType.DecayingSine },
				{nameof(MarkCollectionName), WaveType == WaveType.DecayingSine && UseMarks },							
			};
			SetBrowsable(propertyStates);
		}

		/// <summary>
		/// Updates the browseable state of properties related to wave movement type.
		/// </summary>
		private void UpdateMovementTypeAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{				
				{ nameof(WindowPercentage), MovementType == WaveMovementType.Snake},
			};
			SetBrowsable(propertyStates);
		}

		/// <summary>
		/// Updates the browseable state of properties related to using marks colletions.
		/// </summary>
		private void UpdateUseMarkAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{ nameof(MarkCollectionName), UseMarks},
			};
			SetBrowsable(propertyStates);
		}

		/// <summary>
		/// Updates the visibility of the waveform properties.
		/// </summary>
		private void InitAllAttributes()
		{
			UpdateWaveTypeAttributes();
			UpdateMovementTypeAttributes();
			UpdateUseMarkAttributes();
			TypeDescriptor.Refresh(this);
		}

		#endregion
	}
}
