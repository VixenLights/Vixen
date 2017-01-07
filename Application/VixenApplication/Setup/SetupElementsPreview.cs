using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;

namespace VixenApplication.Setup
{
	public partial class SetupElementsPreview : UserControl, ISetupElementsControl
	{
		public SetupElementsPreview()
		{
			InitializeComponent();
		}

		public event EventHandler<ElementNodesEventArgs> ElementSelectionChanged;
		public event EventHandler ElementsChanged;
		IEnumerable<ElementNode> ISetupElementsControl.SelectedElements { get; set; }

		public IEnumerable<ElementNode> SelectedElements
		{
			get { throw new NotImplementedException(); }
		}

		public Control SetupElementsControl
		{
			get { return this; }
		}

		public DisplaySetup MasterForm { get; set; }

		public void UpdatePatching()
		{
			throw new NotImplementedException();
		}

		public void UpdateScrollPosition()
		{
			throw new NotImplementedException();
		}
	}
}
