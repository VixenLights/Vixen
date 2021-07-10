using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Primitives
{
	/// <summary>
	/// Maintains meta-data about OpenGL types (Uniform & Attribute).
	/// </summary>
	public class MetaDataInfo
	{
		#region Public Properties

		/// <summary>
		/// Name of the uniform.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Address of the uniform.
		/// </summary>
		public int Address { get; set; }

		/// <summary>
		/// Size of the uniform.
		/// </summary>
		public int Size { get; set; }

		#endregion
	}
}
