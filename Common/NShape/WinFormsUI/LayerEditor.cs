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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Dataweb.NShape.Controllers;


namespace Dataweb.NShape.WinFormsUI {

	/// <summary>
	/// UserControl for editing layers.
	/// </summary>
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof(LayerEditor), "LayerEditor.bmp")]
	public partial class LayerEditor : UserControl {

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.LayerEditor" />.
		/// </summary>
		public LayerEditor() {
			InitializeComponent();
		}


		/// <summary>
		/// Specifies the version of the assembly containing the component.
		/// </summary>
		[Category("NShape")]
		[Browsable(true)]
		public new string ProductVersion {
			get { return base.ProductVersion; }
		}


		/// <summary>
		/// Specifies the DiagramPresenter for this layer editor.
		/// </summary>
		[Category("NShape")]
		public IDiagramPresenter DiagramPresenter {
			get { return presenter.DiagramPresenter; }
			set { presenter.DiagramPresenter = value; }
		}


		/// <summary>
		/// Specifies a DiagramSetController which provides the diagrams, this layer editor refers to.
		/// </summary>
		[Category("NShape")]
		public DiagramSetController DiagramSetController {
			get { return controller.DiagramSetController; }
			set { controller.DiagramSetController = value; }
		}


		#region Fields
		#endregion
	}
}
