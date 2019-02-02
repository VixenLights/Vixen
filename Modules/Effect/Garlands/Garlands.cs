using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using VixenModules.Effect.Effect.Location;
using System.Diagnostics;

namespace VixenModules.Effect.Garlands
{
	public class Garlands : PixelEffectBase
	{
		private GarlandsData _data;
		private int _speed;
		private int _frames;

		public Garlands()
		{
			EnableTargetPositioning(true, true);
			_data = new GarlandsData();
			InitAllAttributes();
		}

		public override bool IsDirty
		{
			get
			{
				if (Colors.Any(x => !x.CheckLibraryReference()))
				{
					base.IsDirty = true;
				}

				return base.IsDirty;
			}
			protected set { base.IsDirty = value; }
		}

		#region Setup

		[Value]
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

		#endregion

		#region Config properties

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"MovementType")]
		[ProviderDescription(@"MovementType")]
		[PropertyOrder(0)]
		public MovementType MovementType
		{
			get { return _data.MovementType; }
			set
			{
				_data.MovementType = value;
				IsDirty = true;
				UpdateMovementTypeAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Direction")]
		[ProviderDescription(@"Direction")]
		[PropertyOrder(1)]
		public GarlandsDirection Direction
		{
			get { return _data.Direction; }
			set
			{
				_data.Direction = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Speed")]
		//[NumberRange(1, 100, 1)]
		[PropertyOrder(1)]
		public Curve SpeedCurve
		{
			get { return _data.SpeedCurve; }
			set
			{
				_data.SpeedCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Iterations")]
		[ProviderDescription(@"Iterations")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 20, 1)]
		[PropertyOrder(2)]
		public int Iterations
		{
			get { return _data.Iterations; }
			set
			{
				_data.Iterations = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"GarlandType")]
		[ProviderDescription(@"GarlandType")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 4, 1)]
		[PropertyOrder(4)]
		public int Type
		{
			get { return _data.Type; }
			set
			{
				_data.Type = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Spacing")]
		[ProviderDescription(@"Spacing")]
		//[NumberRange(1, 20, 1)]
		[PropertyOrder(5)]
		public Curve SpacingCurve
		{
			get { return _data.SpacingCurve; }
			set
			{
				_data.SpacingCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Color properties


		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"ColorGradients")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(1)]
		public List<ColorGradient> Colors
		{
			get { return _data.Colors; }
			set
			{
				_data.Colors = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Level properties

		[Value]
		[ProviderCategory(@"Brightness", 3)]
		[ProviderDisplayName(@"Brightness")]
		[ProviderDescription(@"Brightness")]
		public Curve LevelCurve
		{
			get { return _data.LevelCurve; }
			set
			{
				_data.LevelCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/garlands/"; }
		}

		#endregion

		private void UpdateAttributes()
		{
			UpdateMovementTypeAttribute(false);
			TypeDescriptor.Refresh(this);
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as GarlandsData;
				UpdateAttributes();
				IsDirty = true;
			}
		}

		private void InitAllAttributes()
		{
			UpdateStringOrientationAttributes(true);
			UpdateMovementTypeAttribute(false);
			TypeDescriptor.Refresh(this);
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		private void UpdateMovementTypeAttribute(bool refresh = true)
		{
			bool movementType = MovementType == MovementType.Speed;
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1);
			propertyStates.Add("SpeedCurve", movementType);
			propertyStates.Add("Iterations", !movementType);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}
		protected override void SetupRender()
		{
			_frames = 0;
			_speed = 0;
		}

		protected override void CleanUpRender()
		{
			//Nothing to clean up
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			var intervalPos = GetEffectTimeIntervalPosition(frame);
			var intervalPosFactor = intervalPos * 100;
			double level = LevelCurve.GetValue(intervalPosFactor) / 100;
			int width, height;

			int pixelSpacing = CalculateSpacing(intervalPosFactor) * BufferHt / 100 + 3;
			if (Direction == GarlandsDirection.Up || Direction == GarlandsDirection.Down)
			{
				width = BufferWi;
				height = BufferHt;
			}
			else
			{
				width = BufferHt;
				height = BufferWi;
			}
			int limit = height * pixelSpacing * 4;
			int garlandsState;
			if (MovementType == MovementType.Iterations)
			{
				garlandsState = (int) (limit - _frames*(limit/ GetNumberFrames() * Iterations))/4; //Iterations
				if (garlandsState <= 0)
					_frames = 0;
			}
			else
			{
				_speed += CalculateSpeed(intervalPosFactor);
				garlandsState = (limit - _speed % limit) / 4; //Speed
			}
			_frames++;

			for (int ring = 0; ring < height; ring++)
			{
				var ratio = ring / (double)height;
				Color col = GetMultiColorBlend(ratio, false, frame);
				if (level < 1)
				{
					HSV hsv = HSV.FromRGB(col);
					hsv.V = hsv.V * level;
					col = hsv.ToRGB();
				}
				
				var y = garlandsState - ring * pixelSpacing;
				var ylimit = height - ring - 1;
				for (int x = 0; x < width; x++)
				{
					var yadj = y;
					switch (Type)
					{
						case 1:
							switch (x % 5)
							{
								case 2:
									yadj -= 2;
									break;
								case 1:
								case 3:
									yadj -= 1;
									break;
							}
							break;
						case 2:
							switch (x % 5)
							{
								case 2:
									yadj -= 4;
									break;
								case 1:
								case 3:
									yadj -= 2;
									break;
							}
							break;
						case 3:
							switch (x % 6)
							{
								case 3:
									yadj -= 6;
									break;
								case 2:
								case 4:
									yadj -= 4;
									break;
								case 1:
								case 5:
									yadj -= 2;
									break;
							}
							break;
						case 4:
							switch (x % 5)
							{
								case 1:
								case 3:
									yadj -= 2;
									break;
							}
							break;
					}
					switch (Direction)
					{
						case GarlandsDirection.Down:
							if (yadj < ylimit) yadj = ylimit;
							if (yadj < BufferHt) frameBuffer.SetPixel(x, BufferHt - 1 - yadj, col);
						break;
						case GarlandsDirection.Up:
							if (yadj < ylimit) yadj = ylimit;
							if (yadj < BufferHt) frameBuffer.SetPixel(x, yadj, col);
						break;
						case GarlandsDirection.Left:
							if (yadj < ylimit) yadj = ylimit;
							if (yadj < BufferWi) frameBuffer.SetPixel(BufferWi - 1 - yadj, x, col);
						break;
						case GarlandsDirection.Right:
							if (yadj < ylimit) yadj = ylimit;
							if (yadj < BufferWi) frameBuffer.SetPixel(yadj, x, col);
						break;
					}
				}
			}
		}

		private int CalculateSpeed(double intervalPos)
		{
			var value = (int)Math.Round(ScaleCurveToValue(SpeedCurve.GetValue(intervalPos), 100, 1));
			if (value < 1) value = 1;

			return value;
		}

		private int CalculateSpacing(double intervalPos)
		{
			var value = (int)Math.Round(ScaleCurveToValue(SpacingCurve.GetValue(intervalPos), 20, 1));
			if (value < 1) value = 1;

			return value;
		}

		// return a value between c1 and c2
		private int ChannelBlend(int c1, int c2, double ratio)
		{
			return c1 + (int)Math.Floor(ratio * (double)(c2 - c1) + 0.5);
		}

		public Color Get2ColorBlend(int coloridx1, int coloridx2, double ratio, int frame)
		{
			var pos = GetEffectTimeIntervalPosition(frame) ;
			var c1 = Colors[coloridx1].GetColorAt(pos);
			var c2 = Colors[coloridx2].GetColorAt(pos);

			return Color.FromArgb(ChannelBlend(c1.R, c2.R, ratio), ChannelBlend(c1.G, c2.G, ratio), ChannelBlend(c1.B, c2.B, ratio));
		}

		public Color GetMultiColorBlend(double n, bool circular, int frame)
		{
			int colorcnt = Colors.Count;
			if (colorcnt <= 1) return Colors[0].GetColorAt(GetEffectTimeIntervalPosition(frame));
			if (n >= 1.0) n = 0.99999;
			else if (n < 0.0) n = 0.0;
			double realidx = circular ? n * colorcnt : n * (colorcnt - 1);
			int coloridx1 = (int)Math.Floor(realidx);
			int coloridx2 = (coloridx1 + 1) % colorcnt;
			double ratio = realidx - coloridx1;
			return Get2ColorBlend(coloridx1, coloridx2, ratio, frame);
		}

		/// <summary>
		/// Renders the effect by location.
		/// </summary>
		/// <remarks>
		/// The general math is identical between RenderEffectByLocation and RenderEffect.
		/// The RenderEffect draws the garland pattern in one pass adjusting the Y position to get the pattern.		
		/// The RenderEffectByLocation has to process each row in the pattern separately since each row that 
		/// makes up the pattern could be a different number of pixels.
		/// </remarks>		
		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			IList<Tuple<int, int[]>> sparseMatrix = GetSparseMatrix(frameBuffer);

			for (int frame = 0; frame < numFrames; frame++)
			{
				frameBuffer.CurrentFrame = frame;

				// This tells us where we are in the effect duration normalized 0 - 1
				double intervalPos = GetEffectTimeIntervalPosition(frame);

				// This is where we are in the effect duration normalized to 0 - 100
				double intervalPosFactor = intervalPos * 100;

				// Get the brightness of the effect
				double brightnessLevel = LevelCurve.GetValue(intervalPosFactor) / 100;

				int height = sparseMatrix.Count;

				// Get a value between 1 and 20 pixels
				int pixelSpacing = CalculateSpacing(intervalPosFactor) * height / 100 + 3;

				int limit = height * pixelSpacing * 4;
				int garlandsState;
				if (MovementType == MovementType.Iterations)
				{
					garlandsState = (int)(limit - _frames * (limit / GetNumberFrames() * Iterations)) / 4; 
					if (garlandsState <= 0)
					{
						_frames = 0;
					}
				}
				else
				{
					_speed += CalculateSpeed(intervalPosFactor);
					garlandsState = (limit - _speed % limit) / 4; //Speed
				}
				_frames++;

				DrawGarlandPattern(sparseMatrix, height, pixelSpacing, brightnessLevel, garlandsState, frame, frameBuffer);
			}
		}

		void DrawGarlandPattern(
			IList<Tuple<int, int[]>> sparseMatrix,
			int height,
			int pixelSpacing,
			double brightnessLevel,
			int garlandsState,
			int frame,
			IPixelFrameBuffer frameBuffer)
		{
			for (int ring = 0; ring < height; ring++)
			{
				var ratio = ring / (double)height;
				Color col = GetMultiColorBlend(ratio, false, frame);
				if (brightnessLevel < 1)
				{
					HSV hsv = HSV.FromRGB(col);
					hsv.V = hsv.V * brightnessLevel;
					col = hsv.ToRGB();
				}

				int y = garlandsState - ring * pixelSpacing;
				int ylimit = height - ring - 1;

				if (y < ylimit)
				{
					y = ylimit;
				}

				switch (Type)
				{
					case 0:
						DrawPattternType0(sparseMatrix, frameBuffer, y, ylimit, height, col);
						break;
					case 1:
						DrawPatternType1(sparseMatrix, frameBuffer, y, ylimit, height, col);
						break;
					case 2:
						DrawPatternType2(sparseMatrix, frameBuffer, y, ylimit, height, col);
						break;
					case 3:
						DrawPatternType3(sparseMatrix, frameBuffer, y, ylimit, height, col);
						break;
					case 4:
						DrawPatternType4(sparseMatrix, frameBuffer, y, ylimit, height, col);
						break;
					default:
						Debug.Assert(false, "Unsupported Garland Type");
						break;
				}
			}
		}

		/// <summary>
		/// Type 0 is a solid line pattern.
		/// </summary>		
		void DrawPattternType0(
			IList<Tuple<int, int[]>> sparseMatrix,
			IPixelFrameBuffer frameBuffer,
			int y,
			int yLimit,
			int height,
			Color col)
		{
			DrawPatternRing(0, y, yLimit, height, sparseMatrix, frameBuffer, col, () => { return 1; });
		}

		/// <summary>
		/// Type 1 is the following repeating pattern:
		/// 
		///      *		
		///    *   *		
		///  *       * *
		///  0 1 2 3 4 5
		/// </summary>		
		void DrawPatternType1(IList<Tuple<int, int[]>> sparseMatrix,
			IPixelFrameBuffer frameBuffer,
			int y,
			int yLimit,
			int height,
			Color col)
		{
			DrawPatternRing(0, y, yLimit, height, sparseMatrix, frameBuffer, col, new int[2] { 4, 1 });
			DrawPatternRing(1, y - 1, yLimit, height, sparseMatrix, frameBuffer, col, new int[2] { 2, 3 });
			DrawPatternRing(2, y - 2, yLimit, height, sparseMatrix, frameBuffer, col, () => { return 5; });
		}

		/// <summary>
		/// Type 1 is the following repeating pattern:
		/// 
		///      * 
		/// 
		///    *   *
		///       
		///  *       * *
		///  0 1 2 3 4 5
		/// </summary>				
		void DrawPatternType2(
			IList<Tuple<int, int[]>> sparseMatrix,
			IPixelFrameBuffer frameBuffer,
			int y,
			int yLimit,
			int height,
			Color col)
		{
			DrawPatternRing(0, y, yLimit, height, sparseMatrix, frameBuffer, col, new int[2] { 4, 1 });
			DrawPatternRing(1, y - 2, yLimit, height, sparseMatrix, frameBuffer, col, new int[2] { 2, 3 });
			DrawPatternRing(2, y - 4, yLimit, height, sparseMatrix, frameBuffer, col, () => { return 5; });
		}

		/// <summary>
		/// Type 3 is the following repeating pattern:
		/// 
		///        * 
		/// 
		///      *   *
		/// 
		///    *       *
		///             
		///  *           *
		///  0 1 2 3 4 5 6
		/// </summary>		
		void DrawPatternType3(
			IList<Tuple<int, int[]>> sparseMatrix,
			IPixelFrameBuffer frameBuffer,
			int y,
			int yLimit,
			int height,
			Color col)
		{
			DrawPatternRing(0, y, yLimit, height, sparseMatrix, frameBuffer, col, () => { return 6; });
			DrawPatternRing(1, y - 2, yLimit, height, sparseMatrix, frameBuffer, col, new int[2] { 4, 2 });
			DrawPatternRing(2, y - 4, yLimit, height, sparseMatrix, frameBuffer, col, new int[2] { 2, 4 });
			DrawPatternRing(3, y - 6, yLimit, height, sparseMatrix, frameBuffer, col, () => { return 6; });
		}

		/// <summary>
		/// Type 4 is the following repeating pattern:
		/// 
		///   *   *
		/// *   *   * *
		/// 0 1 2 3 4 5
		/// </summary>		
		void DrawPatternType4(
			IList<Tuple<int, int[]>> sparseMatrix,
			IPixelFrameBuffer frameBuffer,
			int y,
			int yLimit,
			int height,
			Color col)
		{
			DrawPatternRing(0, y, yLimit, height, sparseMatrix, frameBuffer, col, new int[3] { 2, 2, 1 });
			DrawPatternRing(1, y - 2, yLimit, height, sparseMatrix, frameBuffer, col, new int[2] { 2, 3 });
		}

		private void DrawPatternRing(
			int startPosition,
			int y,
			int yLimit,
			int height,
			IList<Tuple<int, int[]>> sparseMatrix,
			IPixelFrameBuffer frameBuffer,
			Color col,
			int[] increments)
		{
			PatternIncrement pattern = new PatternIncrement(increments);
			DrawPatternRing(startPosition, y, yLimit, height, sparseMatrix, frameBuffer, col, pattern.GetIncrement);
		}

		private void DrawPatternRing(
			int startPosition,
			int y,
			int yLimit,
			int height,
			IList<Tuple<int, int[]>> sparseMatrix,
			IPixelFrameBuffer frameBuffer,
			Color col,
			Func<int> increment)
		{
			if (y < yLimit)
			{
				y = yLimit;
			}

			if (y < height)
			{
				int rowIndex = 0;
				if (Direction == GarlandsDirection.Up || Direction == GarlandsDirection.Left)
				{
					rowIndex = height - y - 1;
				}
				else
				{
					rowIndex = y;
				}

				for (int x = startPosition; x < sparseMatrix[rowIndex].Item2.Length; x += increment())
				{
					if (Direction == GarlandsDirection.Up || Direction == GarlandsDirection.Down)
					{
						frameBuffer.SetPixel(sparseMatrix[rowIndex].Item2[x], sparseMatrix[rowIndex].Item1, col);
					}
					else
					{
						frameBuffer.SetPixel(sparseMatrix[rowIndex].Item1, sparseMatrix[rowIndex].Item2[x], col);
					}
				}
			}
		}

		/// <summary>
		/// Encapsulates a dynamic for loop increment that is used to draw a pattern.
		/// </summary>
		private class PatternIncrement
		{
			#region Fields

			private int _incrementIndex = 0;
			private int[] _increments;

			#endregion

			#region Public Methods

			public PatternIncrement(int[] increments)
			{
				_increments = increments;
			}
			public int GetIncrement()
			{
				int increment = _increments[_incrementIndex];

				if (_incrementIndex < _increments.Length - 1)
				{
					_incrementIndex++;
				}
				else
				{
					_incrementIndex = 0;
				}

				return increment;
			}

			#endregion
		}

		private IList<Tuple<int, int[]>> GetSparseMatrix(PixelLocationFrameBuffer frameBuffer) 
		{
			IList<Tuple<int, int[]>> sparseMatrix = new List<Tuple<int, int[]>>();
			
			if (Direction == GarlandsDirection.Up || Direction == GarlandsDirection.Down)
			{
				IEnumerable<IGrouping<int, ElementLocation>> nodes = 
					frameBuffer.ElementLocations.OrderBy(elementLocation => elementLocation.Y).ThenBy(elementLocation => elementLocation.X).GroupBy(elementLocation => elementLocation.Y);

				foreach (IGrouping<int, ElementLocation> group in nodes)
				{
					sparseMatrix.Add(new Tuple<int, int[]>(group.Key, group.Select(elementLocation => elementLocation.X).ToArray()));
				}
			}
			else
			{
				IEnumerable<IGrouping<int, ElementLocation>> nodes = 
					frameBuffer.ElementLocations.OrderBy(elementLocation => elementLocation.X).ThenBy(elementLocation => elementLocation.Y).GroupBy(elementLocation => elementLocation.X);

				foreach (IGrouping<int, ElementLocation> group in nodes)
				{
					sparseMatrix.Add(new Tuple<int, int[]>(group.Key, group.Select(elementLocation => elementLocation.Y).ToArray()));
				}
			}
									
			return sparseMatrix;
		}			
	}
}
