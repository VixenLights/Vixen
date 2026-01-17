using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

using Common.OpenGLCommon;
using Common.OpenGLCommon.Constructs;
using Common.OpenGLCommon.Constructs.Shaders;
using Common.OpenGLCommon.Constructs.Vertex;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview.OpenGL
{
	/// <summary>
	/// Draws the background for the preview.
	/// </summary>
	public class PropPreviewBackground : IDrawable
	{
		#region Static Properties

		/// <summary>
		/// Logging component.
		/// </summary>
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		#endregion

		#region Fields

		/// <summary>
		/// Texture used to draw the background image.
		/// </summary>
		private Texture _backgroundTexture;

		/// <summary>
		/// File name of the background image.
		/// </summary>
		private string _backgroundFilename;
		
		/// <summary>
		/// Brightness of the background.
		/// </summary>
		private float _backgroundBrightness;		

		/// <summary>
		/// Shader program used to draw the background.
		/// </summary>
		private BackgroundShaderProgram _backgroundProgram;
		
		/// <summary>
		/// Points that form a rectangle.
		/// </summary>
		private VBO<float> _points;		
		
		/// <summary>
		/// Element Array Buffer (EBO) or index buffer that split the rectangle into two triangles.
		/// </summary>
		private VBO<int> _backgroundElements;

		/// <summary>
		/// Flag to allow first pass initialization of the VAO.
		/// </summary>
		private bool _renderInitComplete = false;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor 
		/// </summary>
		/// <param name="backgroundFilename">Complete path to background filename</param>
		/// <param name="brightness">Desired brightness of the background</param>
		public PropPreviewBackground(string backgroundFilename, float brightness)
		{		
			// Store off path to background image
			_backgroundFilename = backgroundFilename;	
			
			// Store off backbround brightness
			_backgroundBrightness = brightness;
			
			// Initialize the background drawing (OpenGL Shader)
			InitializeBackground();
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Height of the background image.
		/// </summary>
		public int Height { get; set; }

		/// <summary>
		/// Width of the background image.
		/// </summary>
		public int Width { get; set; }

		/// <summary>
		/// Flag that indicates if there is a background image.
		/// </summary>
		public bool HasBackground { get; private set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Draws the background image.
		/// </summary>
		/// <param name="fov"></param>
		/// <param name="cameraView"></param>
		public void Draw(Matrix4 fov, Matrix4 cameraView)
		{
			// If there is not a background then...
			if (!HasBackground)
			{
				// Just exit the method
				return;
			}
			
			// Activate the background shader program
			_backgroundProgram.Use();
			
			// Transfer the projection and camera view matrices			
			_backgroundProgram.TransferGlobalUniforms(fov, cameraView);

			// If this is the first time rendering the background then...
			if (!_renderInitComplete)
			{
				// Bind the points and their relationship to the texture to the VAO
				GlUtility.BindBuffer(_points);

				// Define the attribute indicees
				const int VertexPosition = 0;
				const int TextureCoords = 1;

				// Vertex Position
				GL.VertexAttribPointer(VertexPosition, 3, VertexAttribPointerType.Float, false, 5 * Marshal.SizeOf(typeof(float)), IntPtr.Zero);
				GL.EnableVertexAttribArray(VertexPosition);
								
				// Texture Coords
				GL.VertexAttribPointer(TextureCoords, 2, VertexAttribPointerType.Float, false, 5 * Marshal.SizeOf(typeof(float)), Vector3.SizeInBytes);
				GL.EnableVertexAttribArray(TextureCoords);

				// Bind the EBO to the VAO
				GlUtility.BindBuffer(_backgroundElements);

				// Indicate that render initialization has been completed
				_renderInitComplete = true;	
			}

			// Active the background image texture
			GL.ActiveTexture(TextureUnit.Texture0);			
			GlUtility.BindTexture(_backgroundTexture);

			// Draw the two triangles
			GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero);

			// Clear the vertex array
			GL.BindVertexArray(0);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Initialize the background texture.
		/// </summary>
		private void InitializeBackground()
		{
			// If the path to background is empty then...
			if (string.IsNullOrEmpty(_backgroundFilename))
			{
				// Indicate there is NOT a background and return
				HasBackground = false;
				
				return;
			}
			
			// Create the complete path to the background image
			string file = Path.Combine(Paths.ModuleDataFilesPath + "\\VixenDisplayPreview", _backgroundFilename);

			// Load the background bitmap
			Bitmap backgroundBitmap = File.Exists(file) ? new Bitmap(Image.FromFile(file)) : null;

			// If the bitmap does not load then...
			if (backgroundBitmap == null)
			{
				// Indicate there is NOT a background and return
				HasBackground = false;

				return;
			}

			// Extract the dimensions of the background bitmap
			Width = backgroundBitmap.Width;
			Height = backgroundBitmap.Height;
			
			// Create the background shader program
			_backgroundProgram = new BackgroundShaderProgram();

			// Activate the background program
			_backgroundProgram.Use();

			// Check if there were any problems with the background shader program
			string log = _backgroundProgram.ProgramLog;
			if (!string.IsNullOrWhiteSpace(log))
			{
				Logging.Info("Texture program log: {0}", log);
			}
			
			// Transfer the model matrix
			_backgroundProgram.TransferModelMatrixUniform(Matrix4.Identity);

			// Transfer the brightness of the background
			_backgroundProgram.TransferBrightnessUniform(_backgroundBrightness);
			
			// Create a texture from the bitmap
			_backgroundTexture = new Texture(backgroundBitmap);

			// Create a relationship between the points and the texture
			_points = new VBO<float>(new[] {
				// Positions                               Tex Coord
				Width / 2.0f,	Height / 2.0f,	-.1f,      1f, 1f,		// Top Right
				Width / 2.0f,	-Height / 2.0f,	-.1f,      1f, 0f,	    // Bottom Right
				-Width / 2.0f,	-Height / 2.0f,	-.1f,      0f, 0f,		// Bottom Left
				-Width / 2.0f,	Height / 2.0f,	-.1f,      0f, 1f		// Top Left
			});
			
			// Form two triangles by selecting indices (EBO)
			_backgroundElements = new VBO<int>(new [] { 0, 1, 3, 1, 2, 3 }, BufferTarget.ElementArrayBuffer);

			// Indicate there is a background
			HasBackground = true;
		}
		
		#endregion

		#region IDisposable

		/// <inheritdoc/>
		public void Dispose()
		{			
			if (_backgroundTexture != null)
			{
				// Dispose the background texture			
				_backgroundTexture.Dispose();
				_backgroundTexture = null;	
			}

			if (_points != null)
			{
				// Dispose the rectangle points
				_points.Dispose();
				_points = null;
			}

			if (_backgroundElements != null)
			{
				// Dispose of the EBO / VBO
				_backgroundElements.Dispose();
				_backgroundElements = null;
			}			
		}

		#endregion
	}
}
