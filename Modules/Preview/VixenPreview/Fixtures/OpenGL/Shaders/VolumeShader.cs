using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Primitives;
using VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Volumes;
using Vector3 = OpenTK.Vector3;

namespace VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Shaders
{
	/// <summary>
	/// Maintains a shader program for a graphical volumes.
	/// </summary>
	public abstract class VolumeShader : IDisposable
	{		
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="vertexShaderSource">GLSL source for the vertex shader</param>
		/// <param name="fragmentShaderSource">GLSL source for the fragment shader</param>
		public VolumeShader(string vertexShaderSource, string fragmentShaderSource)
		{
			// Create a dictionary to store attributes by name
			Attributes = new Dictionary<string, AttributeInfo>();

			// Create a dictionary to store uniforms by name
			Uniforms = new Dictionary<string, UniformInfo>();

			// Create a dictionary to store vertex array object by name
			Buffers = new Dictionary<string, uint>();

			// Initialize the vertex array object ID to invalid
			VaoID = -1;
				
			// Create and compile the vertex shader
			VertexShaderID = GL.CreateShader(ShaderType.VertexShader);
			GL.ShaderSource(VertexShaderID, vertexShaderSource);
			GL.CompileShader(VertexShaderID);

			// Create and compile the fragment shader
			FragmentShaderID = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(FragmentShaderID, fragmentShaderSource);
			GL.CompileShader(FragmentShaderID);

			// Create the program
			ProgramID = GL.CreateProgram();

			// Attach the vertex shader to the program
			GL.AttachShader(ProgramID, VertexShaderID);
			string info = GL.GetShaderInfoLog(VertexShaderID);
			Debug.Assert(string.IsNullOrEmpty(info));
	
			// Attach the fragment shader to the program
			GL.AttachShader(ProgramID, FragmentShaderID);
			string info2 = GL.GetShaderInfoLog(FragmentShaderID);
			Debug.Assert(string.IsNullOrEmpty(info2));

			// Link the program
			Link();

			// Create the vertex array object 
			int vao = -1;
			GL.GenVertexArrays(1, out vao);			
			VaoID = vao;

			// Generate the buffers associated with the program shaders
			GenBuffers();
			
			string info3 = GL.GetProgramInfoLog(ProgramID);
			Debug.Assert(string.IsNullOrEmpty(info3));

			// Detach the shaders since they have already been compiled and linked to the program
			GL.DetachShader(ProgramID, VertexShaderID);
			GL.DetachShader(ProgramID, FragmentShaderID);
			
			// Delete te shaders since they have already been compiled and linked to the program
			GL.DeleteShader(FragmentShaderID);
			GL.DeleteShader(VertexShaderID);			
		}

		#endregion

		#region Private Properties

		/// <summary>
		/// Gets or sets the ID of the Shader Program.
		/// </summary>
		private int ProgramID { get; set; }

		/// <summary>
		/// Gets or sets the ID of the Vertex Shader.
		/// </summary>
		private int VertexShaderID { get; set; }

		/// <summary>
		/// Gets or sets the ID of the Fragment Shader.
		/// </summary>
		private int FragmentShaderID { get; set; }

		/// <summary>
		/// Vertex array object ID associated with this shader program.
		/// </summary>
		public int VaoID { get; set; }

		#endregion
		
		#region Fields

		/// <summary>
		/// Dictionary of OpenGL attributes name.
		/// </summary>
		private readonly Dictionary<String, AttributeInfo> Attributes;
		
		/// <summary>
		/// Dictionary of OpenGL uniforms by name.
		/// </summary>
		private readonly Dictionary<String, UniformInfo> Uniforms;
		
		/// <summary>
		/// Dictionary of OpenGL buffer addresses by name.
		/// </summary>
		private readonly Dictionary<String, uint> Buffers;

		/// <summary>
		/// Flag indicating if the resources have been disposed.
		/// </summary>
		private bool _disposed;

		#endregion

		#region Private Methods

