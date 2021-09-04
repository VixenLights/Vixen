using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;
using System.Drawing;
using VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Volumes;

namespace VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Shaders
{
	/// <summary>
	/// Shader for graphical volumes that are assigned a color.
	/// </summary>
	class ColorVolumeShader : VolumeShader
	{
		#region Constructor 

		/// <summary>
		/// Constructor
		/// </summary>
		public ColorVolumeShader() : base(VertexShader, FragmentShader)
		{
		}

		#endregion

		#region Static Constructor

		/// <summary>
		/// Static Constructor
		/// </summary>
		static ColorVolumeShader()
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

		#region Protected Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		protected override void TransferUniforms(IVolume volume)
		{
			// Call the base class implementation
			base.TransferUniforms(volume);

			// Verify the volume's color is configurable
			Debug.Assert(volume is ISpecifyVolumeColor);
			
			// Convert the color to a vector and tranfer it to the GPU
			Color c = ((ISpecifyVolumeColor)volume).Color;
			GL.Uniform3(GetUniform("color"), new Vector3(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f));

			// If the volume supports transparency then...
			if (volume is ISpecifyVolumeTransparency)
			{
				// Transfer the transparency setting to the GPU
				GL.Uniform1(GetUniform("transparency"), (float)((ISpecifyVolumeTransparency)volume).Transparency);
			}
		}

		#endregion

		#region Static Fields
		
		/// <summary>
		/// GLSL source for the fragment shader.
		/// </summary>
		private static string FragmentShader = @"
		#version 330
 
		uniform vec3 lightPosition;
		uniform vec3 lightColor;
		uniform vec3 viewPosition;
		uniform vec3 color;
	    uniform float transparency;

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

		    // Specular Light
			float specularStrength = 0.2;
			vec3 viewDir = normalize(viewPosition - FragPos);
			vec3 reflectDir = reflect(-lightDir, norm);    
			float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
			vec3 specular = specularStrength * spec * lightColor;
			
		    // Calculate the overall color
			vec3 result = (ambient + diffuse + specular) * color.xyz;     
			outputColor = vec4(result, transparency);    
			}
		";

		#endregion
	}
}
