/******************************************************************************
  Copyright 2009-2012 dataweb GmbH
  This file is part of the NShape framework.
  NShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  NShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  NShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System;
using System.ComponentModel;


namespace Dataweb.NShape.Advanced {

	#region Layers

	/// <summary>
	/// Describes the layers a shape is part of.
	/// </summary>
	[Flags]
	public enum LayerIds {
		/// <summary>No Layers.</summary>
		None = 0x0,
		/// <summary>Layer 1</summary>
		Layer01 = 0x00000001,
		/// <summary>Layer 2</summary>
		Layer02 = 0x00000002,
		/// <summary>Layer 3</summary>
		Layer03 = 0x00000004,
		/// <summary>Layer 4</summary>
		Layer04 = 0x00000008,
		/// <summary>Layer 5</summary>
		Layer05 = 0x00000010,
		/// <summary>Layer 6</summary>
		Layer06 = 0x00000020,
		/// <summary>Layer 7</summary>
		Layer07 = 0x00000040,
		/// <summary>Layer 8</summary>
		Layer08 = 0x00000080,
		/// <summary>Layer 9</summary>
		Layer09 = 0x00000100,
		/// <summary>Layer 10</summary>
		Layer10 = 0x00000200,
		/// <summary>Layer 11</summary>
		Layer11 = 0x00000400,
		/// <summary>Layer 12</summary>
		Layer12 = 0x00000800,
		/// <summary>Layer 13</summary>
		Layer13 = 0x00001000,
		/// <summary>Layer 14</summary>
		Layer14 = 0x00002000,
		/// <summary>Layer 15</summary>
		Layer15 = 0x00004000,
		/// <summary>Layer 16</summary>
		Layer16 = 0x00008000,
		/// <summary>Layer 17</summary>
		Layer17 = 0x00010000,
		/// <summary>Layer 18</summary>
		Layer18 = 0x00020000,
		/// <summary>Layer 19</summary>
		Layer19 = 0x00040000,
		/// <summary>Layer 20</summary>
		Layer20 = 0x00080000,
		/// <summary>Layer 21</summary>
		Layer21 = 0x00100000,
		/// <summary>Layer 22</summary>
		Layer22 = 0x00200000,
		/// <summary>Layer 23</summary>
		Layer23 = 0x00400000,
		/// <summary>Layer 24</summary>
		Layer24 = 0x00800000,
		/// <summary>Layer 25</summary>
		Layer25 = 0x01000000,
		/// <summary>Layer 26</summary>
		Layer26 = 0x02000000,
		/// <summary>Layer 27</summary>
		Layer27 = 0x04000000,
		/// <summary>Layer 28</summary>
		Layer28 = 0x08000000,
		/// <summary>Layer 29</summary>
		Layer29 = 0x10000000,
		/// <summary>Layer 30</summary>
		Layer30 = 0x20000000,
		/// <summary>Layer 31</summary>
		Layer31 = 0x40000000,
		/// <summary>All available layers.</summary>
		All = int.MaxValue	// 0xFFFFFFFF results in a type mismatch compiler error
	}


	/// <summary>
	/// A layer used for grouoping shapes on a diagram.
	/// </summary>
	/// <status>reviewed</status>
	[TypeDescriptionProvider(typeof(TypeDescriptionProviderDg))]
	public class Layer {

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Layer" />.
		/// </summary>
		/// <param name="name"></param>
		public Layer(string name) {
			if (name == null) throw new ArgumentNullException("name");
			if (name == string.Empty) throw new ArgumentException("Parameter name must not be empty.");
			this.name = name;
		}


		/// <summary>
		/// The <see cref="T:Dataweb.NShape.LayerIds" /> value used to identify the <see cref="T:Dataweb.NShape.Layer" />.
		/// </summary>
		public LayerIds Id {
			get { return id; }
			internal set { id = value; }
		}


		/// <summary>
		/// The language independent name of the <see cref="T:Dataweb.NShape.Layer" />.
		/// </summary>
		[RequiredPermission(Permission.Data)]
		public string Name {
			get { return name; }
			internal set { name = value; }
		}


		/// <summary>
		/// The localized title of the <see cref="T:Dataweb.NShape.Layer" />.
		/// </summary>
		[RequiredPermission(Permission.Present)]
		public string Title {
			get { return string.IsNullOrEmpty(title) ? name : title; }
			set {
				if (value == name || string.IsNullOrEmpty(value))
					title = null;
				else title = value;
			}
		}


		/// <summary>
		/// Specifies the minimum zoom level where the <see cref="T:Dataweb.NShape.Layer" /> is still visible. On lower zoom levels, the layer will be hidden automatically.
		/// </summary>
		[RequiredPermission(Permission.Present)]
		public int LowerZoomThreshold {
			get { return lowerZoomThreshold; }
			set {
				if (value < 0) throw new ArgumentOutOfRangeException("LowerZoomThreshold");
				lowerZoomThreshold = value;
			}
		}


		/// <summary>
		/// Specifies the maximum zoom level where the <see cref="T:Dataweb.NShape.Layer" /> is still visible. On higher zoom levels, the layer will be hidden automatically.
		/// </summary>
		[RequiredPermission(Permission.Present)]
		public int UpperZoomThreshold {
			get { return upperZoomThreshold; }
			set {
				if (value < 0) throw new ArgumentOutOfRangeException("UpperZoomThreshold");
				upperZoomThreshold = value;
			}
		}


		#region Fields

		private LayerIds id = LayerIds.None;
		private string name = string.Empty;
		private string title = string.Empty;
		private int lowerZoomThreshold = 0;
		private int upperZoomThreshold = 5000;

		#endregion
	}

	#endregion

}
