using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using Common.WPFCommon.Command;

using OpenTK.WinForms;

namespace VixenModules.Preview.VixenPreview.OpenGL
{
	/// <summary>
	/// View model for the WPF OpenGL preview.
	/// </summary>
	public class WPFOpenGLPreviewViewModel : INotifyPropertyChanged, IDisposable
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data">Preview data</param>
		/// <param name="instanceID">Instance ID of the preview</param>
		public WPFOpenGLPreviewViewModel(
			VixenPreviewData data, 
			Guid instanceID,
			OpenGLPreviewDrawingEngine previewDrawingEngine)
		{
			// Save off the preview drawing engine
			DrawingEngine = previewDrawingEngine; 

			// Create Context Menu commands
			ResetSize = new RelayCommand(ExecuteResetSize, CanExecuteResetSize);
			CenterPreview = new RelayCommand(ExecuteCenterPreview);

			// Initialize callback delegates
			DrawingEngine.UpdateStatusDistance = UpdateStatusDistance;
			DrawingEngine.UpdateFrmRate = UpdateFrameRate;
		}

		#endregion

		#region INotifyPropertyChanged

		/// <summary>
		/// Event occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Protected Methods

		/// <summary>
		/// Helper method to fire the property changed event.
		/// </summary>
		/// <param name="propertyName">Property name that changed</param>
		protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			// Fire the property changed event if any objects are registered
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		#region Public Properties

		private int _pixelCount;

		/// <summary>
		/// Pixel or light count.
		/// </summary>
		public int PixelCount 
		{
			get
			{
				return _pixelCount;		
			}
			set
			{
				// If the pixel count has changed then...
				if (_pixelCount != value)
				{
					_pixelCount = value;
					OnPropertyChanged();
				}
			}
		}

		private long _frameRate;

		/// <summary>
		/// Frame rate of the preview.
		/// </summary>
		public long FrameRate
		{
			get
			{
				return _frameRate;
			}
			set
			{
				// If the frame rate has changed then...
				if (_frameRate != value)
				{
					_frameRate = value;
					OnPropertyChanged();
				}
			}
		}

		private float _distance;

		/// <summary>
		/// Distance from the preview in the Z-axis.
		/// </summary>
		public float Distance
		{
			get
			{
				return _distance;
			}
			set
			{
				// If the distance has changed then...
				if (_distance != value)
				{
					_distance = value;
					OnPropertyChanged();
				}
			}
		}

		private string _left;

		/// <summary>
		/// Left coordinate of the preview as a string.
		/// </summary>
		public string Left 
		{ 
			get
			{
				return _left;
			}
			set
			{
				// If the left coordinate has changed then...
				if (_left != value)
				{
					_left = value;
					OnPropertyChanged();
				}
			}
		}

		private string _top;

		/// <summary>
		/// Top coordinate of the preview as a string.
		/// </summary>
		public string Top
		{
			get
			{
				return _top;
			}
			set
			{
				_top = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// OpenGL preview drawing engine.
		/// </summary>
		public OpenGLPreviewDrawingEngine DrawingEngine { get; set; }


		private bool _showStatusChecked;

		/// <summary>
		/// Flag to determine if the Status bar is displayed in the preview.
		/// </summary>
		public bool ShowStatusChecked
		{
			get
			{
				return _showStatusChecked;
			}
			set
			{
				// If the flag changed then...
				if (_showStatusChecked != value)
				{
					_showStatusChecked = value;
					OnPropertyChanged();
				}
			}
		}

		private bool _alwaysOnTop;

		/// <summary>
		/// Flag that determines if the preview should always be the top window.
		/// </summary>
		public bool AlwaysOnTop
		{
			get
			{
				return _alwaysOnTop;
			}
			set
			{
				// If the flag changed then...
				if (_alwaysOnTop != value)
				{
					_alwaysOnTop = value;					
					OnPropertyChanged();
				}
			}
		}

		private bool _autoOnTop;

		/// <summary>
		/// Preview should automatically appear on top when playing content.
		/// </summary>
		public bool AutoOnTop
		{
			get
			{
				return _autoOnTop;
			}
			set
			{
				// If the flag has changed then...
				if (_autoOnTop != value)
				{
					_autoOnTop = value;

					// If auto mode is enabled then...
					if (_autoOnTop)
					{
						// Don't force the window to the top
						AlwaysOnTop = false;
					}
					OnPropertyChanged();
				}
			}
		}

		private string _width;

		/// <summary>
		/// Width of the preview as a string.
		/// </summary>
		public string Width
		{
			get
			{
				return _width;
			}
			set
			{
				_width = value;
				OnPropertyChanged();
			}
		}

		private string _height;

		/// <summary>
		/// Height of the preview as a string.
		/// </summary>
		public string Height
		{
			get
			{
				return _height;
			}
			set
			{
				_height = value;
				OnPropertyChanged();
			}
		}

		private System.Windows.Size _clientSize;

		/// <summary>
		/// Client size of the preview.
		/// </summary>
		public System.Windows.Size ClientSize 
		{ 
			get
			{
				return _clientSize; 
			}
			set
			{
				// If the client size has changed then...
				if (_clientSize != value)
				{
					_clientSize = value;
					OnPropertyChanged();
				}
			}
		}
		
		/// <summary>
		/// Flag determining if light scaling is enabled.
		/// </summary>
		public bool EnableLightScaling
		{
			get
			{
				return DrawingEngine.EnableLightScaling;
			}
			set
			{
				// If the flag changed then...
				if (value != DrawingEngine.EnableLightScaling)
				{
					DrawingEngine.EnableLightScaling = value;
					OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Command to reset the size of the preview.
		/// </summary>
		public ICommand ResetSize { get; private set; }

		/// <summary>
		/// Command to centers the preview.
		/// </summary>
		public ICommand CenterPreview { get; private set; }

		#endregion

		#region Private Methods

		/// <summary>
		/// Center preview command handler.
		/// </summary>
		private void ExecuteCenterPreview()
		{
			// Center the preview and return the new width and height
			ClientSize = DrawingEngine.ExecuteCenterPreview();
		}

		/// <summary>
		/// Reset size command handler.
		/// </summary>
		private void ExecuteResetSize()
		{
			// Reset the size of the preview and return the new width and height
			ClientSize = DrawingEngine.ResetSize();
		}

		/// <summary>
		/// Returns true when the Reset Size command should be enabled.
		/// </summary>
		/// <returns>True when the reset size command should be enabled</returns>
		private bool CanExecuteResetSize()
		{
			// The reset size command is only enabled when the preview has a background image
			return DrawingEngine.HasBackgroundImage();
		}		

		/// <summary>
		/// Call back method to update the distance on the Z-axis.
		/// </summary>
		/// <param name="distance">Updated distance on the Z-axis</param>
		private void UpdateStatusDistance(float distance)
		{
			// Set the Distance property
			Distance = distance;
		}

		/// <summary>
		/// Call back method to update the frame rate.
		/// </summary>
		/// <param name="frameRate">Updated frame rate</param>
		private void UpdateFrameRate(long frameRate)
		{
			// Set the frame rate property
			FrameRate = frameRate;
		}

		#endregion

		#region IDisposable
		
		/// <inheritdoc/>		
		public void Dispose()
		{
			// Dispose the OpenGL drawing engine
			DrawingEngine.Dispose();	
			DrawingEngine = null;			
		}

		#endregion
	}
}
