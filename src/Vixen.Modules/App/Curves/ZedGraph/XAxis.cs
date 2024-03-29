//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright � 2004  John Champion
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//=============================================================================

using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ZedGraph
{
	/// <summary>
	/// <see cref="XAxis"/> inherits from <see cref="Axis"/>, and defines the
	/// special characteristics of a horizontal axis, specifically located at
	/// the bottom of the <see cref="Chart.Rect"/> of the <see cref="GraphPane"/>
	/// object
	/// </summary>
	/// 
	/// <author> John Champion </author>
	/// <version> $Revision: 3.16 $ $Date: 2007-04-16 00:03:02 $ </version>
	[Serializable]
	public class XAxis : Axis, ICloneable, ISerializable
	{
		#region Defaults

		/// <summary>
		/// A simple struct that defines the
		/// default property values for the <see cref="XAxis"/> class.
		/// </summary>
		public new struct Default
		{
			// Default X Axis properties
			/// <summary>
			/// The default display mode for the <see cref="XAxis"/>
			/// (<see cref="Axis.IsVisible"/> property). true to display the scale
			/// values, title, tic marks, false to hide the axis entirely.
			/// </summary>
			public static bool IsVisible = true;

			/// <summary>
			/// Determines if a line will be drawn at the zero value for the 
			/// <see cref="XAxis"/>, that is, a line that
			/// divides the negative values from positive values.
			/// <seealso cref="MajorGrid.IsZeroLine"/>.
			/// </summary>
			public static bool IsZeroLine = false;
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Default constructor that sets all <see cref="XAxis"/> properties to
		/// default values as defined in the <see cref="Default"/> class
		/// </summary>
		public XAxis()
			: this("X Axis")
		{
		}

		/// <summary>
		/// Default constructor that sets all <see cref="XAxis"/> properties to
		/// default values as defined in the <see cref="Default"/> class, except
		/// for the axis title
		/// </summary>
		/// <param name="title">The <see cref="Axis.Title"/> for this axis</param>
		public XAxis(string title)
			: base(title)
		{
			_isVisible = Default.IsVisible;
			_majorGrid._isZeroLine = Default.IsZeroLine;
			_scale._fontSpec.Angle = 0F;
		}

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="rhs">The XAxis object from which to copy</param>
		public XAxis(XAxis rhs)
			: base(rhs)
		{
		}

		/// <summary>
		/// Implement the <see cref="ICloneable" /> interface in a typesafe manner by just
		/// calling the typed version of <see cref="Clone" />
		/// </summary>
		/// <returns>A deep copy of this object</returns>
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		/// <summary>
		/// Typesafe, deep-copy clone method.
		/// </summary>
		/// <returns>A new, independent copy of this class</returns>
		public XAxis Clone()
		{
			return new XAxis(this);
		}

		#endregion

		#region Serialization

		/// <summary>
		/// Current schema value that defines the version of the serialized file
		/// </summary>
		public const int schema2 = 10;

		/// <summary>
		/// Constructor for deserializing objects
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
		/// </param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
		/// </param>
		protected XAxis(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32("schema2");
		}

		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("schema2", schema2);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Setup the Transform Matrix to handle drawing of this <see cref="XAxis"/>
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="pane">
		/// A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		public override void SetTransformMatrix(Graphics g, GraphPane pane, float scaleFactor)
		{
			// Move the origin to the BottomLeft of the ChartRect, which is the left
			// side of the X axis (facing from the label side)
			g.TranslateTransform(pane.Chart._rect.Left, pane.Chart._rect.Bottom);
		}

		/// <summary>
		/// Determines if this <see cref="Axis" /> object is a "primary" one.
		/// </summary>
		/// <remarks>
		/// The primary axes are the <see cref="XAxis" /> (always), the first
		/// <see cref="YAxis" /> in the <see cref="GraphPane.YAxisList" /> 
		/// (<see cref="CurveItem.YAxisIndex" /> = 0),  and the first
		/// <see cref="Y2Axis" /> in the <see cref="GraphPane.Y2AxisList" /> 
		/// (<see cref="CurveItem.YAxisIndex" /> = 0).  Note that
		/// <see cref="GraphPane.YAxis" /> and <see cref="GraphPane.Y2Axis" />
		/// always reference the primary axes.
		/// </remarks>
		/// <param name="pane">
		/// A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <returns>true for a primary <see cref="Axis" /> (for the <see cref="XAxis" />,
		/// this is always true), false otherwise</returns>
		internal override bool IsPrimary(GraphPane pane)
		{
			return this == pane.XAxis;
		}

		/// <summary>
		/// Calculate the "shift" size, in pixels, in order to shift the axis from its default
		/// location to the value specified by <see cref="Axis.Cross"/>.
		/// </summary>
		/// <param name="pane">
		/// A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <returns>The shift amount measured in pixels</returns>
		internal override float CalcCrossShift(GraphPane pane)
		{
			double effCross = EffectiveCrossValue(pane);

			if (!_crossAuto)
				return pane.YAxis.Scale.Transform(effCross) - pane.YAxis.Scale._maxPix;
			else
				return 0;
		}

		/*
				override internal bool IsCrossed( GraphPane pane )
				{
					return !this.crossAuto && this.cross > pane.YAxis.Min && this.cross < pane.YAxis.Max;
				}
		*/

		/// <summary>
		/// Gets the "Cross" axis that corresponds to this axis.
		/// </summary>
		/// <remarks>
		/// The cross axis is the axis which determines the of this Axis when the
		/// <see cref="Axis.Cross" >Axis.Cross</see> property is used.  The
		/// cross axis for any <see cref="XAxis" /> or <see cref="X2Axis" />
		/// is always the primary <see cref="YAxis" />, and
		/// the cross axis for any <see cref="YAxis" /> or <see cref="Y2Axis" /> is
		/// always the primary <see cref="XAxis" />.
		/// </remarks>
		/// <param name="pane">
		/// A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		public override Axis GetCrossAxis(GraphPane pane)
		{
			return pane.YAxis;
		}

//		override internal float GetMinPix( GraphPane pane )
//		{
//			return pane.Chart._rect.Left;
//		}

		#endregion
	}
}