using Common.Controls.ColorManagement.ColorModels;
using FastPixel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Vixen.Attributes;
using Vixen.Marks;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.App.Polygon;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.NorthStar
{
	/// <summary>
	/// Morph pixel effect.
	/// </summary>
	public class Morph : PixelEffectBase
	{
		#region Private Fields

		/// <summary>
		/// Data associated with the effect.
		/// </summary>
		private MorphData _data;

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
		/// Since Vixen can be refreshing at any rate, this field keeps track of the frame time so that
		/// we can render the effect as close to 20 Hz as possible.
		/// </summary>
		private int _frameTime;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public Morph()
		{
			// Enable both string and location positioning
			EnableTargetPositioning(true, true);
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
		[ProviderCategory(@"Polygon", 1)]
		[ProviderDisplayName(@"PolygonModel")]
		[ProviderDescription(@"PolygonModel")]
		[PropertyOrder(1)]
		public Polygon PolygonModel { get; set; }

		/// <summary>
		/// Module data associated with the effect.
		/// </summary>
		public override IModuleDataModel ModuleData
		{
			get
			{
				// Return the effect data
				return _data;
			}
			set
			{
				// Save off the data for the effect
				_data = value as MorphData;

				// Determine if the Render Scale Factor should be visible				
				UpdateAttributes();

				// Mark the effect as dirty
				MarkDirty();
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

		int _width;
		int _widthIncrement;
		int _height;
		int _heightIncrement;

        /// <summary>
        /// Render's the effect for the specified frame.
        /// </summary>		
        protected override void RenderEffect(int frameNum, IPixelFrameBuffer frameBuffer)
        {
            // Get the position within the effect
            double intervalPos = GetEffectTimeIntervalPosition(frameNum);
            double intervalPosFactor = intervalPos * 100;

            int centerX = _bufferWi / 2;
            int centerY = _bufferHt / 2;

            //_width = maxWidth;
            //_height = maxHeight;

            //int verticalPointWidth = _bufferWi / 24;
            //int horizontalPointWidth = _bufferHt / 8;

            //SolidBrush yellowBrush = new SolidBrush(Color.Yellow);

            /*
			//
			// Left Point
			//					
			int pointLength = width;
			DrawPoint(frameBuffer, centerX, centerY, centerX - pointLength, centerY, 2, Color.Yellow);
			
			//
			// Right Point
			//
			pointLength = width;
			DrawPoint(frameBuffer, centerX, centerY, centerX + pointLength, centerY, 2, Color.Yellow);
			
			//
			// Bottom Vertical Point
			//
			pointLength = _bufferHt / 2 - 4;
			DrawPoint(frameBuffer, centerX, centerY, centerX, centerY - pointLength, 6, Color.Yellow);
			
			//
			// Top Vertical Point
			//		
			pointLength = _bufferHt / 2 - 4;
			DrawPoint(frameBuffer, centerX, centerY, centerX, centerY + pointLength, 6, Color.Yellow);				
			*/
            DrawStar(frameBuffer, Color.Yellow, centerX, centerY, _width, _height);

            
            if (frameNum % 8 == 0)
            {
                _width += _widthIncrement;
                _height += _heightIncrement;
            }

            
            if (_width > _maxWidth || _height > _maxHeight)
            {
                _widthIncrement = -1 * _widthIncrement;
                _heightIncrement = -1 * _heightIncrement;
            }
            

            if (_width > _maxWidth)
            {
                _width = _maxWidth;
            }

            if (_height > _maxHeight)
            {
                _height = _maxHeight;
            }

            if (_width < _minWidth || _height < _minHeight)
            {
                _widthIncrement = -1 * _widthIncrement;
                _heightIncrement = -1 * _heightIncrement;
            }

            if (_width < _minWidth)
            {
                _width = _minWidth;
            }

            if (_height < _minHeight)
            {
                _height = _minHeight;
            }
            
            
            /*            
            using (var bitmap = new Bitmap(_bufferWi, _bufferHt))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    Pen pen = new Pen(Color.Yellow);

                    //graphics.DrawLine(pen, _bufferWi / 2, 0, _bufferWi / 2, _bufferHt);
                    int width = _bufferHt / 4;
                   

                    //int centerX = _bufferWi / 2;
                    //int centerY = _bufferHt / 2;
                    //int verticalPointWidth = _bufferWi / 24;
                    //int horizontalPointWidth = _bufferHt / 8;

                    SolidBrush yellowBrush = new SolidBrush(Color.Yellow);

                    //graphics.DrawLine(pen, centerX, centerY, centerX + 5, centerY + 5);
                    //graphics.DrawLine(pen, centerX + 1, centerY, centerX + 4, centerY + 3);
                    //graphics.DrawLine(pen, centerX, centerY + 1, centerX + 3, centerY + 4);


                    //_bufferHt / 4, _bufferHt / 2 - 4); ;

                    //List<PointF> points = new List<PointF>();
                    //points.Add(new PointF(centerX, centerY + 10));
                    //points.Add(new PointF(centerX - 10, centerY + 10));
                    //points.Add(new PointF(centerX - 10, centerY));

                    //graphics.FillPolygon(yellowBrush, points.ToArray());
                }

                FastPixel.FastPixel fp = new FastPixel.FastPixel(bitmap);
                fp.Lock();
                // copy to frameBuffer
                for (int x = 0; x < _bufferWi; x++)
                {
                    for (int y = 0; y < _bufferHt; y++)
                    {
                        CalculatePixel(x, y, _bufferHt, fp, frameBuffer);
                    }
                }
                fp.Unlock(false);
                fp.Dispose();                
            } 
            */
        }


        private void DrawStar(
			IPixelFrameBuffer frameBuffer, 
			Color color, 
			int centerX, 
			int centerY, 
			int width,
			int height)
		{                   
            //width -= 3;
            //height -= 3;

            int borderLength = (int)(height * .25);

            if (borderLength == 0)
            {
                borderLength = 1;
            }


            //
            // Left Point
            //					
            int pointLength = width - borderLength;
			DrawPoint(frameBuffer, centerX, centerY, centerX - width, centerY, 2, color, borderLength);

			//
			// Right Point
			//
			pointLength = width - borderLength;
			DrawPoint(frameBuffer, centerX, centerY, centerX + width, centerY, 2, color, borderLength);

			//
			// Bottom Vertical Point
			//			
			DrawPoint(frameBuffer, centerX, centerY, centerX, centerY - height + borderLength, 6, color, borderLength);

			//
			// Top Vertical Point
			//					
			DrawPoint(frameBuffer, centerX, centerY, centerX, centerY + height - borderLength, 6, color, borderLength);
		}
	
				
		private void DrawPoint(
			IPixelFrameBuffer frameBuffer, 			
			int centerX, 
			int centerY,
			int endX,
			int endY,			
			int slopeChange,
			Color color,
            int borderLength)
		{
			bool horizontal = (endY == centerY);
			
			if (horizontal)
			{
				bool left = endX < centerX;

				if (left)								
				{
					int pointLength = centerX - endX;
					DrawLine(centerX, centerY, centerX - pointLength, centerY, color, frameBuffer, borderLength);

					int yOffset = 1;
					for (int ptLength = pointLength - slopeChange; ptLength > 0; ptLength -= slopeChange)
					{
						DrawLine(centerX, centerY + yOffset, centerX - ptLength, centerY + yOffset, color, frameBuffer, borderLength);
						DrawLine(centerX, centerY - yOffset, centerX - ptLength, centerY - yOffset, color, frameBuffer, borderLength);

						yOffset++;
					}
				}
				else
				{
					int pointLength = endX - centerX;
					DrawLine(centerX, centerY, centerX + pointLength, centerY, color, frameBuffer, borderLength);

					int yOffset = 1;
					for (int ptLength = pointLength - slopeChange; ptLength > 0; ptLength -= slopeChange)
					{
						DrawLine(centerX, centerY + yOffset, centerX + ptLength, centerY + yOffset, color, frameBuffer, borderLength);
						DrawLine(centerX, centerY - yOffset, centerX + ptLength, centerY - yOffset, color, frameBuffer, borderLength);

						yOffset++;
					}
				}
			}
			else
			{
				bool up = endY > centerY;

				if (up)
				{
					int pointLength = endY - centerY;
					DrawLine(centerX, centerY, endX, centerY + pointLength, color, frameBuffer, borderLength);

					int xOffset = 1;
					for (int ptLength = pointLength - slopeChange; ptLength > 0; ptLength -= slopeChange)
					{
						DrawLine(centerX + xOffset, centerY, centerX + xOffset, centerY + ptLength, color, frameBuffer, borderLength);
						DrawLine(centerX - xOffset, centerY, centerX - xOffset, centerY + ptLength, color, frameBuffer, borderLength);

						xOffset++;
					}
				}
				else
				{
					int pointLength = centerY - endY;
					DrawLine(centerX, centerY, endX, centerY - pointLength, color, frameBuffer, borderLength);

					int xOffset = 1;
					for (int ptLength = pointLength - slopeChange; ptLength > 0; ptLength -= slopeChange)
					{
						DrawLine(centerX + xOffset, centerY, centerX + xOffset, centerY - ptLength, color, frameBuffer, borderLength);
						DrawLine(centerX - xOffset, centerY, centerX - xOffset, centerY - ptLength, color, frameBuffer, borderLength);

						xOffset++;
					}
				}
			}			
		}
		
		private double GetIntensity(int x, int y)
		{
			int centerX = _bufferWi / 2;
			int centerY = _bufferHt / 2;

			double intensity = 1.0;			

			double distance = Math.Sqrt((y - centerY) * (y - centerY) + (x - centerX) * (x - centerX));

			//if (distance > _width || distance > _height)
			{
				intensity = 1.0 / (1 + distance) + 0.25;

				if (intensity > 1.0)
				{
					intensity = 1.0;
				}
			}

			return intensity;
		}


		private void DrawLine(int startX, int startY, int endX, int endY, Color color, IPixelFrameBuffer frameBuffer, int borderLength)
		{
			bool horizontal = (startY == endY);
			
			bool down = false;

			if (startX > endX)
			{
				int temp = endX;
				endX = startX;
				startX = temp;
				down = true;
			}

			if (startY > endY)
			{
				int temp = endY;
				endY = startY;
				startY = temp;
				down = true;
			}

            int centerX = _bufferWi / 2;
            int centerY = _bufferHt / 2;

            
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
					double distance = Math.Sqrt((y - centerY) * (y - centerY) + (x - centerX) * (x - centerX)); 
                    HSV hsv = HSV.FromRGB(color);
					double intensity = 1;

					intensity = 1.0 / (1 + distance);

					/*
					if (x > centerX)
					{
						intensity = 1.0/ (1 + x - centerX);
					}
					else
					{
						intensity = 1.0 / (1 + centerX - x);
					}
					*/
					hsv.V = 1.0; // GetIntensity(x, y); // intensity; //1 -.5; //1 - distance * 1 / (_bufferWi / 2);
                    frameBuffer.SetPixel(x, y, hsv);                    
                }
            }
            
			if (horizontal)
			{
				if (down)
				{
                    for (int border = 0; border < borderLength; border++)
                    {
                        HSV hsv = HSV.FromRGB(Color.Yellow);
						hsv.V = GetIntensity(startX - 1 - border, endY); //hsv.V * 1 / (border + 1);

						frameBuffer.SetPixel(startX - 1 - border, endY, hsv);
                    }

					//hsv = HSV.FromRGB(Color.Yellow);
					//hsv.V = hsv.V * .4;
					//frameBuffer.SetPixel(startX - 2, endY, hsv);

					//hsv = HSV.FromRGB(Color.Yellow);
					//hsv.V = hsv.V * .3;
					//frameBuffer.SetPixel(startX - 3, endY, hsv);
				}
				else
				{
                    for (int border = 0; border < borderLength; border++)
                    {
                        HSV hsv = HSV.FromRGB(Color.Yellow);
						hsv.V = GetIntensity(endX + 1 + border, endY); //hsv.V * 1 / (border + 1);
						frameBuffer.SetPixel(endX + 1 + border, endY, hsv);
                    }
                    //HSV hsv = HSV.FromRGB(Color.Yellow);
					//hsv.V = hsv.V * .5;
					//frameBuffer.SetPixel(endX + 1, endY, hsv);

					//hsv = HSV.FromRGB(Color.Yellow);
					//hsv.V = hsv.V * .4;
					//frameBuffer.SetPixel(endX + 2, endY, hsv);

					//hsv = HSV.FromRGB(Color.Yellow);
					//hsv.V = hsv.V * .3;
					//frameBuffer.SetPixel(endX + 3, endY, hsv);
				}
			}
			else
			{
				if (down)
				{
                    for (int border = 0; border < borderLength; border++)
                    {
                        HSV hsv = HSV.FromRGB(Color.Yellow);
						hsv.V = GetIntensity(endX, startY - 1 - border); //hsv.V * 1 / (border + 1);
                        frameBuffer.SetPixel(endX, startY - 1 - border, hsv);
                    }

                    //HSV hsv = HSV.FromRGB(Color.Yellow);
					//hsv.V = hsv.V * .5;
					//frameBuffer.SetPixel(endX, startY - 1, hsv);

					//hsv = HSV.FromRGB(Color.Yellow);
					//hsv.V = hsv.V * .4;
					//frameBuffer.SetPixel(endX, startY - 2, hsv);

					//hsv = HSV.FromRGB(Color.Yellow);
					//hsv.V = hsv.V * .3;
					//frameBuffer.SetPixel(endX, startY - 3, hsv);
				} 
				else
				{
                    for (int border = 0; border < borderLength; border++)
                    {
                        HSV hsv = HSV.FromRGB(Color.Yellow);
						hsv.V = GetIntensity(endX, endY + 1 + border);//hsv.V * 1 / (border + 1);
                        frameBuffer.SetPixel(endX, endY + 1 + border, hsv);
                    }

                    //HSV hsv = HSV.FromRGB(Color.Yellow);
					//hsv.V = hsv.V * .5;
					//frameBuffer.SetPixel(endX, endY + 1, hsv);

					//hsv = HSV.FromRGB(Color.Yellow);
					//hsv.V = hsv.V * .4;
					//frameBuffer.SetPixel(endX, endY + 2, hsv);

					//hsv = HSV.FromRGB(Color.Yellow);
					//hsv.V = hsv.V * .3;
					//frameBuffer.SetPixel(endX, endY + 3, hsv);
				}
			}			
		}


		private static Color _emptyColor = Color.FromArgb(0, 0, 0, 0);

		private void CalculatePixel(int x, int y, int bufferHt, FastPixel.FastPixel bitmap, IPixelFrameBuffer frameBuffer)
		{
			int yCoord = y;
			int xCoord = x;
			if (TargetPositioning == TargetPositioningType.Locations)
			{
				//Flip me over so and offset my coordinates I can act like the string version
				y = Math.Abs((BufferHtOffset - y) + (bufferHt - 1 + BufferHtOffset));
				y = y - BufferHtOffset;
				x = x - BufferWiOffset;
			}
			Color color = bitmap.GetPixel(x, bufferHt - y - 1);

			if (!_emptyColor.Equals(color))
			{
				frameBuffer.SetPixel(xCoord, yCoord, color);
			}
			else if (TargetPositioning == TargetPositioningType.Locations)
			{
				frameBuffer.SetPixel(xCoord, yCoord, Color.Transparent);
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

            //_width = 2;
            //_height = 8;
            _width = 1; // _bufferHt / 4;
            _height = 1; // _bufferHt / 2;

            _minWidth = 1;
			_minHeight = 1;

            _maxWidth = _bufferHt / 4;
            _maxHeight = _bufferHt / 2;

            _widthIncrement = 1;
			_heightIncrement = 2; 
		}

		int _minWidth;
		int _minHeight;

        int _maxWidth; 
        int _maxHeight; 
        
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
			// Loop over the removed mark collections
			foreach (IMarkCollection markCollection in removedCollections)
			{
				// Unregister for property changed events from the mark collection
				markCollection.PropertyChanged -= MarkCollectionPropertyChanged;

				// TODO:
				// If any of the waves had the removed mark collection selected then...				
				//if (waves.Any(waveform => waveform.MarkCollectionId == markCollection.Id))
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
			// TODO: Do we need this method?
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
			UpdateStringOrientationAttributes();
			TypeDescriptor.Refresh(this);
		}
									
		/// <summary>
		/// Updates the mark collection on the waveforms.
		/// </summary>
		private void UpdateMarkCollectionNames()
		{									
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

		#endregion		
	}
}
