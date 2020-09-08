using System.Collections.Generic;

namespace VixenModules.Effect.Morph
{
	/// <summary>
	/// Maintains morph wipe polygon render data.  
	/// This data structure helps render a polygon wipe.
	/// </summary>
	public class MorphWipePolygonRenderData
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public MorphWipePolygonRenderData()
		{
			X1Points = new List<int>();
			Y1Points = new List<int>();

			X2Points = new List<int>();			
			Y2Points = new List<int>();

			HeadIsDone = new List<bool>();
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the X points that form one side of the polygon.
		/// Note these points are not part of the start or end side.
		/// </summary>
		public List<int> X1Points { get; private set; }

		/// <summary>
		/// Gets the Y points that form the other side of the polygon.
		/// Note these points are not part of the start or end side.
		public List<int> Y1Points { get; private set; }

		/// <summary>
		/// Gets the X points that form the other side of the polygon.
		/// Note these points are not part of the start or end side.
		/// </summary>
		public List<int> X2Points { get; private set; }

		/// <summary>
		/// Gets the Y points that form the other side of the polygon.
		/// Note these points are not part of the start or end side.
		/// </summary>
		public List<int> Y2Points { get; private set; }

		/// <summary>
		/// Gets or sets the direction of the polygon.  
		/// Direction is used to assist in drawing the polygon lines.
		/// </summary>
		public bool Direction { get; set; }

		/// <summary>
		/// Gets or sets the flag that indicates the head has wiped off the polygon.
		/// </summary>
		public List<bool> HeadIsDone { get; set; }

		/// <summary>
		/// Gets or sets the acceleration of the head.
		/// </summary>
		public double HeadAcceleration { get; set; }

		/// <summary>
		/// Gets or sets the acceleration of the tail.
		/// </summary>
		public double TailAcceleration { get; set; }

		/// <summary>
		/// Gets or sets the initial velocity of the head at time zero.
		/// </summary>
		public double HeadVelocityZero { get; set; }

		/// <summary>
		/// Gets or sets the initial velocity of the tail at time zero.
		/// </summary>
		public double TailVelocityZero { get; set; }

		/// <summary>
		/// Gets or sets the start offset of the wipe.
		/// </summary>
		public double Stagger { get; set; }

		#endregion
	}
}
