using System;

namespace VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Volumes
{
	/// <summary>
	/// Shader for coloring gray graphical volumes that make up the fixture.
	/// </summary>
	public class GrayVolumeShader : VolumeShader
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public GrayVolumeShader() : base(VertexShader, FragmentShader)
		{
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

			// Define the light color			
			vec3 grayColor = vec3(0.5f, 0.5f, 0.5f);    

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
	}
}
