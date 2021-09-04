using Common.Controls.ColorManagement.ColorModels;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Windows.Media;

namespace VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Shaders
{
	/// <summary>
	/// Shader for coloring gray graphical volumes that make up the fixture.
	/// </summary>
	public class GrayVolumeShader : VolumeShader, ISpecifyVolumeIntensity
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public GrayVolumeShader() : base(VertexShader, FragmentShader)
		{
			// Default the intensity to full intensity
			Intensity = 255;
		}

		#endregion

		#region Static Constructor

		/// <summary>
		/// Static Constructor
		/// </summary>
		static GrayVolumeShader()
		{
			// Initialize the unique ID
			ShaderID = Guid.NewGuid();
		}

		#endregion

		#region Static Public Properties

		/// <summary>
		/// Unique identifier for the shader.
		/// </summary>
		static public Guid ShaderID { get; set; }

		#endregion
	
		#region Static Fields
		
		/// <summary>
		/// GLSL source for the fragment shader.
		/// </summary>
		private static string FragmentShader = @"
		#version 330
 
		uniform vec3 lightPosition;		
		uniform vec3 viewPosition;
		uniform vec3 grayColor;

		in vec3 v_norm;
		in vec3 FragPos;

		out vec4 outputColor;
 
		void main()
		{    			
		    // Ambient Light
		    float ambientStrength = 0.3;
		    vec3 diffuseColor = vec3(1.f, 1.f, 1.f);
		    vec3 ambient = ambientStrength * diffuseColor;

			// Diffuse Light
			vec3 norm = normalize(v_norm);
			vec3 lightDir = normalize(lightPosition - FragPos);
			float diff = max(dot(norm, lightDir), 0.0);    
			vec3 diffuse = diff * diffuseColor;

			// Define the volume color
			//vec3 grayColor = grayColor;

		    // Specular Light
		    float specularStrength = 0.2;
		    vec3 viewDir = normalize(viewPosition - FragPos);
		    vec3 reflectDir = reflect(-lightDir, norm);    
		    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
		    vec3 specular = specularStrength * spec * grayColor;
			
		    // Calculate the overall color
		    vec3 result = (ambient + diffuse + specular) * grayColor.xyz; 
		    
		    outputColor = vec4(result, 1.0f);    
		}
		";

		#endregion

		#region ISpecifyVolumeIntensity

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public int Intensity { get; set; }

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the gray color of the volume taking into account the intensity.
		/// </summary>
		/// <returns>Gray color for the shader taking into account the intensity</returns>
		private Color GetGrayColor()
		{
			// Convert the RGB color to HSV format
			HSV hsv = HSV.FromRGB(System.Drawing.Color.Gray);

			// Update the color for the intensity 
			hsv.V *= Intensity / 255.0;

			// Convert the HSV color back to RGB
			System.Drawing.Color color = hsv.ToRGB();

			// Get the beam color taking into account the intensity
			return Color.FromArgb(color.A, color.R, color.G, color.B);
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>		
		protected override void TransferGlobalUniforms(
			Vector3? lightPosition,
			Matrix4? projectionMatrix,
			Matrix4? viewMatrix)
		{
			// Call the base class implementation
			base.TransferGlobalUniforms(lightPosition, projectionMatrix, viewMatrix);

			// Convert the gray color to a vector and tranfer it to the GPU
			Color c = GetGrayColor();
			GL.Uniform3(GetUniform("grayColor"), new Vector3(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f));
		}

		#endregion
	}
}