		/// <summary>
		/// Links the shader program.
		/// </summary>
		private void Link()
		{
			// Link the shader program
			GL.LinkProgram(ProgramID);

			// Check the info log for problems
			string infoLog = GL.GetProgramInfoLog(ProgramID);
			Debug.Assert(string.IsNullOrEmpty(infoLog));

			// Gets the number of attributes associated with the shader program
			int attributeCount;
			GL.GetProgram(ProgramID, ProgramParameter.ActiveAttributes, out attributeCount);
			
			// Gets the number of uniforms associated with the shader program 
			int uniformCount;
			GL.GetProgram(ProgramID, ProgramParameter.ActiveUniforms, out uniformCount);
						
			// Loop over the discovered attributes
			for (int i = 0; i < attributeCount; i++)
			{
				// Retrieve information about the attribute
				AttributeInfo info = new AttributeInfo();
				string name;
				int length = 0;
				int size;
				ActiveAttribType attributeType;
				GL.GetActiveAttrib(ProgramID, i, 256, out length, out size, out attributeType, out name);

				// Store off the attribute size
				info.Size = size;

				// Store off the attribute type
				info.AttribType = attributeType;

				// Store off the attribute's name, address and information in the dictionary				
				info.Name = name;
				info.Address = GL.GetAttribLocation(ProgramID, info.Name);
				Attributes.Add(name.ToString(), info);
			}

			// Loop over the discovered uniforms
			for (int i = 0; i < uniformCount; i++)
			{
				// Retrieve information about the uniform
				UniformInfo info = new UniformInfo();
				int length = 0;				
				string name;
				int size;
				ActiveUniformType uniformType;
				GL.GetActiveUniform(ProgramID, i, 256, out length, out size, out uniformType, out name);
				
				// Store off the uniform type
				info.UniformType = uniformType;

				// Store off the uniform size
				info.Size = size;

				// Store off the uniform's name, address and information in the dictionary
				info.Name = name.ToString();
				Uniforms.Add(name.ToString(), info);
				info.Address = GL.GetUniformLocation(ProgramID, info.Name);
			}
		}

		/// <summary>
		/// Generate IDs for the vertex arrays and uniforms.
		/// </summary>
		private void GenBuffers()
		{
			// Loop over the attributes associated with the shader
			for (int i = 0; i < Attributes.Count; i++)
			{
				// Get an unused buffer ID
				uint buffer = 0;
				GL.GenBuffers(1, out buffer);

				// Store off the ID in the dictionary
				Buffers.Add(Attributes.Values.ElementAt(i).Name, buffer);
			}

			// Loop over the uniforms associated with the shader
			for (int i = 0; i < Uniforms.Count; i++)
			{
				// Get an unused buffer ID
				uint buffer = 0;
				GL.GenBuffers(1, out buffer);

				// Store off the ID in the dictionary
				Buffers.Add(Uniforms.Values.ElementAt(i).Name, buffer);
			}
		}

		/// <summary>
		/// Gets the address of the attribute with the specified name.
		/// </summary>
		/// <param name="name">Name of the attribute</param>
		/// <returns>Address of the attribute</returns>
		private int GetAttribute(string name)
		{
			Debug.Assert(Attributes.ContainsKey(name));
			return Attributes[name].Address;			
		}
		
		/// <summary>
		/// Gets the address of the buffer with the specified name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private uint GetBuffer(string name)
		{
			Debug.Assert(Buffers.ContainsKey(name));
			return Buffers[name];			
		}

		/// <summary>
		/// Transfers the specified vertex data to the GPU.
		/// </summary>
		/// <param name="vertexData">Vertex data to transfer</param>
		private void TransferVertexBuffer(Vector3[] vertexData)
		{
			GL.BindBuffer(BufferTarget.ArrayBuffer, GetBuffer("vPosition"));
			GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(vertexData.Length * Vector3.SizeInBytes), vertexData, BufferUsageHint.StaticDraw);
			GL.VertexAttribPointer(GetAttribute("vPosition"), 3, VertexAttribPointerType.Float, false, 0, 0);
			GL.EnableVertexAttribArray(GetAttribute("vPosition"));
		}

