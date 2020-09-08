using Common.Controls.ColorManagement.ColorModels;
using Liquid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Vixen.Attributes;
using Vixen.Marks;
using Vixen.Module;
using Vixen.Module.Media;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using VixenModules.Media.Audio;

namespace VixenModules.Effect.Liquid
{
	/// <summary>
	/// Liquid effect based on the Google Liquid Fun API.
	/// </summary>
	public class Liquid : PixelEffectBase
	{
		#region Constants

		private const int Spacing = 50;
		private const int MaxEmitterAnimateSpeed = 10;
		private const int MaxEmitterLifeTime = 1000;
		private const int MaxEmitterParticleVelocity = 100;
		private const int MaxEmitterSourceSize = 100;
		private const int MaxEmitterFlow = 200;
		private const int MaxEmitterMusicFlow = 100;

		#endregion

		#region Private Fields

		/// <summary>
		/// Logical buffer height.
		/// Note this height might not match the actual effect height when the effect is operating in Location mode.
		/// </summary>
		int _bufferHt;

		/// <summary>
		/// Logical buffer height.
		/// Note this width might not match the actual effect width when the effect is operating in Location mode.
		/// </summary>
		int _bufferWt;

		/// <summary>
		/// Data associated with the effect.
		/// </summary>
		private LiquidData _data;

		/// <summary>
		/// Liquid Fun API wrapper.
		/// </summary>
		private LiquidFunWrapper _liquidFunWrapper;

		/// <summary>
		/// Minimum volume associated with the effect.
		/// </summary>
		private double _minVolume;

		/// <summary>
		/// Maximum volume associated with the effect.
		/// </summary>
		private double _maxVolume;

		/// <summary>
		/// Cached frame volumes for the effect.
		/// </summary>
		private List<double> _frameVolumes = new List<double>();

		/// <summary>
		/// Dictionaries to convert from odel enumerations to serialization enumerations.
		/// </summary>
		private Dictionary<ParticleType, EmitterData.ParticleSerializationType> _particleTypeToSerializedParticleType;
		private Dictionary<EdgeHandling, EmitterData.EdgeHandlingSerializationType> _edgeHandlingToSerializedEdgeHandling;
		private Dictionary<NozzleMovement, EmitterData.NozzleMovementSerializationType> _nozzleAngleToSerializedNozzleAngle;
		private Dictionary<FlowControl, EmitterData.FlowControlSerializationType> _flowControlToSerializedFlowControl;

		/// <summary>
		/// Dictionaries to convert from serialization enumerations to model enumerations.
		/// </summary>
		private Dictionary<EmitterData.ParticleSerializationType, ParticleType> _serializedParticleTypeToParticleType;
		private Dictionary<EmitterData.EdgeHandlingSerializationType, EdgeHandling> _serializedEdgeHandlingToEdgeHandling;
		private Dictionary<EmitterData.NozzleMovementSerializationType, NozzleMovement> _serializedNozzleAngleToNozzleAngle;
		private Dictionary<EmitterData.FlowControlSerializationType, FlowControl> _serializedFlowControlToFlowControl;

		/// <summary>
		/// Copy of emitter list that is used during rendering.
		/// A copy is made in an attempt to avoid the collection being modified while the effect is rendering.
		/// </summary>
		private List<IEmitter> _renderEmitterList;

		/// <summary>
		/// Copy of the render scale factor.
		/// A copy is made in an attempt to avoid the value being modified while the effet is rendering.
		/// </summary>
		private int _renderScaleFactor;

		/// <summary>
		/// Mark collection names.  This collection is shared with each of the emitters.
		/// </summary>
		ObservableCollection<string> _markCollectionNames = new ObservableCollection<string>();

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public Liquid()
		{
			// Enable both string and location positioning
			EnableTargetPositioning(true, true);

			// Initialize the default liquid effect data
			_data = new LiquidData();

			// Create the collection of the emitters
			_emitterList = new EmitterCollection();

			// Register for the EmitterList child property changed event
			_emitterList.ChildPropertyChanged += EmitterListChildPropertyChanged;

			// Give the emitter a reference to the effect.
			// This is needed so that the emitters can use the parent to register (listen) for mark collection events
			EmitterList.Parent = this;

			// Initialize the wrapper around the Liquid Fun API
			_liquidFunWrapper = new LiquidFunWrapper();

			// Initialize the audio utilities
			AudioUtilities = new AudioUtilities();
			
			// Initialize the dictionaries that convert between model enumeration types and the serialization model enumeration types
			_particleTypeToSerializedParticleType = new Dictionary<ParticleType, EmitterData.ParticleSerializationType>();
			_particleTypeToSerializedParticleType.Add(ParticleType.Elastic, EmitterData.ParticleSerializationType.Elastic);
			_particleTypeToSerializedParticleType.Add(ParticleType.Powder, EmitterData.ParticleSerializationType.Powder);
			_particleTypeToSerializedParticleType.Add(ParticleType.Tensile, EmitterData.ParticleSerializationType.Tensile);
			_particleTypeToSerializedParticleType.Add(ParticleType.Spring, EmitterData.ParticleSerializationType.Spring);
			_particleTypeToSerializedParticleType.Add(ParticleType.Viscous, EmitterData.ParticleSerializationType.Viscous);
			_particleTypeToSerializedParticleType.Add(ParticleType.StaticPressure, EmitterData.ParticleSerializationType.StaticPressure);
			_particleTypeToSerializedParticleType.Add(ParticleType.Water, EmitterData.ParticleSerializationType.Water);
			_particleTypeToSerializedParticleType.Add(ParticleType.Reactive, EmitterData.ParticleSerializationType.Reactive);
			_particleTypeToSerializedParticleType.Add(ParticleType.Repulsive, EmitterData.ParticleSerializationType.Repulsive);

			_edgeHandlingToSerializedEdgeHandling = new Dictionary<EdgeHandling, EmitterData.EdgeHandlingSerializationType>();
			_edgeHandlingToSerializedEdgeHandling.Add(EdgeHandling.Bounce, EmitterData.EdgeHandlingSerializationType.Bounce);
			_edgeHandlingToSerializedEdgeHandling.Add(EdgeHandling.Wrap, EmitterData.EdgeHandlingSerializationType.Wrap);

			_serializedParticleTypeToParticleType = new Dictionary<EmitterData.ParticleSerializationType, ParticleType>();
			_serializedParticleTypeToParticleType.Add(EmitterData.ParticleSerializationType.Elastic, ParticleType.Elastic);
			_serializedParticleTypeToParticleType.Add(EmitterData.ParticleSerializationType.Powder, ParticleType.Powder);
			_serializedParticleTypeToParticleType.Add(EmitterData.ParticleSerializationType.Tensile, ParticleType.Tensile);
			_serializedParticleTypeToParticleType.Add(EmitterData.ParticleSerializationType.Spring, ParticleType.Spring);
			_serializedParticleTypeToParticleType.Add(EmitterData.ParticleSerializationType.Viscous, ParticleType.Viscous);
			_serializedParticleTypeToParticleType.Add(EmitterData.ParticleSerializationType.StaticPressure, ParticleType.StaticPressure);
			_serializedParticleTypeToParticleType.Add(EmitterData.ParticleSerializationType.Water, ParticleType.Water);
			_serializedParticleTypeToParticleType.Add(EmitterData.ParticleSerializationType.Reactive, ParticleType.Reactive);
			_serializedParticleTypeToParticleType.Add(EmitterData.ParticleSerializationType.Repulsive, ParticleType.Repulsive);

			_serializedEdgeHandlingToEdgeHandling = new Dictionary<EmitterData.EdgeHandlingSerializationType, EdgeHandling>();
			_serializedEdgeHandlingToEdgeHandling.Add(EmitterData.EdgeHandlingSerializationType.Bounce, EdgeHandling.Bounce);
			_serializedEdgeHandlingToEdgeHandling.Add(EmitterData.EdgeHandlingSerializationType.Wrap, EdgeHandling.Wrap);

			_nozzleAngleToSerializedNozzleAngle = new Dictionary<NozzleMovement, EmitterData.NozzleMovementSerializationType>();
			_nozzleAngleToSerializedNozzleAngle.Add(NozzleMovement.FixedAngle, EmitterData.NozzleMovementSerializationType.FixedAngle);
			_nozzleAngleToSerializedNozzleAngle.Add(NozzleMovement.Oscillate, EmitterData.NozzleMovementSerializationType.Oscillate);
			_nozzleAngleToSerializedNozzleAngle.Add(NozzleMovement.SpinClockwise, EmitterData.NozzleMovementSerializationType.SpinClockwise);
			_nozzleAngleToSerializedNozzleAngle.Add(NozzleMovement.SpinCounterClockwise, EmitterData.NozzleMovementSerializationType.SpinCounterClockwise);

			_serializedNozzleAngleToNozzleAngle = new Dictionary<EmitterData.NozzleMovementSerializationType, NozzleMovement>();
			_serializedNozzleAngleToNozzleAngle.Add(EmitterData.NozzleMovementSerializationType.FixedAngle, NozzleMovement.FixedAngle);
			_serializedNozzleAngleToNozzleAngle.Add(EmitterData.NozzleMovementSerializationType.Oscillate, NozzleMovement.Oscillate);
			_serializedNozzleAngleToNozzleAngle.Add(EmitterData.NozzleMovementSerializationType.SpinClockwise, NozzleMovement.SpinClockwise);
			_serializedNozzleAngleToNozzleAngle.Add(EmitterData.NozzleMovementSerializationType.SpinCounterClockwise, NozzleMovement.SpinCounterClockwise);

			_flowControlToSerializedFlowControl = new Dictionary<FlowControl, EmitterData.FlowControlSerializationType>();
			_flowControlToSerializedFlowControl.Add(FlowControl.Continuous, EmitterData.FlowControlSerializationType.Continuous);
			_flowControlToSerializedFlowControl.Add(FlowControl.Pulsating, EmitterData.FlowControlSerializationType.Pulsating);
			_flowControlToSerializedFlowControl.Add(FlowControl.UseMarks, EmitterData.FlowControlSerializationType.UseMarks);
			_flowControlToSerializedFlowControl.Add(FlowControl.Musical, EmitterData.FlowControlSerializationType.Musical);

			_serializedFlowControlToFlowControl = new Dictionary<EmitterData.FlowControlSerializationType, FlowControl>();
			_serializedFlowControlToFlowControl.Add(EmitterData.FlowControlSerializationType.Continuous, FlowControl.Continuous);
			_serializedFlowControlToFlowControl.Add(EmitterData.FlowControlSerializationType.Pulsating, FlowControl.Pulsating);
			_serializedFlowControlToFlowControl.Add(EmitterData.FlowControlSerializationType.UseMarks, FlowControl.UseMarks);
			_serializedFlowControlToFlowControl.Add(EmitterData.FlowControlSerializationType.Musical, FlowControl.Musical);
		}
		
