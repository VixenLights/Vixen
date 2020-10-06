using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Vixen.Attributes;
using Vixen.Marks;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Wave
{
	/// <summary>
	/// Wave pixel effect.
	/// </summary>
	public class Wave : PixelEffectBase
	{
		#region Private Types

		/// <summary>
		/// This enumeration is used by the Ivey wave to control looping.
		/// </summary>
		private enum LoopControl
		{
			BreakLoop,
			ContinueLoop,
			Normal
		};

		#endregion

		#region Private Constants

		private const int MaxWaveVelocity = 10;
		private const int MaxWaveThickness = 100;
		private const int MaxWaveFrequency = 3600;
		private const int MinWaveFrequency = 180;

		private const int MaxWaveHeight = 100;
		private const int MinWaveYOffset = -250;
		private const int MaxWaveYOffset = 250;

		private const double pi_180 = Math.PI / 180;

		#endregion

		#region Private Fields

		/// <summary>
		/// Data associated with the effect.
		/// </summary>
		private WaveData _data;
		
		/// <summary>
		/// Random number generator for Ivey waveform.
		/// </summary>
		private Random _rand = new Random();
		
		/// <summary>
		/// Logical buffer height.
		/// Note this height might not match the actual effect height when the effect is operating in Location mode.
		/// </summary>
		private int _bufferHt;

		/// <summary>
		/// Logical buffer height.
		/// Note this width might not match the actual effect width when the effect is operating in Location mode.
		/// </summary>
		private int _bufferWi;

		/// <summary>
		/// Number of columns to render each frame.
		/// </summary>
		private int _speedIncrement;

		/// <summary>
		/// This effect is designed around a 50 ms refresh rate.  This sets a base slow speed for the effect.
		/// No matter what the Vixen refresh rate is set to the this effect is going to update the Waves
		/// at a 20 Hz rate.
		/// This could be improved in the future.
		/// </summary>
		private int _50msFrameNumber;

		/// <summary>
		/// Since Vixen can be refreshing at any rate, this field keeps track of the frame time so that
		/// we can render the effect as close to 20 Hz as possible.
		/// </summary>
		private int _frameTime;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public Wave()
		{
			// Enable both string and location positioning
			EnableTargetPositioning(true, true);

			// Initialize the default liquid effect data
			_data = new WaveData();

			// Create the collection of waves
			_waves = new WaveFormCollection();

			// Give the wave a reference to the effect.
			// This is needed so that the waveforms can use the parent to register (listen) for mark collection events
			Waves.Parent = this;
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

		/// <summary>
		/// Module data associated with the effect.
		/// </summary>
		public override IModuleDataModel ModuleData
		{
			get
			{
				// Convert the model into the serialized data structure
				UpdateWaveData();

				// Return the effect data
				return _data;
			}
			set
			{
				// Save off the data for the effect
				_data = value as WaveData;

				// Update the wave model data
				UpdateWaveModel(_data);

				// Determine if the Render Scale Factor should be visible				
				UpdateAttributes();

				// Mark the effect as dirty
				MarkDirty();
			}
		}

		private WaveFormCollection _waves;
		
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Waves")]
		[ProviderDescription(@"Waves")]
		[PropertyOrder(2)]
		public WaveFormCollection Waves
		{
			get
			{
				return _waves;
			}
			set
			{
				_waves = value;
				MarkDirty();
				OnPropertyChanged();
			}
		}
		
		#endregion

		#region Protected Methods

		/// <summary>
		/// Gets the data associated with the effect.
		/// </summary>
		protected override EffectTypeModuleData EffectModuleData
		{
			get
			{
				UpdateWaveData();
				return _data;
			}
		}

		/// <summary>
		/// Releases resources from the rendering process.
		/// </summary>
		protected override void CleanUpRender()
		{
		}

		/// <summary>
		/// Renders the effect by location.
		/// </summary>		
		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			// Make a local copy that is faster than the logic to get it for reuse.
			var localBufferHt = BufferHt;

			// Create a virtual matrix based on the rendering scale factor
			PixelFrameBuffer virtualFrameBuffer = new PixelFrameBuffer(_bufferWi, _bufferHt);

			// Loop over the frames
			for (int frameNum = 0; frameNum < numFrames; frameNum++)
			{
				//Assign the current frame
				frameBuffer.CurrentFrame = frameNum;

				// Render the effet to the virtual frame buffer
				RenderEffect(frameNum, virtualFrameBuffer);

				// Loop through the sparse matrix
				foreach (ElementLocation elementLocation in frameBuffer.ElementLocations)
				{
					// Lookup the pixel from the virtual frame buffer
					UpdateFrameBufferForLocationPixel(
						elementLocation.X,
						elementLocation.Y,
						localBufferHt,
						virtualFrameBuffer,
						frameBuffer);
				}
				
				virtualFrameBuffer.ClearBuffer();
			}
		}

		/// <summary>
		/// Updates the frame buffer for a location based pixel.
		/// </summary>
		private void UpdateFrameBufferForLocationPixel(int x, int y, int bufferHt, IPixelFrameBuffer tempFrameBuffer, IPixelFrameBuffer frameBuffer)
		{
			// Save off the original location node
			int yCoord = y;
			int xCoord = x;

			// Flip me over so and offset my coordinates I can act like the string version
			y = Math.Abs((BufferHtOffset - y) + (bufferHt - 1 + BufferHtOffset));
			y = y - BufferHtOffset;
			x = x - BufferWiOffset;
			
			// Retrieve the color from the bitmap
			Color color = tempFrameBuffer.GetColorAt(x, y);

			// Set the pixel on the frame buffer
			frameBuffer.SetPixel(xCoord, yCoord, color);
		}

		/// <summary>
		/// Render's the effect for the specified frame.
		/// </summary>		
		protected override void RenderEffect(int frameNum, IPixelFrameBuffer frameBuffer)
		{
			// Get the position within the effect
			double intervalPos = GetEffectTimeIntervalPosition(frameNum);
			double intervalPosFactor = intervalPos * 100;

			// Accumulate the frame time 
			_frameTime += FrameTime;

			// If at least 50ms has expired since the last time we updated the wave then...
			if (_frameTime >= 50)
			{
				// Calculate how many 50 ms frames we need to process
				int logicalFrames = _frameTime / 50;

				// Loop over the logical 50ms frames
				for (int index = 0; index < logicalFrames; index++)
				{
					// Loop over the waves in the effect
					foreach (IWaveform wave in Waves.ToList())
					{
						// If the wave has been primed then...
						if (wave.PrimeWave)
						{
							// Advance the frame for the priming
							_50msFrameNumber += wave.BufferWi;
						}

						IPixelFrameBuffer waveframeBuffer = null;

						// Only want to actually update the frame buffer if we are 
						// processing a true Vixen frame.  If we are processing a 
						// logical 50ms frame then we just want to update the wave 
						// data structures but not render pixels.  Setting the frame
						// buffer to null will prevent rendering pixels.
						if (index == logicalFrames - 1)
						{
							waveframeBuffer = frameBuffer;
						}

						// Render the specified wave
						RenderWave(
							wave,
							_50msFrameNumber,
							wave.Mirror,
							CalculateWaveFrequency(wave, intervalPosFactor),
							CalculateWaveThickness(wave, intervalPosFactor),
							CalculateWaveHeight(wave, intervalPosFactor),
							CalculateWaveSpeed(wave, intervalPosFactor),
							CalculateWaveYOffset(wave, intervalPosFactor),
							GetColor(wave, intervalPos),
							wave.WaveType,
							wave.PhaseShift,
							waveframeBuffer,
							wave,
							wave.Direction);
					}

					// Advance to the next logical frame
					_50msFrameNumber++;
					_frameTime -= 50;
				}
			}
			else
			{
				// Update all the waves with existing pixel data from the last frame
				foreach (IWaveform wave in Waves.ToList())
				{					
					// This else block handles when the Vixen refresh rate is faster than
					// 20 Hz.  In this case we just repeat the last frame of Wave data
					// until the next 50 Hz.
					RenderColumns(
						wave,
						frameBuffer,
						wave.Direction,
						CalculateWaveSpeed(wave, intervalPosFactor),
						false);
				}
			}
		}
		
		/// <summary>
		/// Setup for rendering.
		/// </summary>
		protected override void SetupRender()
		{					
			// Store off the matrix width and height
			_bufferWi = BufferWi;
			_bufferHt = BufferHt;

			// Reset the frame counter
			_50msFrameNumber = 0;

			// Reset the frame time within the effect
			_frameTime = 0;

			// Determine the width of the matrix when in string mode
			double matrixWidth;
			if (TargetPositioning == TargetPositioningType.Strings)
			{
				matrixWidth = BufferWi;
			}
			else
			{
				matrixWidth = MaxPixelsPerString;
			}

			// Determine the speed of the wave.  In string mode the default (minimum) speed is one column per frame.
			_speedIncrement = (int)Math.Round(BufferWi / matrixWidth, MidpointRounding.AwayFromZero);
			
			// Loop over the waves
			foreach (IWaveform wave in Waves.ToList())
			{
				// Initialize min and max sawtooth values so that rendering can determine
				// the true min and max
				wave.SawToothMinY = int.MaxValue;
				wave.SawToothMaxY = 0;

				// Reset the waveform counters
				wave.SawToothX = 0;
				wave.DecayFactor = 0;
				wave.Degrees = 0;				
				wave.TriangleCounter = 0;

				// Clear all the pixels out from the last rendering
				wave.Pixels.Clear();

				// Initialize wave maxtrix width and height based on the direction of the wave
				if (wave.Direction == DirectionType.RightToLeft ||
					 wave.Direction == DirectionType.LeftToRight)
				{
					wave.BufferHt = _bufferHt;
					wave.BufferWi = _bufferWi;
				}
				else
				{
					wave.BufferHt = _bufferWi;
					wave.BufferWi = _bufferHt;
				}

				// If the wave is an Ivey wave then...
				if (wave.WaveType == WaveType.FractalIvey)
				{
					// Initialize the Ivey random wave data
					InitializeFractalIveyData(wave);
				}
				
				// Initialize the snake window
				wave.WindowStart = 0;
				wave.WindowStop = (int)(wave.BufferWi * ((double)wave.WindowPercentage / 100.0));
							
				// If the waveform is being primed then...
				if (wave.PrimeWave)
				{
					// Prime the wave with enough frames to fill the matrix (in the X axis)
					for (int primeFrame = 0; primeFrame < wave.BufferWi; primeFrame++)
					{
						double intervalPos = GetEffectTimeIntervalPosition(primeFrame);
						double intervalPosFactor = intervalPos * 100;
																		
						// Render the waveform
						RenderWave(
							wave,
							primeFrame,
							wave.Mirror,
							CalculateWaveFrequency(wave, intervalPosFactor),
							CalculateWaveThickness(wave, intervalPosFactor),
							CalculateWaveHeight(wave, intervalPosFactor),
							CalculateWaveSpeed(wave, intervalPosFactor),
							CalculateWaveYOffset(wave, intervalPosFactor),
							GetColor(wave, intervalPos),							
							wave.WaveType,
							wave.PhaseShift,
							null,
							wave,
							wave.Direction);
					}
				}
			}
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

			// Give the collection of waves the mark collections
			Waves.MarkCollections = MarkCollections;
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
			// Make a copy of the waves in an attempt to minimize thread exceptions
			IEnumerable<IWaveform> waves = Waves.ToList();

			// Loop over the removed mark collections
			foreach (IMarkCollection markCollection in removedCollections)
			{
				// Unregister for property changed events from the mark collection
				markCollection.PropertyChanged -= MarkCollectionPropertyChanged;

				// If any of the waves had the removed mark collection selected then...
				if (waves.Any(waveform => waveform.MarkCollectionId == markCollection.Id))
				{
					// Mark the effect dirty
					MarkDirty();
				}
			}

			// Update the collection of mark collection names
			// This method also removes this mark collection as being selected on any wave.
			UpdateMarkCollectionNames();
		}

		/// <summary>
		/// Virtual method that is called by base class when the target positioning changes.
		/// </summary>
		protected override void TargetPositioningChanged()
		{
			// Update whether the render scale factor is visible 
			UpdateRenderScaleFactorAttributes(true);
		}

		#endregion

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/Wave/"; }
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Updates the visibility of fields.
		/// </summary>
		private void UpdateAttributes()
		{
			UpdateRenderScaleFactorAttributes(false);
			UpdateStringOrientationAttributes();
			TypeDescriptor.Refresh(this);
		}

		/// <summary>
		/// Calculates the speed of the wave.
		/// </summary>		
		private int CalculateWaveSpeed(IWaveform waveform, double intervalPos)
		{
			int speed = (int)Math.Round(ScaleCurveToValue(waveform.Speed.GetValue(intervalPos), MaxWaveVelocity * _speedIncrement, 0));

			if (speed == 0)
			{
				speed = 1;
			}

			return speed;
		}

		/// <summary>
		/// Calculates the thickness of the wave.
		/// </summary>		
		private int CalculateWaveThickness(IWaveform waveform, double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(waveform.Thickness.GetValue(intervalPos), MaxWaveThickness, 0));
		}

		/// <summary>
		/// Calculates the frequency of the wave.
		/// </summary>		
		private int CalculateWaveFrequency(IWaveform waveform, double intervalPos)
		{
			int frequency = (int)Math.Round(ScaleCurveToValue(waveform.Frequency.GetValue(intervalPos), MaxWaveFrequency, MinWaveFrequency));
			
			if (frequency == 0)
			{
				frequency = 1;
			}
			return frequency;
		}

		/// <summary>
		/// Calculates the height of the wave.
		/// </summary>		
		private int CalculateWaveHeight(IWaveform waveform, double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(waveform.Height.GetValue(intervalPos), MaxWaveHeight, 0));
		}

		/// <summary>
		/// Calculates the Y Offset of the wave.
		/// </summary>		
		private int CalculateWaveYOffset(IWaveform waveform, double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(waveform.YOffset.GetValue(intervalPos), MaxWaveYOffset, MinWaveYOffset));
		}

		/// <summary>
		/// Gets the color of the wave.
		/// </summary>		
		private Color GetColor(IWaveform waveform, double intervalPos)
		{
			return waveform.Color.GetColorAt(intervalPos);
		}

		/// <summary>
		/// Converts from the model wave data to the serialized wave data.
		/// </summary>
		private void UpdateWaveData()
		{
			// Clear the collection of wave forms
			_data.WaveformData.Clear();

			// Loop over the waves in the model wave collection
			foreach (IWaveform waveform in Waves.ToList())
			{
				// Create a new serialized wave form
				WaveformData serializedWaveform = new WaveformData();

				// Transfer the properties from the wave form model to the serialized wave form data
				serializedWaveform.Speed = new Curve(waveform.Speed);
				serializedWaveform.Thickness = new Curve(waveform.Thickness);
				serializedWaveform.WaveType = waveform.WaveType;
				serializedWaveform.UseMarks = waveform.UseMarks;
				serializedWaveform.Frequency = new Curve(waveform.Frequency);
				serializedWaveform.Height = new Curve(waveform.Height);
				serializedWaveform.Mirror = waveform.Mirror;
				serializedWaveform.YOffset = new Curve(waveform.YOffset);
				serializedWaveform.Direction = waveform.Direction;
				serializedWaveform.Color = new ColorGradient(waveform.Color);
				serializedWaveform.ColorHandling = waveform.ColorHandling;
				serializedWaveform.PhaseShift = waveform.PhaseShift;
				serializedWaveform.PrimeWave = waveform.PrimeWave;
				serializedWaveform.MovementType = waveform.MovementType;
				serializedWaveform.WindowPercentage = waveform.WindowPercentage;
				serializedWaveform.MarkCollectionId = waveform.MarkCollectionId;

				// Add the serialized waveform to the collection
				_data.WaveformData.Add(serializedWaveform);
			}
		}

		/// <summary>
		/// Converts from the serialized wave data to the model wave data.
		/// </summary>		
		private void UpdateWaveModel(WaveData waveData)
		{
			// Clear the view model waveform collection
			Waves.Clear();

			// Loop over the waves in the serialized effect data
			foreach (WaveformData serializedWaveform in waveData.WaveformData)
			{
				// Create a new waveform in the model
				IWaveform waveform = new Waveform();
													
				// Transfer the properties from the serialized effect data to the waveform model
				waveform.Speed = new Curve(serializedWaveform.Speed);
				waveform.Thickness = new Curve(serializedWaveform.Thickness);
				waveform.WaveType = serializedWaveform.WaveType;
				waveform.UseMarks = serializedWaveform.UseMarks;
				waveform.Frequency = new Curve(serializedWaveform.Frequency);
				waveform.Height = new Curve(serializedWaveform.Height);
				waveform.Mirror = serializedWaveform.Mirror;
				waveform.YOffset = serializedWaveform.YOffset;
				waveform.Direction = serializedWaveform.Direction;
				waveform.Color = new ColorGradient(serializedWaveform.Color);
				waveform.ColorHandling = serializedWaveform.ColorHandling;
				waveform.PrimeWave = serializedWaveform.PrimeWave;
				waveform.MovementType = serializedWaveform.MovementType;				
				waveform.PhaseShift = serializedWaveform.PhaseShift;				
				waveform.WindowPercentage = serializedWaveform.WindowPercentage;
				waveform.MarkCollectionId = serializedWaveform.MarkCollectionId;				
				waveform.MarkCollections = MarkCollections;

				// Add the waveform the effect's collection
				Waves.Add(waveform);
			}
		}

		/// <summary>
		/// Calculates the bottom of the wave based on a radius and wave thickness.
		/// </summary>		
		private int CalculateBottomOfWave(int ystart, double r, int thicknessWave)
		{
			return (int)(ystart - (r * (thicknessWave / 100.0)));
		}

		/// <summary>
		/// Calculates the top of the wave based on a radius and wave thickness.
		/// </summary>		
		private int CalculateTopOfWave(int ystart, double r, int thicknessWave)
		{
			return (int)(ystart + (r * (thicknessWave / 100.0)));
		}

		/// <summary>
		/// Calculates the top and bottom of the wave based on a radius and wave thickness.
		/// </summary>		
		private void CalculateTopAndBottomOfWave(int yStart, double r, int waveThickness, ref int y1, ref int y2)
		{
			// Calculate the bottom of the wave
			y1 = CalculateBottomOfWave(yStart, r, waveThickness);

			// Calculate the top of the wave
			y2 = CalculateTopOfWave(yStart, r, waveThickness);

			// Adjust the y2 if it is less than y1
			if (y2 <= y1)
			{
				y2 = y1 + 1; //minimum height
			}
		}

		/// <summary>
		/// Populates all the Y positions between y1 and y2.
		/// </summary>		
		private void PopulateYPositions(
			int y1, 
			int y2, 
			int roundedWaveYOffset, 
			List<Tuple<Color, int>> pixelsColumn, 
			IWaveform wave,
			Color fillColor)			
		{						
			// Loop over the Y positions that make up the wave
			for (int yNew = y1, y = 0; yNew < y2; yNew++, y++)
			{
				// If the color handling for this wave is Across the wave then...
				if (wave.ColorHandling == WaveColorHandling.Across)
				{
					// Sample the gradient based on the position of the Y pixel 
					fillColor = wave.Color.GetColorAt(((float)y) / (y2-y1));
				}

				// Add the adjusted Y position to the column
				int adjustedY = yNew + roundedWaveYOffset;

				// Add the pixel to the column collection
				pixelsColumn.Add(new Tuple<Color, int>(fillColor, adjustedY));
			}
		}

		/// <summary>
		/// Populates a column of Y coordinates for the specified wave.
		/// </summary>		
		private void PopulateYCoordinates(int y1, int y2, double yc, IWaveform wave, int yoffset, bool mirrorWave, Color fillColor)
		{
			// Create a new column of pixels
			List<Tuple<Color, int>> pixelsColumn = new List<Tuple<Color, int>>();

			// Add the column to the wave's collection of columns
			wave.Pixels.Enqueue(pixelsColumn);

			// Adjust the offset
			double waveYOffset = (wave.BufferHt / 2.0) * (yoffset * 0.01);

			// Round the offset to an integer
			int roundedWaveYOffset = (int)Math.Round(waveYOffset);

			// Populate the Y positions of the wave
			PopulateYPositions(y1, y2, roundedWaveYOffset, pixelsColumn, wave, fillColor);
			
			// If the waveform is being mirroed then...
			if (mirrorWave)
			{
				// Mirror the two Y coordinates around the center
				int y1mirror = (int)(yc + (yc - y1));
				int y2mirror = (int)(yc + (yc - y2));

				// If the Y coordinates are ascending order then...
				if (y1mirror < y2mirror)
				{
					y1 = y1mirror;
					y2 = y2mirror;
				}
				else
				{
					// Otherwise reverse the coordinates
					y2 = y1mirror;
					y1 = y2mirror;
				}

				// Populate the mirrored Y positions of the wave
				PopulateYPositions(y1, y2, roundedWaveYOffset, pixelsColumn, wave, fillColor);				
			}
		}

		/// <summary>
		/// This method handles growing and shrinking the waveform.
		/// </summary>		
		private void GrowAndShrink(
			bool mirrorWave,
			int numberWaves,
			int thicknessWave,
			int waveHeight,
			int wspeed,
			int yoffset,
			Color fillColor,			
			int angleOffset,
			IPixelFrameBuffer frameBuffer,
			IWaveform wave,
			DirectionType direction,
			Action<IWaveform, int, int, int, int, bool, int, Color> renderColumn)
		{
			// If the movement type is "Grow and Shrink" and the waveform has reached the 
			// edge of the display element then...
			if (wave.MovementType == WaveMovementType.GrowAndShrink &&
				 wave.Pixels.Count >= wave.BufferWi)
			{
				// Set the flag to start shrinking the waveform
				wave.Shrink = true;
			}

			// If waveform is NOT shrinking then...
			if (!wave.Shrink)
			{
				// Render colum(s) of the waveform based on the wave speed
				for (int s = 0; s < wspeed; s++)
				{
					// Render a column of the waveform
					renderColumn(
						wave,
						angleOffset,
						waveHeight,
						numberWaves,
						thicknessWave,
						mirrorWave,
						yoffset,
						fillColor);
				}
			}
			else // Shrinking the waveform
			{
				// Remove columns from the pixel queue based on the wave speeed
				for (int s = 0; s < wspeed; s++)
				{
					// If there is at least one Y coordinate in the queue then...
					if (wave.Pixels.Count > 0)
					{
						// Remove a Y coordinate from the queue
						// Note removing the last one added vs normal queue behavior
						QueueExtension.Pop(wave.Pixels);
					}
				}

				// If the pixel queue is empty then...
				if (wave.Pixels.Count == 0)
				{
					// Reset the shrink flag so that the waveform starts to grow
					wave.Shrink = false;
				}
			}

			// Render the wave onto the frame buffer
			RenderColumns(
				wave,
				frameBuffer,
				direction,
				wspeed,
				true);
		}

		/// <summary>
		/// Renders the columns of the wave on the display element.
		/// </summary>		
		private void RenderColumns(
			IWaveform wave,			
			IPixelFrameBuffer frameBuffer,
			DirectionType direction,
			int wspeed,
			bool updateWindow)			
		{
			// If the queue has more Y coordinates than the display element then...
			while (wave.Pixels.Count > wave.BufferWi)
			{
				// Remove Y coordinates from the queue
				wave.Pixels.Dequeue();
			}

			// Only want to update the snake window if this is a 50ms frame
			// where we are rendering new wave data
			if (updateWindow)
			{
				// Move the snake window
				wave.WindowStart += wspeed;
				wave.WindowStop += wspeed;
			
				// If the snake window has moved off the display element then...
				if (wave.WindowStart >= wave.BufferWi)
				{
					// Reset the window back to the start of the display element
					wave.WindowStart = 0;
					wave.WindowStop = (int)(wave.BufferWi * (wave.WindowPercentage / 100.0));
				}
			}
			
			// Get the columns of the display element
			List<List<Tuple<Color, int>>> columns = wave.Pixels.ToList();
			
			// Loop over the columns in the display element
			for (int index = 0; index < columns.Count; index++)
			{
				// Get the Y coordinates associated with the current column
				List<Tuple<Color, int>> column = columns[index];
				
				// Loop over the Y coordinates
				for(int y = 0; y < column.Count; y++)				
				{
					// Get the Y position of the pixel
					int yPos = column[y].Item2;
										
					if (direction == DirectionType.LeftToRight ||
						 direction == DirectionType.BottomToTop)
					{
						if (direction == DirectionType.LeftToRight)							 
						{
							RenderPixelToFrameBuffer(wave, index, index, yPos, column[y].Item1, frameBuffer);
						}
						else
						{
							RenderPixelToFrameBuffer(wave, index, yPos, index, column[y].Item1, frameBuffer);
						}
					}
					else
					{
						if (direction == DirectionType.RightToLeft)
						{
							RenderPixelToFrameBuffer(wave, index, wave.BufferWi - index - 1, yPos, column[y].Item1, frameBuffer);
						}
						else
						{
							RenderPixelToFrameBuffer(wave, index, yPos, wave.BufferWi - index - 1, column[y].Item1, frameBuffer);
						}
					}
				}
			}
		}

		/// <summary>
		/// Renders the specified pixel to the frame buffer.
		/// </summary>		
		private void RenderPixelToFrameBuffer(IWaveform wave, int rawIndex, int x, int y, Color fillColor, IPixelFrameBuffer frameBuffer)
		{
			// Frame buffer is null when priming waves in the SetupRender method
			if (frameBuffer != null)
			{
				// If in sname mode then...
				if (wave.MovementType == WaveMovementType.Snake)
				{
					// If the specified pixel is within the window then...
					if (wave.WindowStart <= rawIndex && rawIndex <= wave.WindowStop)
					{
						// Render the pixel
						frameBuffer.SetPixel(x, y, fillColor);
					}
				}
				else
				{
					// Render the pixel
					frameBuffer.SetPixel(x, y, fillColor);
				}
			}
		}

		/// <summary>
		/// Calculates the Sine of the specified angle in degrees.
		/// </summary>		
		private double Sine(double degree)
		{
			double radian = degree * pi_180;
			return Math.Sin(radian);
		}

		/// <summary>
		/// Renders the wave effect.
		/// </summary>		
		private void RenderWave(
			IWaveform wave,
			int frame,
			bool mirrorWave,
			int numberOfWaves,
			int thicknessWave,
			int waveHeight,
			int wspeed,
			int yoffset,
			Color fillColor,			
			WaveType waveType,
			int angleOffset,
			IPixelFrameBuffer frameBuffer,
			IWaveform waveform,
			DirectionType direction)
		{
			switch(waveType)
			{
				case WaveType.Sine:
					RenderSineWave(
						mirrorWave,
						numberOfWaves,
						thicknessWave,
						waveHeight,
						wspeed,
						yoffset,
						fillColor,						
						angleOffset,
						frameBuffer,
						wave,
						direction);
					break;
				case WaveType.Square:
					RenderSquareWave(
						waveform,
						numberOfWaves,
						yoffset,
						wspeed * 2,
						waveHeight,
						angleOffset,
						thicknessWave,
						mirrorWave,
						frameBuffer,
						fillColor,						
						direction);
					break;
				case WaveType.Triangle:
					RenderWaveTriangleWave(
						mirrorWave,
						numberOfWaves,
						thicknessWave,
						waveHeight,
						wspeed,
						yoffset,
						fillColor,						
						angleOffset,
						frameBuffer,
						wave,
						direction);
					break;
				case WaveType.FractalIvey:
					RenderIveyWave(
						wave,
						numberOfWaves,
						thicknessWave,
						mirrorWave,
						fillColor,						
						frameBuffer,
						frame,
						wspeed * 2,
						direction);
					break;
				case WaveType.DecayingSine:
					RenderDecayingSineWave(
						wave,
						frame,
						mirrorWave,
						numberOfWaves,
						thicknessWave,
						waveHeight,
						wspeed,
						yoffset,
						fillColor,						
						angleOffset,
						frameBuffer,
						direction);
					break;
				case WaveType.Sawtooth:
					RenderSawTooth(
						wave,
						frame,
						mirrorWave,
						numberOfWaves,
						thicknessWave,
						waveHeight,
						wspeed,
						yoffset,
						fillColor,						
						angleOffset,
						frameBuffer,
						direction);
					break;
				default:
					Debug.Assert(false, "Unsupported Wave Type!");
					break;
			}			
		}
		
		/// <summary>
		/// Updates the mark collection on the waveforms.
		/// </summary>
		private void UpdateMarkCollectionNames()
		{						
			// Check to see if selected mark collection names need to be updated
			Waves.UpdateSelectedMarkCollectionNames();			
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
		/// Draws vertical lines for the square or sawtooth waveforms.
		/// </summary>		
		private void RenderVerticalLines(IWaveform wave, int y1, int y2, int yOffset, bool mirrorWave, int lines, Color fillColor)
		{
			// Loop over the previous wave thickness minus the line that was already added above
			for (int x = 0; x < lines; x++)
			{
				// Populate more vertical lines
				PopulateYCoordinates(y1, y2, wave.YC, wave, yOffset, mirrorWave, fillColor);
			}
		}

		#endregion

		#region Private Fractal Ivey Wave Methods

		/// <summary>
		/// Renders the fractal ivey waveform.
		/// </summary>
		private void RenderIveyWave(
			IWaveform wave,
			int numberWaves,
			int waveThickness,
			bool mirrorWave,
			Color fillColor,			
			IPixelFrameBuffer frameBuffer,
			int frame,
			int wspeed,
			DirectionType direction)
		{
			// Initialize the state (this variable causes movement)
			int state = (int)((frame) * wspeed);

			// Loop over all the columns in the display element
			for (int x = 0; x < wave.BufferWi; x++)
			{
				int yStart = 0;

				LoopControl loopControl = RenderFractalIveyWave(
					wave,
					state,
					numberWaves,
					x,
					ref yStart);

				if (loopControl == LoopControl.BreakLoop)
				{
					break;
				}
				else if (loopControl == LoopControl.ContinueLoop)
				{
					continue;
				}

				// Calculate the top and bottom of the wave							
				int y1 = 0;
				int y2 = 0;
				double r = wave.YC;
				CalculateTopAndBottomOfWave(yStart, r, waveThickness, ref y1, ref y2);
				
				// Sets the Y coordinates of the wave onto the wave
				PopulateYCoordinates(y1, y2, wave.YC, wave, 0, mirrorWave, fillColor);
			}

			// Render the wave onto the frame buffer
			RenderColumns(
				wave,
				frameBuffer,
				direction,
				wspeed,
				true);

			// Clear out the pixels for the next frame
			wave.Pixels.Clear();
		}

		/// <summary>
		/// Renders the factal ivey waveform.
		/// </summary>		
		private LoopControl RenderFractalIveyWave(
			IWaveform wave,
			int state,
			int numberOfWaves,
			int x,
			ref int ystart)
		{
			int eff_x = x + wave.BufferWi * (state / 2 / wave.BufferWi); //effective x before wrap

			if (eff_x >= numberOfWaves * wave.BufferWi)
			{
				return LoopControl.BreakLoop;
			}

			bool ok = (eff_x <= state / 2);  //ivy "grows"
			if (!ok)
			{
				return LoopControl.ContinueLoop;
			}
			ystart = wave.IveyYValues[eff_x] / 2;

			return LoopControl.Normal;
		}

		/// <summary>
		/// Initialize the Ivey wave random data.
		/// </summary>		
		private void InitializeFractalIveyData(IWaveform wave)
		{
			int numberOfWaves = CalculateWaveFrequency(wave, 0);
			//double r = 0;
			int delay = 0;
			int delta = 0; //next branch length, angle

			for (int index = 0; index < numberOfWaves * wave.BufferWi; index++)
			{
				wave.IveyYValues.Add(0);
			}

			for (int x1 = 0; x1 < numberOfWaves * wave.BufferWi; ++x1)
			{
				wave.IveyYValues[x1] = (int)((delay-- > 0) ? wave.IveyYValues[x1 - 1] + delta : 2 * wave.YC);
				if (wave.IveyYValues[x1] >= 2 * wave.BufferHt) { delta = -2; wave.IveyYValues[x1] = 2 * wave.BufferHt - 1; if (delay > 1) delay = 1; }
				if (wave.IveyYValues[x1] < 0) { delta = 2; wave.IveyYValues[x1] = 0; if (delay > 1) delay = 1; }
				if (delay < 1)
				{
					delta = (_rand.Next() % 7) - 3;
					delay = 2 + (_rand.Next() % 3);
				}
			}
		}

		#endregion

		#region Private Square Wave Methods

		/// <summary>
		/// Renders the square wave waveform.
		/// </summary>		
		private void RenderSquareWave(
			IWaveform wave,
			int numberOfWaves,
			int yOffset,
			int wspeed,
			int waveHeight,
			int angleOffset,
			int waveThickness,
			bool mirrorWave,
			IPixelFrameBuffer frameBuffer,
			Color fillColor,			
			DirectionType direction)
		{
			// Grow and shrink the square wave
			GrowAndShrink(mirrorWave, numberOfWaves, waveThickness, waveHeight, wspeed, yOffset, fillColor, angleOffset, frameBuffer, wave, direction, RenderSquareColumn);
		}

		/// <summary>
		/// Renders a column of the square wave.
		/// </summary>								
		private void RenderSquareColumn(
			IWaveform wave,
			int angleOffset,
			int waveHeight,
			int numberOfWaves,
			int thicknessWave,
			bool mirrorWave,
			int yoffset,
			Color fillColor)
		{
			// Calculate the sin for the previous x
			double sinradMinus1 = Sine(wave.Degrees + angleOffset);

			// Increment the degrees for the next x
			wave.Degrees += wave.GetDegreesPerX(numberOfWaves);

			// Calculate the sin for the current x
			double sinrad = Sine(wave.Degrees + angleOffset);

			// Calculate the top and bottom of the wave
			int y1 = 0;
			int y2 = 0;
			bool verticalLine = RenderSquareWave(wave, thicknessWave, waveHeight, sinrad, sinradMinus1, ref y1, ref y2, wave.YC, wave.Pixels.Count);

			// Sets the Y coordinates of the wave onto the wave pixels property
			PopulateYCoordinates(y1, y2, wave.YC, wave, yoffset, mirrorWave, fillColor);

			// If the wave is at the vertical line then...
			if (verticalLine)
			{
				// Add additional vertical lines as necessary
				RenderVerticalLines(wave, y1, y2, yoffset, mirrorWave, wave.LastWaveThickness - 1, fillColor);
			}
		}

		/// <summary>
		/// Calculates the Y coordinates of the square wave.
		/// </summary>		
		private bool RenderSquareWave(
			IWaveform wave, 
			int thicknessWave, 
			int waveHeight, 
			double sinrad, 
			double sinradMinus1, 
			ref int y1, 
			ref int y2, 
			double yc, 
			int x)
		{
			bool verticalLine = false;

			if (Math.Sign(sinrad) != Math.Sign(sinradMinus1) && x != 1)
			{
				y1 = (int)(yc - yc * (waveHeight / 100.0));
				y2 = (int)Math.Ceiling((yc + yc * (waveHeight / 100.0)));
				verticalLine = true;
			}
			else if (sinrad > 0.0)
			{
				y1 = (int)(yc + 1 + yc * (waveHeight / 100.0) * ((100.0 - thicknessWave) / 100.0));
				y2 = (int)Math.Ceiling((yc + yc * (waveHeight / 100.0)));				
			}
			else
			{
				y1 = (int)(yc - yc * (waveHeight / 100.0));
				y2 = (int)(yc - yc * (waveHeight / 100.0) * ((100.0 - thicknessWave) / 100.0));				
			}

			// Limit y1 and y2
			if (y1 < 0)
			{
				y1 = 0;
			}

			if (y2 < 1)
			{
				y2 = 1;
			}

			if (y1 > wave.BufferHt - 1)
			{
				y1 = wave.BufferHt - 1;
			}
			if (y2 > wave.BufferHt)
			{
				y2 = wave.BufferHt;
			}

			if (y2 <= y1)
			{
				y2 = y1 + 1;
			}

			if (!verticalLine)
			{
				wave.LastWaveThickness = y2 - y1;
			}

			return verticalLine;
		}
		
		#endregion

		#region Private Triangle Wave Methods

		/// <summary>
		/// Renders the triangle waveform.
		/// </summary>		
		private void RenderWaveTriangleWave(
			bool mirrorWave,
			int numberOfWaves,
			int waveThickness,
			int waveHeight,
			int wspeed,
			int yOffset,
			Color fillColor,			
			int angleOffset,
			IPixelFrameBuffer frameBuffer,
			IWaveform wave,
			DirectionType direction)
		{
			// Grow and shrink the triangle wave
			GrowAndShrink(mirrorWave, numberOfWaves, waveThickness, waveHeight, wspeed, yOffset, fillColor, angleOffset, frameBuffer, wave, direction, RenderColumnTriangle);
		}

		/// <summary>
		/// Renders a column of the triangle waveform.
		/// </summary>		
		private void RenderColumnTriangle(
			IWaveform wave,
			int angleOffset,
			int waveHeight,
			int numberOfWaves,
			int thicknessWave,
			bool mirrorWave,
			int yoffset,
			Color fillColor)
		{
			// Calculate the Y starting point of the wave
			int ystart = CalculateYStartTriangleWave(
				wave,
				numberOfWaves,
				waveHeight,
				wave.TriangleCounter + angleOffset);

			// Increment the triangle x counter
			wave.TriangleCounter++;

			// Calculate the top and bottom of the wave			
			int y1 = 0;
			int y2 = 0;
			double r = wave.YC;
			CalculateTopAndBottomOfWave(ystart, r, thicknessWave, ref y1, ref y2);

			// Sets the Y coordinates of the wave onto the wave
			PopulateYCoordinates(y1, y2, wave.YC, wave, yoffset, mirrorWave, fillColor);
		}

		/// <summary>
		/// Calculates the starting Y coordinate of the wave.
		/// </summary>		
		private int CalculateYStartTriangleWave(
			IWaveform wave, 
			int numberOfWaves, 
			int waveHeight, 
			int x)						
		{			
			// Calculate number of waves
			double waves = ((double)numberOfWaves / 180.0) / 5; 

			// Calculate the wave amplitude
			int amp = wave.BufferHt * waveHeight / 100;

			int yStart = 0;

			if (amp == 0)
			{
				yStart = 0;
			}
			else
			{				
				yStart = (wave.BufferHt - amp) / 2 + Math.Abs((int)(x * waves) % (int)(2 * amp) - amp);
			}

			// Check to see if the top of the wave has exceeded the height of the dispay element
			if (yStart > wave.BufferHt - 1)
			{
				yStart = wave.BufferHt - 1;
			}

			return yStart;
		}
				
		#endregion

		#region Private Sawtooth Wave Methods

		/// <summary>
		/// Renders the SawTooth waveform.
		/// </summary>		
		private void RenderSawTooth(
			IWaveform wave,
			int frame,
			bool mirrorWave,
			int numberOfWaves,
			int waveThickness,
			int waveHeight,
			int wspeed,
			int yOffset,
			Color fillColor,			
			int angleOffset,
			IPixelFrameBuffer frameBuffer,
			DirectionType direction)
		{
			// Grow and shrink the sawtooth waveform
			GrowAndShrink(mirrorWave, numberOfWaves, waveThickness, waveHeight, wspeed, yOffset, fillColor, angleOffset, frameBuffer, wave, direction, RenderSawToothColumn);
		}

		/// <summary>
		/// Renders a column of the Sawtooth waveform.
		/// </summary>		
		private void RenderSawToothColumn(
			IWaveform wave,
			int angleOffset,
			int waveHeight,
			int numberOfWaves,
			int thicknessWave,
			bool mirrorWave,
			int yoffset,
			Color fillColor)
		{
			// Calculate the slope of the sawtooth
			int toothGap = 0;
			double m = CalculateSlopeOfSawTooth(wave, numberOfWaves, waveHeight, ref toothGap);

			// Calculate the sine of the previous X position
			double sinradMinus1 = Sine(wave.Degrees + angleOffset);

			// Increment the X position
			double degree_per_x = numberOfWaves / wave.BufferWi;
			wave.Degrees += degree_per_x;

			// Calculate the sine of the next X position 
			double sinrad = Sine(wave.Degrees + angleOffset);

			// Calculate the bottom of the sawtooth
			int y1 = CalculateBottomOfWave((int)wave.YC, wave.YC, waveHeight);
			
			// Calculate the Y Position on the sawtooth slant
			int ystart = CalculateYPositionOnSawtooth(
				wave,
				sinrad,
				sinradMinus1,				
				m,
				toothGap,
				(int)y1);

			// Check to see if the vertical lines need to be drawn
			bool verticalLine = IsVerticalLine(sinrad, sinradMinus1);

			int y2 = 0;
			if (verticalLine)
			{
				//RenderSawtoothVerticalLine(wave, waveHeight, ref y1, ref y2, wave.YC);
				y1 = wave.SawToothMinY;
				y2 = wave.SawToothMaxY;
			}
			else
			{
				// Calculate the Y axis top and bottom of the wave
				double r = wave.YC;				
				CalculateTopAndBottomOfWave(ystart, r, thicknessWave, ref y1, ref y2);
				wave.LastWaveThickness = y2 - y1;
			}

			// If the Y value is greater than the current max Y 
			if (wave.SawToothMaxY < y2)
			{
				// Update the max Y, this property is used to draw the vertial line
				wave.SawToothMaxY = y2;
			}

			// If the Y value is less than the current min Y
			if (wave.SawToothMinY > y1)
			{
				// Update the min Y, this property is used to draw the vertial line
				wave.SawToothMinY = y1;
			}

			// Populate the Y coordinates for the wave
			PopulateYCoordinates(y1, y2, wave.YC, wave, yoffset, mirrorWave, fillColor);

			// If the wave is at the vertical line then...
			if (verticalLine)
			{
				// Add additional vertical lines as necessary
				RenderVerticalLines(wave, y1, y2, yoffset, mirrorWave, (int)(wave.LastWaveThickness - (wave.LastSawToothY - wave.LastLastSawToothY)), fillColor);				
			}

			// Store off the last two Y positions of the sawtooth
			wave.LastLastSawToothY = wave.LastSawToothY;
			wave.LastSawToothY = ystart;
		}		

		/// <summary>
		/// Returns the Y position on the sawtooth slanted line.
		/// </summary>		
		private int CalculateYPositionOnSawtooth(
			IWaveform wave,			
			double sinrad,
			double sinradMinus1,						
			double m,
			int horizontalGapBetweenTeeth,
			int yc)
		{
			// Determine if the sine is positive
			bool sinradPositive = sinrad >= 0;

			// Determine if the previous sine is negative
			bool sinradMinus1Negative = sinradMinus1 < 0;

			// If the sine has transitioned from negative to positive or
			// X position is greater than the expected tooth gap space then...
			if (sinradPositive && sinradMinus1Negative ||
				 wave.SawToothX >= horizontalGapBetweenTeeth)
			{
				// Reset the sawtooth X position
				wave.SawToothX = 1;
			}
			else
			{
				// Otherwise increment the sawtooth X position
				wave.SawToothX++;
			}

			// Calculate where we are on the sawtooth slanted line
			int ystart = (int)(yc + m * (wave.SawToothX));
			
			return ystart;
		}

		/// <summary>
		/// Calculates the horizontal gap between teeth.
		/// </summary>				
		private int FindToothGap(double degreePerX)
		{
			return (int)Math.Ceiling(360.0 / degreePerX);			
		}

		/// <summary>
		/// Calculates the slope of the sawtooth slant line.
		/// </summary>		
		private double CalculateSlopeOfSawTooth(IWaveform wave, int numberOfWaves, int waveHeight, ref int toothGap)
		{
			// Determine the degrees per X column
			double degree_per_x = numberOfWaves / wave.BufferWi;
			
			// Calculate the horizontal gap between teeth
			toothGap = FindToothGap(degree_per_x);

			// Calculate the start and end y positions of the sawtooth slant line
			int y1 = (int)(wave.YC - wave.YC * (waveHeight / 100.0));
			int y2 = (int)(wave.YC + wave.YC * (waveHeight / 100.0));

			// Calculate the rise of the sawtooth
			double rise = (y2 - y1);

			// Calculate the slope
			double m = rise / ((double)toothGap);

			// Return the slope of the line
			return m;
		}

		/// <summary>
		/// Returns true if the sine is changing sign (+/-).
		/// </summary>		
		private bool IsVerticalLine(double sinrad, double sinradMinus1)
		{
			bool sinradPositive = sinrad >= 0;
			bool sinradMinus1Negative = sinradMinus1 < 0;

			// If the sine has changed sign (+/-) then...
			return (sinradPositive && sinradMinus1Negative);
		}

		#endregion

		#region Private Sine Wave Methods			  					  		

		/// <summary>
		/// Renders a column of the sine waveform.
		/// </summary>		
		private void RenderColumnSine(
			IWaveform wave,
			int angleOffset,
			int waveHeight,
			int numberOfWaves,
			int waveThickness,
			bool mirrorWave,
			int yOffset,
			Color fillColor)
		{						
			// Calculate the sine of the specified degrees
			double sinrad = Sine(wave.Degrees + angleOffset);

			// Increment the position of the wave for the next frame
			wave.Degrees += wave.GetDegreesPerX(numberOfWaves);

			// Render the Y coordinates of the sine wave
			RenderColumnSine(sinrad, waveHeight, waveThickness, yOffset, mirrorWave, wave, wave.YC, fillColor);			
		}

		/// <summary>
		/// Renders the Y coordinates of the specified sine value.
		/// </summary>		
		private void RenderColumnSine(double sinrad, int waveHeight, int waveThickness, int yOffset, bool mirrorWave, IWaveform wave, double r, Color fillColor)
		{			
			// Determine the center Y position of this column
			int ystart = (int)(r * (waveHeight / 100.0) * sinrad + wave.YC);

			// Determine the starting and ending y coordinate for this column	
			int y1 = 0;
			int y2 = 0;
			CalculateTopAndBottomOfWave(ystart, r, waveThickness, ref y1, ref y2);

			// Populate the Y coordinates for the wave
			PopulateYCoordinates(y1, y2, wave.YC, wave, yOffset, mirrorWave, fillColor);
		}

		/// <summary>
		/// Renders the sine waveform.
		/// </summary>		
		private void RenderSineWave(
			bool mirrorWave,
			int numberOfWaves,
			int waveThickness,
			int waveHeight,
			int wspeed,
			int yOffset,
			Color fillColor,			
			int angleOffset,
			IPixelFrameBuffer frameBuffer,
			IWaveform wave,
			DirectionType direction)
		{
			// Grow and shrink the sine wave
			GrowAndShrink(mirrorWave, numberOfWaves, waveThickness, waveHeight, wspeed, yOffset, fillColor, angleOffset, frameBuffer, wave, direction, RenderColumnSine);			
		}

		#endregion

		#region Private Decaying Sine Wave Methods

		/// <summary>
		/// Renders the Decaying Sine waveform.
		/// </summary>		
		private void RenderDecayingSineWave(
			IWaveform wave,
			int frame,
			bool mirrorWave,
			int numberOfWaves,
			int waveThickness,
			int waveHeight,
			int wspeed,
			int yOffset,
			Color fillColor,			
			int angleOffset,
			IPixelFrameBuffer frameBuffer,
			DirectionType direction)
		{
			// Initialize the state (this variable causes movement)
			int state = (int)(frame * wspeed);

			// Initialize the amplitude of the wave
			double r = wave.YC;

			// Decrease the amplitude of the wave			
			r -= wave.DecayFactor;

			// Increase the decay factor with each frame so that the wave amplitude gets smaller
			wave.DecayFactor += _speedIncrement;
			
			// If the amplitude is at zero or below then...
			if (r <= 0)
			{
				// If a mark is active then...
				if (IsMarkActive(wave, frame))
				{
					// Reset the decay factor such that the wave returns to full amplitude
					wave.DecayFactor = 0;
				}
				else
				{
					// Keep the amplitude at zero
					r = 0;
				}
			}
			else
			{
				// If a mark is active then...
				if (!string.IsNullOrEmpty(wave.MarkCollectionName) && IsMarkActive(wave, frame))
				{
					// Reset the decay factor such that the wave returns to full amplitude
					wave.DecayFactor = 0;
				}
			}

			// Loop over all the columns in the display element
			for (int x = 0; x < wave.BufferWi; x++)
			{
				// state causes it to move												
				double sinrad = Sine(x * wave.GetDegreesPerX(numberOfWaves) + state + angleOffset);

				// Render the Y coordinates of the sine wave
				RenderColumnSine(sinrad, waveHeight, waveThickness, yOffset, mirrorWave, wave, r, fillColor);
			}

			// Render the wave onto the frame buffer
			RenderColumns(
				wave,				
				frameBuffer,
				direction,
				wspeed,
				true);

			// Clear out the pixels for the next frame
			wave.Pixels.Clear();
		}

		/// <summary>
		/// Returns true if a mark covers the current frame.
		/// </summary>		
		private bool IsMarkActive(IWaveform waveform, int frameNum)
		{
			// Default to the mark being active
			bool markActive = true;

			// If a mark collection has been selected then...
			if (!string.IsNullOrEmpty(waveform.MarkCollectionName))
			{
				// If a mark collection has been specified then default to NOT active
				markActive = false;
				
				// Get the selected mark collection
				IMarkCollection marks = MarkCollections.FirstOrDefault(item => item.Id == waveform.MarkCollectionId);

				// Determine how far into the effect we are
				int millisecondsIntoEffect = FrameTime * frameNum;

				// Create a time span for the specified number of milliseconds
				TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, millisecondsIntoEffect);

				// Gets the marks inclusive of the effect
				List<IMark> marksLeftInEffect = marks.MarksInclusiveOfTime(StartTime + timeSpan, StartTime + TimeSpan);

				// Default to there not being a mark active
				markActive = false;

				// Loop over the remaining marks				
				if (marksLeftInEffect.Count > 0)
				{
					// Get the first mark that overlaps the effect (or whats left of the effect)
					IMark mark = marksLeftInEffect[0];

					// If the mark includes the current frame then...
					if (mark.StartTime <= StartTime + timeSpan &&
					    StartTime + timeSpan <= mark.EndTime)
					{
						// Indicate a mark was found
						markActive = true;						
					}
				}
			}

			return markActive;
		}

		/// <summary>
		/// Updates whether the render scale factor is visible.
		/// </summary>		
		private void UpdateRenderScaleFactorAttributes(bool refresh)
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

		#endregion
	}
}
