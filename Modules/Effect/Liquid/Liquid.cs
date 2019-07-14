using Common.Controls.ColorManagement.ColorModels;
using FastPixel;
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
		private const int MaxEmitterVelocity = 50;
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
		/// Dictionaries to convert from view model enumerations to serialization enumerations.
		/// </summary>
		private Dictionary<ParticleType, EmitterData.ParticleSerializationType> _particleTypeToSerializedParticleType;
		private Dictionary<EdgeHandling, EmitterData.EdgeHandlingSerializationType> _edgeHandlingToSerializedEdgeHandling;
		private Dictionary<NozzleAngle, EmitterData.NozzleAngleSerializationType> _nozzleAngleToSerializedNozzleAngle;
		private Dictionary<FlowControl, EmitterData.FlowControlSerializationType> _flowControlToSerializedFlowControl;

		/// <summary>
		/// Dictionaries to convert from serialization enumerations to view model enumerations.
		/// </summary>
		private Dictionary<EmitterData.ParticleSerializationType, ParticleType> _serializedParticleTypeToParticleType;
		private Dictionary<EmitterData.EdgeHandlingSerializationType, EdgeHandling> _serializedEdgeHandlingToEdgeHandling;
		private Dictionary<EmitterData.NozzleAngleSerializationType, NozzleAngle> _serializedNozzleAngleToNozzleAngle;
		private Dictionary<EmitterData.FlowControlSerializationType, FlowControl> _serializedFlowControlToFlowControl;

		/// <summary>
		/// Copy of emitter list that is used during rendering.
		/// A copy is made in an attempt to avoid the collection being modified while the effect is rendering.
		/// </summary>
		private List<IEmitter> _renderEmitterList;

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

			// Give the emitter a reference to the effect.
			// This is needed so that the emitters can use the parent to register (listen) for mark collection events
			EmitterList.Parent = this;

			// Initialize the wrapper around the Liquid Fun API
			_liquidFunWrapper = new LiquidFunWrapper();

			// Initialize the audio utilities
			AudioUtilities = new AudioUtilities();
			
			// Initialize the dictionaries that convert between view model enumeration types and the serialization model enumeration types
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

			_nozzleAngleToSerializedNozzleAngle = new Dictionary<NozzleAngle, EmitterData.NozzleAngleSerializationType>();
			_nozzleAngleToSerializedNozzleAngle.Add(NozzleAngle.FixedAngle, EmitterData.NozzleAngleSerializationType.FixedAngle);
			_nozzleAngleToSerializedNozzleAngle.Add(NozzleAngle.Oscillate, EmitterData.NozzleAngleSerializationType.Oscillate);
			_nozzleAngleToSerializedNozzleAngle.Add(NozzleAngle.SpinClockwise, EmitterData.NozzleAngleSerializationType.SpinClockwise);
			_nozzleAngleToSerializedNozzleAngle.Add(NozzleAngle.SpinCounterClockwise, EmitterData.NozzleAngleSerializationType.SpinCounterClockwise);

			_serializedNozzleAngleToNozzleAngle = new Dictionary<EmitterData.NozzleAngleSerializationType, NozzleAngle>();
			_serializedNozzleAngleToNozzleAngle.Add(EmitterData.NozzleAngleSerializationType.FixedAngle, NozzleAngle.FixedAngle);
			_serializedNozzleAngleToNozzleAngle.Add(EmitterData.NozzleAngleSerializationType.Oscillate, NozzleAngle.Oscillate);
			_serializedNozzleAngleToNozzleAngle.Add(EmitterData.NozzleAngleSerializationType.SpinClockwise, NozzleAngle.SpinClockwise);
			_serializedNozzleAngleToNozzleAngle.Add(EmitterData.NozzleAngleSerializationType.SpinCounterClockwise, NozzleAngle.SpinCounterClockwise);

			_flowControlToSerializedFlowControl = new Dictionary<FlowControl, EmitterData.FlowControlSerializationType>();
			_flowControlToSerializedFlowControl.Add(FlowControl.Continuous, EmitterData.FlowControlSerializationType.Continuous);
			_flowControlToSerializedFlowControl.Add(FlowControl.Pulsating, EmitterData.FlowControlSerializationType.Pulsating);
			_flowControlToSerializedFlowControl.Add(FlowControl.UseMarks, EmitterData.FlowControlSerializationType.UseMarks);

			_serializedFlowControlToFlowControl = new Dictionary<EmitterData.FlowControlSerializationType, FlowControl>();
			_serializedFlowControlToFlowControl.Add(EmitterData.FlowControlSerializationType.Continuous, FlowControl.Continuous);
			_serializedFlowControlToFlowControl.Add(EmitterData.FlowControlSerializationType.Pulsating, FlowControl.Pulsating);
			_serializedFlowControlToFlowControl.Add(EmitterData.FlowControlSerializationType.UseMarks, FlowControl.UseMarks);
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
				// Convert the view model into the serialized data structure
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

				// Convert the serialized data structure into the emitter view model
				UpdateViewModel(_data);

				// Updates whether the render scale factor is visible
				UpdateRenderScaleFactorAttributes(false);

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
		[ProviderDisplayName(@"ParticleSize")]
		[ProviderDescription(@"ParticleSize")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 10000, 1)]
		[PropertyOrder(5)]
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
		[ProviderCategory(@"Colors", 2)]
		[ProviderDisplayName(@"ColorArray")]
		[ProviderDescription(@"ColorArray")]
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

		private EmitterCollection _emitterList;

		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Emitters")]
		[ProviderDescription(@"Emitters")]
		[PropertyOrder(6)]
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
			// Tuple is composed of position in x, y and then color of the particle
			Tuple<int, int, Color>[] particles = _liquidFunWrapper.GetParticles();

			// Turn on pixels for the particles
			foreach (Tuple<int, int, Color> particle in particles)
			{
				frameBuffer.SetPixel(
					 x: particle.Item1,
					 y: particle.Item2,
					 c: particle.Item3);
			}
		}

		/// <summary>
		/// Renders the effect by location.
		/// </summary>		
		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{			
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
				// Tuple is composed of position in x, y and then color of the particle
				Tuple<int, int, Color>[] particles = _liquidFunWrapper.GetParticles();

				// Create a bitmap the size of the logical render area
				// Note this render area is smaller matrix than the actual display elements
				using (Bitmap bitmap = new Bitmap(_bufferWt, _bufferHt))
				{					
					// Loop over each of the liquid particles
					foreach (Tuple<int, int, Color> particle in particles)
					{
						// If the particle is within the viewable area then...
						if ((particle.Item1 >= 0 && particle.Item1 < _bufferWt) &&
							 (particle.Item2 >= 0 && particle.Item2 < _bufferHt))
						{
							// Set the corresponding pixel in the bitmap
							bitmap.SetPixel(
								particle.Item1,
								particle.Item2,
								particle.Item3);
						}
					}
																				
					// Convert logical render area to the actual render area
					using (Bitmap largerBitmap = new Bitmap(bitmap, BufferWi, BufferHt))
					{
						// Transfer the pixel data from the bitmap to the frame buffer
						using (FastPixel.FastPixel fastPixel= new FastPixel.FastPixel(largerBitmap))
						{
							fastPixel.Lock();
							foreach (ElementLocation elementLocation in frameBuffer.ElementLocations)
							{								
								UpdateFrameBufferForLocationPixel(elementLocation.X, elementLocation.Y, BufferHt, fastPixel, frameBuffer);								
							}
							fastPixel.Unlock(false);
						}						
					}						
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
				_bufferWt = BufferWi / RenderScaleFactor;
				_bufferHt = BufferHt / RenderScaleFactor; 
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
					// Initialize the position of the emitter with a random position
					emitter.LocationX = Rand(0, _bufferWt);
					emitter.LocationY = Rand(0, _bufferHt);
				}

				// Reset the On/Off Timers
				emitter.OnTimer = 0;
				emitter.OffTimer = 0;
			}

			GetAudioSettings();
			CacheOffAudioVolumes();
		}

		#endregion

		#region Private Properties

		[Browsable(false)]
		private AudioUtilities AudioUtilities { get; set; }

		#endregion

		#region Private Audio Methods

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
		/// Updates the emitter's flow and updates the specified emitter wrapper.
		/// </summary>		
		private void UpdateEmitterFlow(EmitterWrapper emitterWrapper, IEmitter emitter, int frameNum, double intervalPosFactor)
		{
			// Update the flow based on the emitter's flow control
			switch (emitter.FlowControl)
			{
				case FlowControl.Continuous:
					// Continuous On (do nothing)
					break;
				case FlowControl.Pulsating:
					const int FramesPerSecond = 1000 / FrameTime;

					// If the emitter is On then...
					if (emitter.On)
					{
						// Keep track of the number of frames the emitter is on
						emitter.OnTimer++;

						// Check to see if the emitter should transition to Off
						if (emitter.OnTimer == emitter.OnTime * FramesPerSecond)
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
						emitter.OffTimer++;

						// Check to see if the emitter should transition to On
						if (emitter.OffTimer == emitter.OffTime * FramesPerSecond)
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
				default:
					Debug.Assert(false, "Unsupported Flow Control");
					break;
			}

			// If the emitter is intermittently On/Off then...
			if (emitter.OnOff)
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
					emitterWrapper.Flow = CalculateEmitterFlow(intervalPosFactor, emitter, (int)(frameNum * FrameTime), frameNum);
				}
			}
			else
			{
				// Emitter is Continously On get the flow from the curve
				emitterWrapper.Flow = CalculateEmitterFlow(intervalPosFactor, emitter, (int)(frameNum * FrameTime), frameNum);
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
			// Determine the emitter nozzle angle setting
			switch (emitter.NozzleAngle)
			{
				case NozzleAngle.Oscillate:

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
				case NozzleAngle.FixedAngle:

					// Retrieve the emitter nozzle angle from the curve
					emitterWrapper.Angle = CalculateEmitterNozzleAngle(intervalPosFactor, emitter);
					break;
				case NozzleAngle.SpinClockwise:

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
				case NozzleAngle.SpinCounterClockwise:

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
			hsv.V *= emitter.Brightness.GetValue(intervalPos) / 100;
			emitterWrapper.Color = hsv.ToRGB();
		}

		/// <summary>
		/// Converts the emitters from the view model into emitter wrappers to pass to the C++ Liquid Fun API.
		/// </summary>		
		private IList<EmitterWrapper> ConvertEmittersToEmitterWrappers(int frameNum, double intervalPos, double intervalPosFactor)
		{
			// Create the collection of emitter wrappers
			List<EmitterWrapper> emitterWrappers = new List<EmitterWrapper>();

			// Loop over the view model emitters
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
		/// Calculates the emitter's flow.
		/// </summary>		
		private int CalculateEmitterFlow(double intervalPos, IEmitter emitter, int time, int frame)
		{
			// Initialize the flow to Off
			int value = 0;

			// If the emitter is using musical volume flow then...
			if (emitter.FlowMatchesMusic)
			{
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
			}
			else
			{
				// Otherwise get the flow from the curve
				value = (int)Math.Round(ScaleCurveToValue(emitter.Flow.GetValue(intervalPos), MaxEmitterFlow, 0));
			}

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
			return (int)Math.Round(ScaleCurveToValue(emitter.ParticleVelocity.GetValue(intervalPos), MaxEmitterVelocity, 0));
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
			return (int)Math.Round(ScaleCurveToValue(emitter.Angle.GetValue(intervalPos), 360, 0));
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
				if (emitter1.Animate)
				{
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

					// Loop over the other emitters and look for collisions
					foreach (IEmitter otherEmitter in _renderEmitterList)
					{
						int maxRadius = Math.Max(CalculateEmitterSourceSize(intervalPosFactor, emitter1),
														  CalculateEmitterSourceSize(intervalPosFactor, otherEmitter));

						if ((emitter1.LocationX + maxRadius >= otherEmitter.LocationX && emitter1.LocationX - maxRadius < otherEmitter.LocationX &&
								emitter1.LocationY + maxRadius >= otherEmitter.LocationY && emitter1.LocationY - maxRadius < otherEmitter.LocationY) &&
								emitter1 != otherEmitter)
						{
							otherEmitter.VelX = -otherEmitter.VelX;
							otherEmitter.VelY = -otherEmitter.VelY;
							emitter1.VelX = -emitter1.VelX;
							emitter1.VelY = -emitter1.VelY;
							otherEmitter.LocationY = otherEmitter.LocationY + otherEmitter.VelY;
							otherEmitter.LocationX = otherEmitter.LocationX + otherEmitter.VelX;
							emitter1.LocationY = emitter1.LocationY + emitter1.VelY;
							emitter1.LocationX = emitter1.LocationX + emitter1.VelX;
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
		/// Updates the serialized effect data from the view model.
		/// </summary>
		private void UpdateLiquidData()
		{
			// Clear the collection of serialized emitters
			_data.EmitterData.Clear();

			// Loop over the emitters in the view model
			foreach (IEmitter emitter in EmitterList.ToList())
			{
				// Create a newe serialized emitter
				EmitterData serializedEmitter = new EmitterData();

				// Transfer the properties from the view model to the serialized emitter
				serializedEmitter.ParticleType = _particleTypeToSerializedParticleType[emitter.ParticleType];
				serializedEmitter.UseColorArray = emitter.UseColorArray;
				serializedEmitter.FramesPerColor = emitter.FramesPerColor;
				serializedEmitter.Color = new ColorGradient(emitter.Color);
				serializedEmitter.Lifetime = new Curve(emitter.Lifetime);
				serializedEmitter.ParticleVelocity = new Curve(emitter.ParticleVelocity);
				serializedEmitter.Animate = emitter.Animate;
				serializedEmitter.EdgeHandling = _edgeHandlingToSerializedEdgeHandling[emitter.EdgeHandling];
				serializedEmitter.VelocityX = new Curve(emitter.VelocityX);
				serializedEmitter.VelocityY = new Curve(emitter.VelocityY);
				serializedEmitter.X = new Curve(emitter.X);
				serializedEmitter.Y = new Curve(emitter.Y);
				serializedEmitter.ManualFlow = emitter.ManualFlow;
				serializedEmitter.Flow = new Curve(emitter.Flow);
				serializedEmitter.SourceSize = new Curve(emitter.SourceSize);
				serializedEmitter.NozzleAngle = _nozzleAngleToSerializedNozzleAngle[emitter.NozzleAngle];
				serializedEmitter.Angle = new Curve(emitter.Angle);
				serializedEmitter.Oscillate = emitter.Oscillate;
				serializedEmitter.OscillateStartAngle = emitter.OscillateStartAngle;
				serializedEmitter.OscillateEndAngle = emitter.OscillateEndAngle;
				serializedEmitter.OscillationSpeed = new Curve(emitter.OscillationSpeed);
				serializedEmitter.FlowControl = _flowControlToSerializedFlowControl[emitter.FlowControl];
				serializedEmitter.MarkCollectionId = emitter.MarkCollectionId;										
				serializedEmitter.OnTime = emitter.OnTime;
				serializedEmitter.OffTime = emitter.OffTime;

				// Add the serialized emitter to the collection
				_data.EmitterData.Add(serializedEmitter);
			}
		}

		/// <summary>
		/// updates the view model from the serialized effect data.
		/// </summary>
		/// <param name="liquidData">(Serialized) effect data</param>		
		private void UpdateViewModel(LiquidData liquidData)
		{
			// Clear the view model emitter collection
			EmitterList.Clear();

			// Loop over the emitters in the serialized effect data
			foreach (EmitterData emitter in liquidData.EmitterData)
			{
				// Create a new emitter in the view model
				IEmitter emitterViewModel = new Emitter();

				// Transfer the properties from the serialized effect data to the emitter view model
				emitterViewModel.ParticleType = _serializedParticleTypeToParticleType[emitter.ParticleType];
				emitterViewModel.UseColorArray = emitter.UseColorArray;
				emitterViewModel.FramesPerColor = emitter.FramesPerColor;
				emitterViewModel.Color = emitter.Color;
				emitterViewModel.Lifetime = new Curve(emitter.Lifetime);
				emitterViewModel.ParticleVelocity = new Curve(emitter.ParticleVelocity);
				emitterViewModel.Animate = emitter.Animate;
				emitterViewModel.EdgeHandling = _serializedEdgeHandlingToEdgeHandling[emitter.EdgeHandling];
				emitterViewModel.VelocityX = new Curve(emitter.VelocityX);
				emitterViewModel.VelocityY = new Curve(emitter.VelocityY);
				emitterViewModel.X = new Curve(emitter.X);
				emitterViewModel.Y = new Curve(emitter.Y);
				emitterViewModel.ManualFlow = emitter.ManualFlow;
				emitterViewModel.Flow = new Curve(emitter.Flow);
				emitterViewModel.SourceSize = new Curve(emitter.SourceSize);
				emitterViewModel.NozzleAngle = _serializedNozzleAngleToNozzleAngle[emitter.NozzleAngle];
				emitterViewModel.Angle = new Curve(emitter.Angle);
				emitterViewModel.Oscillate = emitter.Oscillate;
				emitterViewModel.OscillateStartAngle = emitter.OscillateStartAngle;
				emitterViewModel.OscillateEndAngle = emitter.OscillateEndAngle;
				emitterViewModel.OscillationSpeed = new Curve(emitter.OscillationSpeed);
				emitterViewModel.FlowControl = _serializedFlowControlToFlowControl[emitter.FlowControl];
				emitterViewModel.MarkCollectionId = emitter.MarkCollectionId;
				emitterViewModel.OnTime = emitter.OnTime;
				emitterViewModel.OffTime = emitter.OffTime;
				emitterViewModel.MarkNameCollection = _markCollectionNames;
				emitterViewModel.MarkCollections = MarkCollections;

				// Add the emitter to the view model emitter collection
				EmitterList.Add(emitterViewModel);
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
				if (emitter.InEdit != null)
				{
					emitter.InEdit.MarkCollectionId = selectedMarkcollectionGuid[index];
				}
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
			
			// Retrieve the color from the bitmap
			Color color = bitmap.GetPixel(x, y); 
			frameBuffer.SetPixel(xCoord, yCoord, color);
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