		#endregion

		#region Public Properties

		/// <summary>
		/// String orientation of the effect.
		/// </summary>
		public override StringOrientation StringOrientation
		{
			get { return _data.Orientation; }
			set
			{
				_data.Orientation = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}
		
		[Value]
		[ProviderCategory(@"Setup", 1)]
		[ProviderDisplayName(@"RenderScaleFactor")]
		[ProviderDescription(@"RenderScaleFactor")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 10, 1)]
		[PropertyOrder(4)]
		public int RenderScaleFactor
		{
			get
			{
				return _data.RenderScaleFactor;
			}
			set
			{
				_data.RenderScaleFactor = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}
		
		/// <summary>
		/// Module data associated with the effect.
		/// </summary>
		public override IModuleDataModel ModuleData
		{
			get
			{
				// Convert the model into the serialized data structure
				UpdateLiquidData();

				// Return the effect data
				return _data;
			}
			set
			{
				// Save off the data for the effect
				_data = value as LiquidData;

				// Initialize the audio utilities
				AudioUtilities.DecayTime = _data.DecayTime;
				AudioUtilities.AttackTime = _data.AttackTime;
				AudioUtilities.Gain = _data.Gain;
				AudioUtilities.Normalize = _data.Normalize;
				AudioUtilities.LowPass = _data.LowPass;
				AudioUtilities.LowPassFreq = _data.LowPassFreq;
				AudioUtilities.HighPass = _data.HighPass;
				AudioUtilities.HighPassFreq = _data.HighPassFreq;

				// Convert the serialized data structure into the emitter collection
				UpdateEmitterModel(_data);

				// Updates the browseable state of the top level properties
				UpdateAllAttributes();
				
				// Give the emitter container a reference to the effect
				_emitterList.Parent = this;

				OnPropertyChanged("EmitterList");
				MarkDirty();
			}
		}

		#endregion

		#region Configuration Properties

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"TopBarrier")]
		[ProviderDescription(@"TopBarrier")]
		[PropertyOrder(1)]
		public bool TopBarrier
		{
			get { return _data.TopBarrier; }
			set
			{
				_data.TopBarrier = value;
				MarkDirty();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"BottomBarrier")]
		[ProviderDescription(@"BottomBarrier")]
		[PropertyOrder(2)]
		public bool BottomBarrier
		{
			get { return _data.BottomBarrier; }
			set
			{
				_data.BottomBarrier = value;
				MarkDirty();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"LeftBarrier")]
		[ProviderDescription(@"LeftBarrier")]
		[PropertyOrder(3)]
		public bool LeftBarrier
		{
			get { return _data.LeftBarrier; }
			set
			{
				_data.LeftBarrier = value;
				MarkDirty();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"RightBarrier")]
		[ProviderDescription(@"RightBarrier")]
		[PropertyOrder(4)]
		public bool RightBarrier
		{
			get { return _data.RightBarrier; }
			set
			{
				_data.RightBarrier = value;
				MarkDirty();
				OnPropertyChanged();
			}
		}
				
		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"MixColors")]
		[ProviderDescription(@"MixColors")]
		[PropertyOrder(5)]
		public bool MixColors
		{
			get { return _data.MixColors; }
			set
			{
				_data.MixColors = value;
				MarkDirty();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"ParticleSize")]
		[ProviderDescription(@"ParticleSize")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 10000, 1)]
		[PropertyOrder(6)]
		public int ParticleSize
		{
			get { return _data.ParticleSize; }
			set
			{
				_data.ParticleSize = value;
				MarkDirty();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"WarmUpFrames")]
		[ProviderDescription(@"WarmUpFrames")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 500, 1)]
		[PropertyOrder(7)]
		public int WarmUpFrames
		{
			get
			{
				return _data.WarmUpFrames;
			}
			set
			{
				_data.WarmUpFrames = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"DespeckleThreshold")]
		[ProviderDescription(@"DespeckleThreshold")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 4, 1)]
		[PropertyOrder(8)]
		public int DespeckleThreshold
		{
			get
			{
				return _data.DespeckleThreshold;
			}
			set
			{
				_data.DespeckleThreshold = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		private EmitterCollection _emitterList;

		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Emitters")]
		[ProviderDescription(@"Emitters")]
		[PropertyOrder(9)]
		public EmitterCollection EmitterList
		{
			get
			{
				return _emitterList;
			}
			set
			{
				_emitterList = value;
				MarkDirty();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Colors", 2)]
		[ProviderDisplayName(@"ColorList")]
		[ProviderDescription(@"ColorList")]
		[PropertyOrder(5)]
		public List<ColorGradient> Colors
		{
			get
			{
				return _data.Colors;
			}
			set
			{
				_data.Colors = value;
				MarkDirty();
				OnPropertyChanged();
			}
		}
		
		#endregion

		#region Audio Sensitivity Range

		[Value]
		[ProviderCategory(@"AudioSensitivityRange", 3)]
		[PropertyOrder(0)]
		[ProviderDisplayName(@"Gain")]
		[ProviderDescription(@"Boosts the volume")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 200, .5)]
		public int Gain
		{
			get { return _data.Gain * 10; }
			set
			{
				_data.Gain = value / 10;
				AudioUtilities.Gain = value / 10;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"AudioSensitivityRange", 3)]
		[ProviderDisplayName(@"HighPassFilter")]
		[ProviderDescription(@"Passes frequencies above a given frequency")]
		[PropertyOrder(1)]
		public bool HighPass
		{
			get { return _data.HighPass; }
			set
			{
				_data.HighPass = value;
				AudioUtilities.HighPass = value;
				IsDirty = true;
				UpdateLowHighPassAttributes(true);
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"AudioSensitivityRange", 3)]
		[ProviderDisplayName(@"HighPassFrequency")]
		[ProviderDescription(@"Passes frequencies above this value")]
		[PropertyOrder(2)]
		public int HighPassFreq
		{
			get { return _data.HighPassFreq; }
			set
			{
				_data.HighPassFreq = value;
				AudioUtilities.HighPassFreq = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"AudioSensitivityRange", 3)]
		[ProviderDisplayName(@"LowPassFilter")]
		[ProviderDescription(@"Passes frequencies below a given frequency")]
		[PropertyOrder(3)]
		public bool LowPass
		{
			get { return _data.LowPass; }
			set
			{
				_data.LowPass = value;
				AudioUtilities.LowPass = value;
				IsDirty = true;
				UpdateLowHighPassAttributes(true);
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"AudioSensitivityRange", 3)]
		[ProviderDisplayName(@"LowPassFrequency")]
		[ProviderDescription(@"Passes frequencies below this value")]
		[PropertyOrder(4)]
		public int LowPassFreq
		{
			get { return _data.LowPassFreq; }
			set
			{
				_data.LowPassFreq = value;
				AudioUtilities.LowPassFreq = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"AudioSensitivityRange", 3)]
		[ProviderDescription(@"Configures the peak volume of the selected audio range to the max emitter flow")]
		[PropertyOrder(5)]
		public bool Normalize
		{
			get { return _data.Normalize; }
			set
			{
				AudioUtilities.Normalize = value;
				_data.Normalize = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"AudioSensitivityRange", 3)]
		[PropertyOrder(6)]
		[ProviderDisplayName(@"Zoom")]
		[ProviderDescription(@"The range of the volume levels displayed by the effect")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 20, 1)]
		public int Range
		{
			get { return 20 - _data.Range; }
			set
			{
				_data.Range = 20 - value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Response Speed

		[Value]
		[ProviderCategory(@"ResponseSpeed", 4)]
		[PropertyOrder(1)]
		[ProviderDisplayName(@"AttackTime")]
		[ProviderDescription(@"How quickly the emitter flow initially reacts to a volume peak")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 300, 10)]
		public int AttackTime
		{
			get { return _data.AttackTime; }
			set
			{
				_data.AttackTime = value;
				AudioUtilities.AttackTime = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"ResponseSpeed", 4)]
		[PropertyOrder(2)]
		[ProviderDisplayName(@"DecayTime")]
		[ProviderDescription(@"How quickly the emitter flow falls from a volume peak")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 5000, 300)]
		public int DecayTime
		{
			get { return _data.DecayTime; }
			set
			{
				_data.DecayTime = value;
				AudioUtilities.DecayTime = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Virtual method that is called by base class when the target positioning changes.
		/// </summary>
		protected override void TargetPositioningChanged()
		{
			// Update whether the render scale factor is visible 
			UpdateRenderScaleFactorAttributes(true);
		}

		/// <summary>
		/// Virtualized event handler for when the mark collection changes.
		/// </summary>
		protected override void MarkCollectionsChanged()
		{
			// Loop over the mark collections
			foreach (IMarkCollection collection in MarkCollections)
			{
				// Register for property changed events from the specific mark collections
				collection.PropertyChanged -= MarkCollectionPropertyChanged;
				collection.PropertyChanged += MarkCollectionPropertyChanged;
			}

			// Update the shared mark collection names
			UpdateMarkCollectionNames();

			// Give the collection of emitters the mark collections
			EmitterList.MarkCollections = MarkCollections;

			// Give the collection of emitter the shared mark collection names
			EmitterList.MarkNameCollection = _markCollectionNames;
		}

		/// <summary>
		/// Method for effects to manage mark collections changing.
		/// </summary>
		protected override void MarkCollectionsAdded(IList<IMarkCollection> addedCollections)
		{
			// Loop over the added mark collections
			foreach (IMarkCollection markCollection in addedCollections)
			{
				// Register for property changed events from the added mark collections
				markCollection.PropertyChanged -= MarkCollectionPropertyChanged;
				markCollection.PropertyChanged += MarkCollectionPropertyChanged;
			}

			// Update the collection of mark collection names
			UpdateMarkCollectionNames();
		}

		/// <summary>
		/// Virtualized event handler for when a mark collection has been removed.
		/// </summary>		
		protected override void MarkCollectionsRemoved(IList<IMarkCollection> removedCollections)
		{
			// Make a copy of the emitters in a weak attempt to minimize thread exceptions
			IEnumerable<IEmitter> emitters = EmitterList.ToList();

			// Loop over the removed mark collections
			foreach (IMarkCollection markCollection in removedCollections)
			{
				// Unregister for property changed events from the mark collection
				markCollection.PropertyChanged -= MarkCollectionPropertyChanged;

				// If any of the emitter had the removed mark collection selected then...
				if (emitters.Any(emitter => emitter.MarkCollectionId == markCollection.Id))
				{
					// Mark the effect dirty
					MarkDirty();
				}
			}

			// Update the collection of mark collection names
			// This method also removes this mark collection as being selected on any emitter.
			UpdateMarkCollectionNames();
		}

		/// <summary>
		/// Gets the data associated with the effect.
		/// </summary>
		protected override EffectTypeModuleData EffectModuleData
		{
			get
			{
				UpdateLiquidData();
				return _data;
			}
		}

		/// <summary>
		/// Releases resources from the rendering process.
		/// </summary>
		protected override void CleanUpRender()
		{
			// Releae the audio resources
			AudioUtilities.FreeMem();

			// Finalize the Liquid Fun API
			_liquidFunWrapper.FinalizeWorld();
		}

		/// <summary>
		/// Render's the effect for the specified frame.
		/// </summary>		
		protected override void RenderEffect(int frameNum, IPixelFrameBuffer frameBuffer)
		{
			double intervalPos = GetEffectTimeIntervalPosition(frameNum);
			double intervalPosFactor = intervalPos * 100;

			// Have the Liquid Fun Physics engine advance time and recalculate the position of particles
			_liquidFunWrapper.StepWorld(
				 FrameTime / 1000f,
				 _data.MixColors,
				 ConvertEmittersToEmitterWrappers(frameNum, intervalPos, intervalPosFactor).ToArray());

			// Get the collection of particles from Liquid Fun API
			// Tuple is composed of the position of the particle in x, y and then the color of the particle
			Tuple<int, int, Color>[] particles = _liquidFunWrapper.GetParticles();

			// Turn on pixels for the particles
			foreach (Tuple<int, int, Color> particle in particles)
			{
				frameBuffer.SetPixel(
					 x: particle.Item1,
					 y: particle.Item2,
					 c: particle.Item3);
			}
									
			// If despeckling is turned on then...
			if (DespeckleThreshold > 0)
			{
				for (int y = 0; y < BufferHt; y++)
				{
					for (int x = 0; x < BufferWi; x++)
					{
						if (frameBuffer.GetColorAt(x, y) == Color.Transparent)
						{
							frameBuffer.SetPixel(x, y, GetDespeckleColor(frameBuffer, x, y, DespeckleThreshold));
						}
					}
				}
			}			
		}

		/// <summary>
		/// Gets the color to fill in the specified pixel speck.
		/// </summary>				
		private Color GetDespeckleColor(IPixelFrameBuffer frameBuffer, int x, int y, int despeckle) 
		{
			int red = 0;
			int green = 0;
			int blue = 0;
			int count = 0;

			int startx = x - 1;
			if (startx < 0)
			{
				startx = 0;
			}
	
			int starty = y - 1;
			if (starty < 0)
			{
				starty = 0;
			}

			int endx = x + 1;
			if (endx >= BufferWi)
			{
				endx = BufferWi - 1;
			}

			int endy = y + 1;
			if (endy >= BufferHt)
			{
				endy = BufferHt - 1;
			}

			int blacks = 0;
			
			for (int yy = starty; yy <= endy; ++yy)
			{
				for (int xx = startx; xx <= endx; ++xx)
				{
					if (yy != y || xx != x) // dont evaluate the pixel itself
					{
						Color c = frameBuffer.GetColorAt(xx, yy);

						// if any surrounding pixel is also black then we return black ... we only despeckly totally surrounded pixels
						if (c == Color.Transparent)							
						{
							blacks++;
							if (blacks >= despeckle)
							{
								return Color.Transparent;
							}
						}
						
						red += c.R;
						green += c.G;
						blue += c.B;
						count++;
					}
				}
			}

			if (count == 0)
			{
				return Color.Transparent;
			}

			return Color.FromArgb(red / count, green / count, blue / count);
		}
						
		/// <summary>
		/// Renders the effect by location.
		/// </summary>		
		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			// Make a local copy that is faster than the logic to get it for reuse.
			var localBufferHt = BufferHt;
						
			// Loop over the frames
			for (int frameNum = 0; frameNum < numFrames; frameNum++)
			{
				//Assign the current frame
				frameBuffer.CurrentFrame = frameNum;

				// Get the position in the effect
				double intervalPos = GetEffectTimeIntervalPosition(frameNum);
				double intervalPosFactor = intervalPos * 100;

				// Have the Liquid Fun Physics engine advance time and recalculate the position of particles
				_liquidFunWrapper.StepWorld(
					FrameTime / 1000f,
					_data.MixColors,
					ConvertEmittersToEmitterWrappers(frameNum, intervalPos, intervalPosFactor).ToArray());

				// Get the collection of particles from Liquid Fun API
				// Tuple is composed of the position in x, y and then color of the particle
				Tuple<int, int, Color>[] particles = _liquidFunWrapper.GetParticles();
				
				// Create a bitmap the size of the logical render area
				// Note this render area is smaller matrix than the actual display elements
				using (var fp = new FastPixel.FastPixel(_bufferWt, _bufferHt))
				{
					fp.Lock();
					// Loop over each of the liquid particles
					foreach (Tuple<int, int, Color> particle in particles)
					{
						// If the particle is within the viewable area then...
						if ((particle.Item1 >= 0 && particle.Item1 < _bufferWt) &&
							 (particle.Item2 >= 0 && particle.Item2 < _bufferHt))
						{
							// Set the corresponding pixel in the bitmap
							fp.SetPixel(
								particle.Item1,
								particle.Item2,
								particle.Item3);
						}
					}
					
					foreach (ElementLocation elementLocation in frameBuffer.ElementLocations)
					{
						UpdateFrameBufferForLocationPixel(
							elementLocation.X, 
							elementLocation.Y, 
							localBufferHt,
							fp, 
							frameBuffer);
					}					

					fp.Unlock(false);																
				}				
			}			
		}
		
		/// <summary>
		/// Setup for rendering.
		/// </summary>
		protected override void SetupRender()
		{
			// Create a copy of the emitter list to avoid collection exceptions
			_renderEmitterList = EmitterList.ToList();
			
			// Determine the logical render area based on the target positioning
			if (TargetPositioning == TargetPositioningType.Locations)
			{
				// Since this effect uses one pixel to represent a liquid particle
				// it looks better to render on a smaller matrix and then scale it up to the actual render matrix.
				// When the scale factor is one liquid particles appear and disassapear because location 
				// mode is often a sparse matrix.				
				_renderScaleFactor = RenderScaleFactor;
				_bufferWt = BufferWi / _renderScaleFactor;
				_bufferHt = BufferHt / _renderScaleFactor;
				
				// Need to increase the render height if the scale factor did not divide evenly
				if (BufferHt % _renderScaleFactor != 0)
				{
					_bufferHt++;
				}

				// Need to increase the render width if the scale factor did not divide evenly
				if (BufferWi % _renderScaleFactor != 0)
				{
					_bufferWt++;
				}
			}
			else
			{
				_bufferWt = BufferWi;
				_bufferHt = BufferHt; 
			}
			
			// Initialize the liquid fun API wrapper
			_liquidFunWrapper.Initialize(_bufferWt, _bufferHt, BottomBarrier, TopBarrier, LeftBarrier, RightBarrier, ParticleSize, CalculateEmitterParticleLifetime(0, _renderEmitterList[0]));

			// Loop over emitters
			foreach (IEmitter emitter in _renderEmitterList)
			{
				// If the emitter uses animated movement then...
				if (emitter.Animate)
				{
					// If the emitter's initial position should be random then...
					if (emitter.RandomStartingPosition)
					{
						// Initialize the position of the emitter with a random position
						emitter.LocationX = Rand(0, _bufferWt);
						emitter.LocationY = Rand(0, _bufferHt);
					}
					// Otherwise let the user pick the starting position using the X, Y sliders
					else
					{
						// Scale the initial X and Y position of the emitter based on the overall display element width and height
						emitter.LocationX = (emitter.AnimateXStart / 100.0) * _bufferWt; // Zero = Left, 100 = Right
						emitter.LocationY = (emitter.AnimateYStart / 100.0) * _bufferHt; // Zero = Bottom, 100 = Top
					}
				}

				// Reset the On/Off Timers
				emitter.OnTimer = 0;
				emitter.OffTimer = 0;
			}

			GetAudioSettings();			
			CacheOffAudioVolumes();

			// Process the optional warm up frames
			for(int warmUpFrame = 0; warmUpFrame < WarmUpFrames; warmUpFrame++)
			{
				// Advance the Liquid engine
				_liquidFunWrapper.StepWorld(
					FrameTime / 1000f,
					_data.MixColors,
					ConvertEmittersToEmitterWrappers(0, 0, 0).ToArray());
			}
		}

		#endregion

		#region Private Properties

		[Browsable(false)]
		private AudioUtilities AudioUtilities { get; set; }

		#endregion

		#region Private Audio Methods

		/// <summary>
		/// Updates the browseable state of the low and high pass frequency properties.
		/// </summary>
		/// <param name="refresh"></param>
		private void UpdateLowHighPassAttributes(bool refresh = false)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
				{
					 {"LowPassFreq", LowPass},
					 {"HighPassFreq", HighPass}
				};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		/// <summary>
		/// Returns true if audio is associated with the sequence.
		/// </summary>		
		private bool AudioAssociated()
		{
			bool audioAssociated = false;

			if (Media != null)
			{
				foreach (IMediaModuleInstance module in Media)
				{
					var audio = module as Audio;
					if (audio != null)
					{
						if (audio.Channels == 0)
						{
							continue;
						}

						audioAssociated = true;
						break;
					}
				}
			}

			return audioAssociated;
		}

		/// <summary>
		/// Loads the audio associated with the sequence into the AudioUtilities helper.
		/// </summary>
		private void GetAudioSettings()
		{
			if (Media != null)
			{
				foreach (IMediaModuleInstance module in Media)
				{
					var audio = module as Audio;
					if (audio != null)
					{
						if (audio.Channels == 0)
						{
							continue;
						}
						AudioUtilities.TimeSpan = TimeSpan;
						AudioUtilities.StartTime = StartTime;
						AudioUtilities.Gain = Gain;
						AudioUtilities.ReloadAudio(audio);
					}
				}
			}
		}

		/// <summary>
		/// Saves off volumes for use during rendering.
		/// </summary>
		private void CacheOffAudioVolumes()
		{
			// If audio has been associated with the sequence then...
			if (AudioUtilities.AudioLoaded)
			{
				// Clear out the cached volumes
				_frameVolumes.Clear();

				// Initialize the minimum and maximum volumes for the effect
				_minVolume = AudioUtilities.VolumeAtTime(0);
				_maxVolume = _minVolume;

				// Loop over the frames in the effect
				for (int frameIndex = 1; frameIndex <= (int)(TimeSpan.TotalMilliseconds / Spacing); frameIndex++)
				{
					// Get the volume for the specified frame
					double currentVolume = AudioUtilities.VolumeAtTime(frameIndex * Spacing);

					// Store off the current volume
					_frameVolumes.Add(currentVolume);

					// If the current volume is softer than the current minimum then...
					if (currentVolume < _minVolume)
					{
						// Save off a new current minimum volume
						_minVolume = currentVolume;
					}
					// Else if the volume is louder than the current maximum then...
					else if (currentVolume > _maxVolume)
					{
						// Save off a new current maximum volume
						_maxVolume = currentVolume;
					}
				}
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Updates whether the render scale factor is visible.
		/// </summary>		
		void UpdateRenderScaleFactorAttributes(bool refresh)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
				{
					{"RenderScaleFactor", TargetPositioning == TargetPositioningType.Locations },
				};
			SetBrowsable(propertyStates);

			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		/// <summary>
		/// Update the browseable state of all top level properties.
		/// </summary>
		private void UpdateAllAttributes()
		{
			// Updates the browseable state of render scale factor property
			UpdateRenderScaleFactorAttributes(false);

			// Updates the browseable state of the audio properties
			UpdateAudioAttributes();

			// Update the browseable state of the color list collection
			UpdateColorListAttributes();
		}

		/// <summary>
		/// Updates the browseable state of the color list collection.
		/// </summary>		
		void UpdateColorListAttributes(bool refresh = true)
		{
			bool emittersWithColorList = EmitterList.Any(emit => emit.UseColorArray);

			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
				{
					{"Colors", emittersWithColorList },
				};
			SetBrowsable(propertyStates);

			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		/// <summary>
		/// Updates the emitter's flow and updates the specified emitter wrapper.
		/// </summary>		
		private void UpdateEmitterFlow(EmitterWrapper emitterWrapper, IEmitter emitter, int frameNum, double intervalPosFactor)
		{
			// Update the flow based on the emitter's flow control
			switch (emitter.FlowControl)
			{
				case FlowControl.Continuous:
					// Emitter is Continously On get the flow from the curve
					emitterWrapper.Flow = (int)Math.Round(ScaleCurveToValue(emitter.Flow.GetValue(intervalPosFactor), MaxEmitterFlow, 0));					
					break;
				case FlowControl.Pulsating:
					
					// If the emitter is On then...
					if (emitter.On)
					{
						// Keep track of the number of frames the emitter is on
						emitter.OnTimer+= FrameTime;

						// Check to see if the emitter should transition to Off
						if (emitter.OnTimer >= emitter.OnTime)
						{
							// Turn the emitter Off
							emitter.On = false;

							// Reset the On frame counter
							emitter.OnTimer = 0;
						}
					}
					else
					{
						// Keep track of the number of frames the emitter is Off
						emitter.OffTimer += FrameTime;

						// Check to see if the emitter should transition to On
						if (emitter.OffTimer >= emitter.OffTime)
						{
							// Turn the emitter On
							emitter.On = true;

							// Reset the Off frame counter
							emitter.OffTimer = 0;
						}
					}
					break;
				case FlowControl.UseMarks:

					// If a mark collection has been selected then...
					if (!string.IsNullOrEmpty(emitter.MarkCollectionName))
					{
						// Get the selected mark collection
						IMarkCollection marks = MarkCollections.FirstOrDefault(item => item.Id == emitter.MarkCollectionId);

						// Determine how far into the effect we are
						int millisecondsIntoEffect = FrameTime * frameNum;

						// Create a time span for the specified number of milliseconds
						TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, millisecondsIntoEffect);

						// Gets the marks inclusive of the effect
						List<IMark> marksLeftInEffect = marks.MarksInclusiveOfTime(StartTime + timeSpan, StartTime + TimeSpan);

						// Default the emitter to Off
						emitter.On = false;

						// Loop over the remaining marks
						//foreach (IMark mark in marksLeftInEffect)
						if (marksLeftInEffect.Count > 0)
						{
							// Get the first mark that overlaps the effect (or whats left of the effect)
							IMark mark = marksLeftInEffect[0];

							// If the mark includes the current frame then...
							if (mark.StartTime <= StartTime + timeSpan &&
									 StartTime + timeSpan <= mark.EndTime)
							{
								// Turn the emitter On
								emitter.On = true;
							}
						}
					}
					break;
					case FlowControl.Musical:
						// Get the emitter flow based on the associated music volume
						emitterWrapper.Flow = CalculateEmitterMusicalFlow(emitter, (int)(frameNum * FrameTime), frameNum);
					break;
				default:
					Debug.Assert(false, "Unsupported Flow Control");
					break;
			}

			// If the emitter is intermittently On/Off then...
			if (emitter.FlowControl == FlowControl.Pulsating ||
				 emitter.FlowControl == FlowControl.UseMarks)
			{
				// If the emitter is NOT On then...
				if (!emitter.On)
				{
					// Set the flow to nothing
					emitterWrapper.Flow = 0;
				}
				else
				{
					// Get the flow from the curve
					emitterWrapper.Flow = (int)Math.Round(ScaleCurveToValue(emitter.Flow.GetValue(intervalPosFactor), MaxEmitterFlow, 0));
				}
			}			
		}

		/// <summary>
		/// Update the emitter's position.
		/// </summary>		
		private void UpdateEmitterPosition(EmitterWrapper wrapper, IEmitter emitter, double intervalPos, double intervalPosFactor)
		{			
			// Animate emitters as necessary
			AnimateEmitters(intervalPos, intervalPosFactor);

			// If the emitter is animated then...
			if (emitter.Animate)
			{
				// Transfer the position to the wrapper
				wrapper.X = (int)emitter.LocationX;
				wrapper.Y = (int)emitter.LocationY;
			}
			else
			{
				// Otherwise get the position from the curves
				wrapper.X = CalculateEmitterX(intervalPosFactor, emitter);
				wrapper.Y = CalculateEmitterY(intervalPosFactor, emitter);
			}		
		}

		/// <summary>
		/// Updates the nozzle angle of the specified emitter.
		/// </summary>		
		private void UpdateEmitterNozzleAngle(EmitterWrapper emitterWrapper, IEmitter emitter, double intervalPosFactor)
		{
			// Determine the emitter nozzle movement setting
			switch (emitter.NozzleMovement)
			{
				case NozzleMovement.Oscillate:

					// Determine the oscillation speed
					emitter.AngleOscillationDelta = CalculateEmitterOscillationSpeed(intervalPosFactor, emitter);

					// If oscillating clockwise then...
					if (emitter.OscillationClockwise)
					{
						// Update the angle by the delta factor moving clockwise
						emitter.IntAngle -= emitter.AngleOscillationDelta;

						// If the angle has passed the start angle then...
						if (emitter.IntAngle < emitter.OscillateStartAngle)
						{
							// Switch directions
							emitter.OscillationClockwise = false;
						}
					}
					else
					{
						// Update the angle by the delta factor moving counter-clockwise
						emitter.IntAngle += emitter.AngleOscillationDelta;

						// If the angle has passed the end angle then...
						if (emitter.IntAngle > emitter.OscillateEndAngle)
						{
							// Switch directions
							emitter.OscillationClockwise = true;
						}
					}

					// Update the emitter angle on the wrapper
					emitterWrapper.Angle = emitter.IntAngle;
					break;
				case NozzleMovement.FixedAngle:

					// If the emitter is animated and the nozzle movement is fixed then...
					if (emitter.Animate && emitter.NozzleMovement == NozzleMovement.FixedAngle)
					{
						// Set the nozzle angle to point behind the movement
						emitterWrapper.Angle = emitter.IntAngle;
					}
					else
					{
						// Retrieve the emitter nozzle angle from the curve
						emitterWrapper.Angle = CalculateEmitterNozzleAngle(intervalPosFactor, emitter);
					}
					break;
				case NozzleMovement.SpinClockwise:

					// Determine the oscillation speed
					emitter.AngleOscillationDelta = CalculateEmitterOscillationSpeed(intervalPosFactor, emitter);

					// Update the angle by the delta factor moving clockwise
					emitter.IntAngle -= emitter.AngleOscillationDelta;

					// If the angle has exceeded 360 degrees then...
					if (emitter.IntAngle > 360)
					{
						// Subtract off 360 degrees
						emitter.IntAngle -= 360;
					}
					// Update the emitter angle on the wrapper
					emitterWrapper.Angle = emitter.IntAngle;
					break;
				case NozzleMovement.SpinCounterClockwise:

					// Determine the oscillation speed
					emitter.AngleOscillationDelta = CalculateEmitterOscillationSpeed(intervalPosFactor, emitter);

					// Update the angle by the delta factor moving counter-clockwise
					emitter.IntAngle += emitter.AngleOscillationDelta;

					// If the angle is less than zero then...
					if (emitter.IntAngle <= 0)
					{
						// Add 360 degrees
						emitter.IntAngle += 360;
					}
					// Update the emitter angle on the wrapper
					emitterWrapper.Angle = emitter.IntAngle;
					break;
				default:
					Debug.Assert(false, "Unsupported Nozzle Angle");
					break;
			}
		}

		/// <summary>
		/// Increments the color index for the specified emitter.
		/// </summary>		
		private void IncrementColorIndex(IEmitter emitter)
		{
			// Keep track of the number of frames for the current color
			emitter.FrameColorCounter++;

			// If the effect has rendered the prescribed frames for the current color then...
			if (emitter.FrameColorCounter >= emitter.FramesPerColor)
			{
				// Move to the next color
				emitter.ColorArrayIndex++;

				// Reset the frames per color counter
				emitter.FrameColorCounter = 0;
			}

			// If the effect was on the last color then...
			if (emitter.ColorArrayIndex >= Colors.Count)
			{
				// Wrap around to the first color
				emitter.ColorArrayIndex = 0;
			}
		}

		/// <summary>
		/// Gets the current color from the array and then retrieves the color based on the gradient.
		/// </summary>		
		private Color GetColorArrayItem(IEmitter emitter, double intervalPos)
		{
			// Defend against the user removing colors
			if (emitter.ColorArrayIndex >= Colors.Count)
			{
				emitter.ColorArrayIndex = 0;
			}
			return Colors[emitter.ColorArrayIndex].GetColorAt(intervalPos);
		}

		/// <summary>
		/// Updates the color for the specified emitter.
		/// </summary>		
		private void UpdateEmitterColor(EmitterWrapper emitterWrapper, IEmitter emitter, double intervalPos, double intervalPosFactor)
		{
			Color emitterColor;

			// If using the color array get the next color from the array
			if (emitter.UseColorArray)
			{
				// Get the color from the array and the position in the gradient
				emitterColor = GetColorArrayItem(emitter, intervalPos);

				// Increment the position in the array as applicable
				IncrementColorIndex(emitter);
			}
			else
			{
				// Get the color from the position in the gradient
				emitterColor = emitter.Color.GetColorAt(intervalPos);
			}

			// Adjust the color for brightness setting
			HSV hsv = HSV.FromRGB(emitterColor);
			hsv.V *= emitter.Brightness.GetValue(intervalPosFactor) / 100;
			emitterWrapper.Color = hsv.ToRGB();
		}

		/// <summary>
		/// Converts the emitters from the emitter model into emitter wrappers to pass to the C++ Liquid Fun API.
		/// </summary>		
		private IList<EmitterWrapper> ConvertEmittersToEmitterWrappers(int frameNum, double intervalPos, double intervalPosFactor)
		{
			// Create the collection of emitter wrappers
			List<EmitterWrapper> emitterWrappers = new List<EmitterWrapper>();

			// Loop over the model emitters
			foreach (IEmitter emitter in _renderEmitterList)
			{
				// Create the emitter wrapper for interfacing with the C++
				EmitterWrapper emitterWrapper = new EmitterWrapper();

				UpdateEmitterFlow(emitterWrapper, emitter, frameNum, intervalPosFactor);
				UpdateEmitterPosition(emitterWrapper, emitter, intervalPos, intervalPosFactor);								
			   UpdateEmitterNozzleAngle(emitterWrapper, emitter, intervalPosFactor);				
				UpdateEmitterColor(emitterWrapper, emitter, intervalPos, intervalPosFactor);

				emitterWrapper.Lifetime = CalculateEmitterParticleLifetime(intervalPosFactor, emitter);				
				emitterWrapper.Velocity = CalculateEmitterVelocity(intervalPosFactor, emitter);
				emitterWrapper.SourceSize = CalculateEmitterSourceSize(intervalPosFactor, emitter);
				emitterWrapper.ParticleType = (WrapperParticleType)emitter.ParticleType;

				// Add the wrapper to the collection of wrappers
				emitterWrappers.Add(emitterWrapper);
			}

			return emitterWrappers;
		}

		/// <summary>
		/// Calculates the emitter's flow based on the music.
		/// </summary>		
		private int CalculateEmitterMusicalFlow(IEmitter emitter, int time, int frame)
		{
			// Initialize the flow to Off
			int value = 0;
			
			// Initialize the flow to the maximum musical flow
			value = MaxEmitterMusicFlow;

			// Return the volume for the current frame
			double volume = 0;
			if (frame < _frameVolumes.Count)
			{
				volume = _frameVolumes[frame];
			}

			// Conver the volume to a range from zero to 1.0
			double val = ConvertRange(_minVolume, _maxVolume, 0.0, 1.0, volume);

			// Set the flow to percentage of the max based on the volume
			value = (int)(val * value);
						
			return value;
		}

		/// <summary>
		/// Calculates the emitter's X velocity from the curve.
		/// </summary>		
		private double CalculateEmitterVelocityX(IEmitter emitter, double intervalPosFactor)
		{
			return ScaleCurveToValue(emitter.VelocityX.GetValue(intervalPosFactor), MaxEmitterAnimateSpeed, 0);
		}

		/// <summary>
		/// Calculates the emitter's Y velocity from the curve.
		/// </summary>		
		private double CalculateEmitterVelocityY(IEmitter emitter, double intervalPosFactor)
		{
			return ScaleCurveToValue(emitter.VelocityY.GetValue(intervalPosFactor), MaxEmitterAnimateSpeed, 0);
		}

		/// <summary>
		/// Calculates the emitter's particle lifetime from the curve.
		/// </summary>		
		private int CalculateEmitterParticleLifetime(double intervalPos, IEmitter emitter)
		{
			return (int)Math.Round(ScaleCurveToValue(emitter.Lifetime.GetValue(intervalPos), MaxEmitterLifeTime, 0));
		}

		/// <summary>
		/// Calculates the emitter's X position from the curve.
		/// </summary>		
		private int CalculateEmitterX(double intervalPos, IEmitter emitter)
		{
			return (int)Math.Round(ScaleCurveToValue(emitter.X.GetValue(intervalPos), _bufferWt, 0));
		}

		/// <summary>
		/// Calculates the emitter's Y position from the curve.
		/// </summary>		
		private int CalculateEmitterY(double intervalPos, IEmitter emitter)
		{
			return (int)Math.Round(ScaleCurveToValue(emitter.Y.GetValue(intervalPos), _bufferHt, 0));
		}

		/// <summary>
		/// Calculates the emitter's velocity from the curve.
		/// </summary>		
		private int CalculateEmitterVelocity(double intervalPos, IEmitter emitter)
		{
			return (int)Math.Round(ScaleCurveToValue(emitter.ParticleVelocity.GetValue(intervalPos), MaxEmitterParticleVelocity, 0));
		}

		/// <summary>
		/// Calculates the emitter's emitting source size (radius) from the curve.
		/// </summary>		
		private int CalculateEmitterSourceSize(double intervalPos, IEmitter emitter)
		{
			return (int)Math.Round(ScaleCurveToValue(emitter.SourceSize.GetValue(intervalPos), MaxEmitterSourceSize, 0));
		}

		/// <summary>
		/// Calculates the emitter's nozzle angle from the curve.
		/// </summary>		
		private int CalculateEmitterNozzleAngle(double intervalPos, IEmitter emitter)
		{
			return (int)Math.Round(ScaleCurveToValue(emitter.NozzleAngle.GetValue(intervalPos), 360, 0));
		}

		/// <summary>
		/// Calculates the oscillation speed of the specified emitter from the curve.
		/// </summary>				
		private int CalculateEmitterOscillationSpeed(double intervalPos, IEmitter emitter)
		{
			return (int)Math.Round(ScaleCurveToValue(emitter.OscillationSpeed.GetValue(intervalPos), 45, 0));
		}

		/// <summary>
		/// Animates emitters seleced for animation.
		/// </summary>		
		private void AnimateEmitters(double intervalPos, double intervalPosFactor)
		{		
			// Loop over the emitters
			foreach (IEmitter emitter1 in _renderEmitterList)
			{
				// If the emitter is animated then...
				if (emitter1.Animate)
				{
					// Store off the previous position of the emitter
					double previousXPosition = emitter1.LocationX;
					double previousYPosition = emitter1.LocationY;

					// Determine if the X velocity is negative
					bool negativeX = emitter1.VelX < 0;

					// Get the X velocity from the curve
					emitter1.VelX = CalculateEmitterVelocityX(emitter1, intervalPosFactor);

					// If the velocity was negative then...
					if (negativeX)
					{
						// Negate the velocity from the curve
						emitter1.VelX = -emitter1.VelX;
					}

					// Determine if the Y velocity is negative
					bool negativeY = emitter1.VelY < 0;

					// Get the X velocity from the curve
					emitter1.VelY = CalculateEmitterVelocityY(emitter1, intervalPosFactor);

					// If the velocity was negative then..
					if (negativeY)
					{
						// Negate the velocity from the curve
						emitter1.VelY = -emitter1.VelY;
					}

					// Move the emitter
					emitter1.LocationX = emitter1.LocationX + emitter1.VelX;
					emitter1.LocationY = emitter1.LocationY + emitter1.VelY;

					// Determine the delta from the previous position
					double deltaX = emitter1.LocationX - previousXPosition;
					double deltaY = emitter1.LocationY - previousYPosition;

					// Determine the angle of the movement
					double angleRadians = Math.Atan2(deltaY, deltaX);

					// Convert the angle to degrees from radians
					emitter1.IntAngle = (int)(angleRadians * 180 / Math.PI);				
					
					// Loop over the emitters and look for collisions
					foreach (IEmitter otherEmitter in _renderEmitterList)
					{															
						// Calculate the distance between the two emitter's centers
						double distanceBetweenCenters = Math.Sqrt((emitter1.LocationX - otherEmitter.LocationX) * (emitter1.LocationX - otherEmitter.LocationX) + 
							                                       (emitter1.LocationY - otherEmitter.LocationY) * (emitter1.LocationY -otherEmitter.LocationY));

						// Sum the radius of the two emitters
						double radiusSum = CalculateEmitterSourceSize(intervalPosFactor, emitter1) + CalculateEmitterSourceSize(intervalPosFactor, otherEmitter);

						// If the distance between the centers is less than or equal to radius the emitters have collided
						if (distanceBetweenCenters <= radiusSum)
						{
							// Reverse the velocity of both emitters
							otherEmitter.VelX = -otherEmitter.VelX;
							otherEmitter.VelY = -otherEmitter.VelY;
							emitter1.VelX = -emitter1.VelX;
							emitter1.VelY = -emitter1.VelY;					
						}
					}						
				}

				// Determine the emitting radius of the emitter
				int radius = CalculateEmitterSourceSize(intervalPos, emitter1);

				// Determine the behavior when an emitter reaches the edge of the element
				switch (emitter1.EdgeHandling)
				{
					case EdgeHandling.Bounce:
						// If the emitter is off matrix to the left then...
						if (emitter1.LocationX - radius < 0)
						{
							// Position the emitter at the left edge of the matrix
							emitter1.LocationX = radius;

							// Reverse the X velocity
							emitter1.VelX = -emitter1.VelX;
						}
						// If the emitter is off matrix to the right then...
						else if (emitter1.LocationX + radius >= _bufferWt)
						{
							// Position the emitter at the right edge of the matrix
							emitter1.LocationX = _bufferWt - radius - 1;

							// Reverse the X velocity
							emitter1.VelX = -emitter1.VelX;
						}

						// If the emitter is off matrix at the bottom then...
						if (emitter1.LocationY - radius < 0)
						{
							// Position the emitter at the bottom of the matrix
							emitter1.LocationY = radius;

							// Reverse the Y velocity
							emitter1.VelY = -emitter1.VelY;
						}
						// If the emitter is off matrix at the top then...
						else if (emitter1.LocationY + radius >= _bufferHt)
						{
							// Position the emitter at the top of the matrix
							emitter1.LocationY = _bufferHt - radius - 1;

							// Reverse the Y velocity
							emitter1.VelY = -emitter1.VelY;
						}
						break;

					case EdgeHandling.Wrap:

						// If the emitter is off matrix to the left then...
						if (emitter1.LocationX + radius < 0)
						{
							// Move the emitter X to the right side of the matrix just out of view
							emitter1.LocationX += _bufferWt + (radius * 2);
						}

						// If the emitter is off matrix to the bottom then...
						if (emitter1.LocationY + radius < 0)
						{
							// Move the emitter to the top just out of view
							emitter1.LocationY += _bufferHt + (radius * 2);
						}

						// If the emitter is off matrix to the right then...
						if (emitter1.LocationX - radius > _bufferWt)
						{
							// Move the emitter to the left just out of view
							emitter1.LocationX -= _bufferWt + (radius * 2);
						}

						// If the emitter is off matrix to the top then...
						if (emitter1.LocationY - radius > _bufferHt)
						{
							// Move the emitter to the bottom just out of view
							emitter1.LocationY -= _bufferHt + (radius * 2);
						}
						break;
					default:
						Debug.Assert(false, "Unsuppported Edge Handling Value");
						break;
				}
			}			
		}

		/// <summary>
		/// Updates the serialized effect data from the emitter collection.
		/// </summary>
		private void UpdateLiquidData()
		{
			// Clear the collection of serialized emitters
			_data.EmitterData.Clear();

			// Loop over the emitters in the model emitter collection
			foreach (IEmitter emitter in EmitterList.ToList())
			{
				// Create a newe serialized emitter
				EmitterData serializedEmitter = new EmitterData();

				// Transfer the properties from the emitter model to the serialized emitter
				serializedEmitter.ParticleType = _particleTypeToSerializedParticleType[emitter.ParticleType];
				serializedEmitter.UseColorArray = emitter.UseColorArray;
				serializedEmitter.FramesPerColor = emitter.FramesPerColor;
				serializedEmitter.Color = new ColorGradient(emitter.Color);
				serializedEmitter.Lifetime = new Curve(emitter.Lifetime);
				serializedEmitter.ParticleVelocity = new Curve(emitter.ParticleVelocity);
				serializedEmitter.Animate = emitter.Animate;
				serializedEmitter.RandomStartingPosition = emitter.RandomStartingPosition;
				serializedEmitter.AnimateXStart = emitter.AnimateXStart;
				serializedEmitter.AnimateYStart = emitter.AnimateYStart;
				serializedEmitter.EdgeHandling = _edgeHandlingToSerializedEdgeHandling[emitter.EdgeHandling];
				serializedEmitter.VelocityX = new Curve(emitter.VelocityX);
				serializedEmitter.VelocityY = new Curve(emitter.VelocityY);
				serializedEmitter.X = new Curve(emitter.X);
				serializedEmitter.Y = new Curve(emitter.Y);
				serializedEmitter.Flow = new Curve(emitter.Flow);
				serializedEmitter.SourceSize = new Curve(emitter.SourceSize);
				serializedEmitter.NozzleMovement = _nozzleAngleToSerializedNozzleAngle[emitter.NozzleMovement];
				serializedEmitter.NozzleAngle = new Curve(emitter.NozzleAngle);				
				serializedEmitter.OscillateStartAngle = emitter.OscillateStartAngle;
				serializedEmitter.OscillateEndAngle = emitter.OscillateEndAngle;
				serializedEmitter.OscillationSpeed = new Curve(emitter.OscillationSpeed);
				serializedEmitter.FlowControl = _flowControlToSerializedFlowControl[emitter.FlowControl];
				serializedEmitter.MarkCollectionId = emitter.MarkCollectionId;										
				serializedEmitter.OnTime = emitter.OnTime;
				serializedEmitter.OffTime = emitter.OffTime;
				serializedEmitter.Brightness = new Curve(emitter.Brightness);
				
				// Add the serialized emitter to the collection
				_data.EmitterData.Add(serializedEmitter);
			}
		}

		/// <summary>
		/// Updates the emitter model from the serialized effect data.
		/// </summary>
		/// <param name="liquidData">(Serialized) effect data</param>		
		private void UpdateEmitterModel(LiquidData liquidData)
		{
			// Clear the view model emitter collection
			EmitterList.Clear();

			// Loop over the emitters in the serialized effect data
			foreach (EmitterData emitter in liquidData.EmitterData)
			{
				// Create a new emitter in the model
				var emitterModel = new Emitter();

				// Transfer the properties from the serialized effect data to the emitter model
				emitterModel.ParticleType = _serializedParticleTypeToParticleType[emitter.ParticleType];
				emitterModel.UseColorArray = emitter.UseColorArray;
				emitterModel.FramesPerColor = emitter.FramesPerColor;
				emitterModel.Color = emitter.Color;
				emitterModel.Lifetime = new Curve(emitter.Lifetime);
				emitterModel.ParticleVelocity = new Curve(emitter.ParticleVelocity);
				emitterModel.Animate = emitter.Animate;
				emitterModel.RandomStartingPosition = emitter.RandomStartingPosition;
				emitterModel.AnimateXStart = emitter.AnimateXStart;
				emitterModel.AnimateYStart = emitter.AnimateYStart;
				emitterModel.EdgeHandling = _serializedEdgeHandlingToEdgeHandling[emitter.EdgeHandling];
				emitterModel.VelocityX = new Curve(emitter.VelocityX);
				emitterModel.VelocityY = new Curve(emitter.VelocityY);
				emitterModel.X = new Curve(emitter.X);
				emitterModel.Y = new Curve(emitter.Y);
				emitterModel.Flow = new Curve(emitter.Flow);
				emitterModel.SourceSize = new Curve(emitter.SourceSize);
				emitterModel.NozzleMovement = _serializedNozzleAngleToNozzleAngle[emitter.NozzleMovement];
				emitterModel.NozzleAngle = new Curve(emitter.NozzleAngle);				
				emitterModel.OscillateStartAngle = emitter.OscillateStartAngle;
				emitterModel.OscillateEndAngle = emitter.OscillateEndAngle;
				emitterModel.OscillationSpeed = new Curve(emitter.OscillationSpeed);
				emitterModel.FlowControl = _serializedFlowControlToFlowControl[emitter.FlowControl];
				emitterModel.MarkCollectionId = emitter.MarkCollectionId;
				emitterModel.OnTime = emitter.OnTime;
				emitterModel.OffTime = emitter.OffTime;
				
				// The Brightness property was omitted from the serialization in the initial release of this effect
				// so we need to check for null to support existing sequences
				if (emitter.Brightness != null)
				{
					emitterModel.Brightness = new Curve(emitter.Brightness);
				}
				emitterModel.MarkNameCollection = _markCollectionNames;
				emitterModel.MarkCollections = MarkCollections;
				emitterModel.InitAllAttributes();
				// Add the emitter to the emitter collection
				EmitterList.Add(emitterModel);
			}
		}

		/// <summary>
		/// Updates the collection of mark collection names.
		/// </summary>
		private void UpdateMarkCollectionNames()
		{
			// Save off any selected Mark Collections
			List<Guid> selectedMarkcollectionGuid = new List<Guid>();
			foreach (IEmitter emitter in EmitterList)
			{
				selectedMarkcollectionGuid.Add(emitter.MarkCollectionId);
			}

			// Clear the mark collection names
			_markCollectionNames.Clear();

			// Loop through the mark collections
			foreach (IMarkCollection collection in MarkCollections)
			{
				// Add the mark collection names to the collection
				_markCollectionNames.Add(collection.Name);
			}

			// Check to see if selected mark collection names need to be updated
			EmitterList.UpdateSelectedMarkCollectionNames();

			// Restore the selected mark collections
			int index = 0;
			foreach (IEmitter emitter in EmitterList)
			{
				emitter.MarkCollectionId = selectedMarkcollectionGuid[index];				
				index++;
			}			
		}

		/// <summary>
		/// Event for when a property changes on a mark collection.
		/// </summary>		
		private void MarkCollectionPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			// If a mark collection name changed then...
			if (e.PropertyName == "Name")
			{				
				// Update the collection of mark collection names
				UpdateMarkCollectionNames();				
			}
		}
		
		/// <summary>
		/// Updates the frame buffer for a location based pixel.
		/// </summary>
		private void UpdateFrameBufferForLocationPixel(int x, int y, int bufferHt, FastPixel.FastPixel bitmap, IPixelFrameBuffer frameBuffer)
		{
			// Save off the original location node
			int yCoord = y;
			int xCoord = x;
			
			// Flip me over so and offset my coordinates I can act like the string version
			y = Math.Abs((BufferHtOffset - y) + (bufferHt - 1 + BufferHtOffset));
			y = y - BufferHtOffset;
			x = x - BufferWiOffset;

			// Scale the location down to the render bitmap so that we can retrieve the color of the pixel
			int scaledX = x / _renderScaleFactor;
			int scaledY = y / _renderScaleFactor;

			// Retrieve the color from the bitmap
			Color color = bitmap.GetPixel(scaledX, scaledY);

			// Set the pixel on the frame buffer
			frameBuffer.SetPixel(xCoord, yCoord, color);
		}

		/// <summary>
		/// Updates the browseable state of the audio properties.
		/// </summary>		
		private void UpdateAudioAttributes(bool refresh = true)
		{
			bool emittersWithMusicalFlow = EmitterList.Any(emit => emit.FlowControl == FlowControl.Musical);

			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(13);
			propertyStates.Add("Sensitivity", emittersWithMusicalFlow && AudioAssociated());
			propertyStates.Add("LowPass", emittersWithMusicalFlow && AudioAssociated());
			propertyStates.Add("LowPassFreq", emittersWithMusicalFlow && AudioAssociated());
			propertyStates.Add("HighPass", emittersWithMusicalFlow && AudioAssociated());
			propertyStates.Add("HighPassFreq", emittersWithMusicalFlow && AudioAssociated());
			propertyStates.Add("Range", emittersWithMusicalFlow && AudioAssociated());
			propertyStates.Add("Normalize", emittersWithMusicalFlow && AudioAssociated());
			propertyStates.Add("DecayTime", emittersWithMusicalFlow && AudioAssociated());
			propertyStates.Add("AttackTime", emittersWithMusicalFlow && AudioAssociated());
			propertyStates.Add("Gain", emittersWithMusicalFlow && AudioAssociated());						
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}

			UpdateLowHighPassAttributes(refresh);
		}

		/// <summary>
		/// Event handler for when a child property within the EmitterList is modified.
		/// </summary>		
		private void EmitterListChildPropertyChanged(object sender, PropertyChangedEventArgs e)
		{	
			// Updates the browseable state of the audio attributes
			UpdateAudioAttributes();

			// Update the browseable state of the color list collection
			UpdateColorListAttributes();
		}
		
		#endregion

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/Liquid/"; }
		}

		#endregion
	}
}