		/// <summary>
		/// Transfers the specified vertex normal data to the GPU.
		/// </summary>
		/// <param name="normalData">Vertex normal data to transfer</param>
		private void TransferNormalBuffer(Vector3[] normalData)
		{
			GL.BindBuffer(BufferTarget.ArrayBuffer, GetBuffer("vNormal"));
			GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(normalData.Length * Vector3.SizeInBytes), normalData, BufferUsageHint.StaticDraw);
			GL.VertexAttribPointer(GetAttribute("vNormal"), 3, VertexAttribPointerType.Float, true, 0, 0);
			GL.EnableVertexAttribArray(GetAttribute("vNormal"));
		}
		
		#endregion

		#region IVolumeShader

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public void Use()
		{
			// Indicate to the GPU to use the program associated with this shader
			GL.UseProgram(ProgramID);

			// Bind the vertex array
			GL.BindVertexArray(VaoID);
		}

		/// <summary>
		/// Transfers the vertex and normal data associated with the specified graphical volumes to the GPU.
		/// </summary>
		/// <param name="volumes">Volumes to transfer vertex data to the GPU</param>
		public void TransferBuffers(IEnumerable<IVolume> volumes)
		{
			// Create a collection to hold all the vertex data for the volumes
			List<Vector3> vertices = new List<Vector3>();

			// Create a collection to hold all the vertex normal data for the volumes
			List<Vector3> normals = new List<Vector3>();

			// Loop over the graphical volumes
			foreach (IVolume v in volumes)
			{
				// Add the vertex data from the volume to the overall collection
				vertices.AddRange(v.GetVertices().ToList());

				// Add the vertex normal data from the volume to the overall collection
				normals.AddRange(v.GetNormals().ToList());
			}

			// Transfer the vertex data to the GPU			
			TransferVertexBuffer(vertices.ToArray());

			// Transfer the vertex normal data to the GPU
			TransferNormalBuffer(normals.ToArray());
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public void TransferUniformsAndDraw(IEnumerable<IVolume> volumes, Vector3? lightPosition, Matrix4? projectionMatrix, Matrix4? viewMatrix)
		{
			// Declare an index for where we are in the vertex array object
			int index = 0;

			// Transfer global uniforms that only need to sent to the GPU when they change
			TransferGlobalUniforms(lightPosition, projectionMatrix, viewMatrix);

			// Loop over the volumes associated with this shader
			foreach (IVolume volume in volumes)
			{
				// If the volume is visible then...
				if (volume.Visible)
				{
					// Transfer the uniforms associated with the volume
					TransferUniforms(volume);

					// Draw the graphical volume
					GL.DrawArrays(BeginMode.Triangles, index, volume.GetVertices().Length);
				}

				// Move the index to the next volume
				index += volume.GetVertices().Length;
			}
		}

		#endregion

		#region IDisposable

		/// <summary>
		/// Refer to MSDN documentation.
		/// </summary>		
		public void Dispose()
		{
			// If the class has not been disposed then...
			if (!_disposed)
			{
				// Delete the OpenGL shader program
				GL.DeleteProgram(ProgramID);

				// Remember that the class has been disposed
				_disposed = true;
			}
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Gets the address of the uniform with the specified name.
		/// </summary>
		/// <param name="name">Name of the uniform</param>
		/// <returns>Address of the uniform</returns>
		protected int GetUniform(string name)
		{
			Debug.Assert(Uniforms.ContainsKey(name));
			return Uniforms[name].Address;
		}

		/// <summary>
		/// Transfers global uniforms that are shared by multiple graphical volumes.
		/// </summary>
		/// <param name="lightPosition">Position of the light source</param>
		/// <param name="projectionMatrix">Projection matrix used to draw</param>
		/// <param name="viewMatrix">View matrix used to draw</param>
		protected virtual void TransferGlobalUniforms(
			Vector3? lightPosition,
			Matrix4? projectionMatrix,
			Matrix4? viewMatrix)
		{
			// If the view matrix has changed then...
			if (viewMatrix.HasValue)
			{
				// Transfer the view matrix of the volume
				Matrix4 localViewMatrix = viewMatrix.Value;
				GL.UniformMatrix4(GetUniform("viewMatrix"), false, ref localViewMatrix);
			}

			// If the projection matrix has changed then...
			if (projectionMatrix.HasValue)
			{
				// Transfer the projection matrix of the volume
				Matrix4 localProjectionMatrix = projectionMatrix.Value;
				GL.UniformMatrix4(GetUniform("projectionMatrix"), false, ref localProjectionMatrix);
			}

			// If the light position has changed then...
			if (lightPosition.HasValue)
			{
				// Transfer the position of the light source
				GL.Uniform3(GetUniform("lightPosition"), lightPosition.Value);

				// Transfer the position of the camera
				GL.Uniform3(GetUniform("viewPosition"), lightPosition.Value);
			}
		}

		/// <summary>
		/// Transfers the uniforms associated with the specified graphical volume to the GPU.
		/// </summary>
		/// <param name="volume">Volume to process</param>		
		protected virtual void TransferUniforms(IVolume volume)					
		{
			// Transfer the model matrix of the volume
			Matrix4 modelMatrix = volume.ModelMatrix;
			GL.UniformMatrix4(GetUniform("modelMatrix"), false, ref modelMatrix);

			// Transfer the inverse tranposed model matrix for normal processing
			Matrix4 inverseTransposeModelMatrix = modelMatrix;
			inverseTransposeModelMatrix.Invert();
			inverseTransposeModelMatrix.Transpose();
			GL.UniformMatrix4(GetUniform("inverseTransposedModelMatrix"), false, ref inverseTransposeModelMatrix);
		}

		#endregion

		#region Protected Static Field
		
		/// <summary>
		/// GLSL source for the vertex shader.
		/// </summary>
		protected static string VertexShader = @"
		#version 330
 
		in vec3 vPosition;
		in vec3 vNormal;

		out vec3 v_norm;
		out vec3 FragPos;

		uniform mat4 modelMatrix;
		uniform mat4 viewMatrix;
		uniform mat4 projectionMatrix;
		uniform mat4 inverseTransposedModelMatrix;
		 
		void main()
		{
		    gl_Position = projectionMatrix * viewMatrix *modelMatrix * vec4(vPosition, 1.0);
		    FragPos = vec4(modelMatrix * vec4(vPosition, 1.0)).xyz;		    
			v_norm = mat3(inverseTransposedModelMatrix) * vNormal; 
		}
		";
		#endregion
	}
}
