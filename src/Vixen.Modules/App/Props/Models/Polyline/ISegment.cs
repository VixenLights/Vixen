using System;
using System.Collections.Generic;
using System.Text;

namespace VixenModules.App.Props.Models.Polyline
{
	/// <summary>
	/// Maintains a segment of a prop.
	/// </summary>
	public interface ISegment
	{
		/// <summary>
		/// X start point of the segment.
		/// </summary>
		float StartX { get; set; }
		
		/// <summary>
		/// Y start point of the segment.
		/// </summary>
		float StartY { get; set; }

		/// <summary>
		/// X end point of the segment.
		/// </summary>
		float EndX { get; set; }

		/// <summary>
		/// Y end point of the segment.
		/// </summary>
		float EndY { get; set; }
	}
}
