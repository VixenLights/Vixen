using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;

namespace VixenApplication.Setup
{
	interface ISetupElementsControl
	{
		event EventHandler<ElementNodesEventArgs> ElementSelectionChanged;

		// really, this SHOULD have a parameter with what elements changed; however, for now we'll make
		// it parameterless and assume everything has changed (ie. just use SelectedElements); it's a shitload easier.
		//event EventHandler<ElementNodesEventArgs> ElementsChanged;
		event EventHandler ElementsChanged;

		IEnumerable<ElementNode> SelectedElements { get; }

		Control SetupElementsControl { get; }

		void UpdatePatching();
	}

	public class ElementNodesEventArgs : EventArgs
	{
		public List<ElementNode> ElementNodes;

		public ElementNodesEventArgs(IEnumerable<ElementNode> nodes)
		{
			ElementNodes = new List<ElementNode>(nodes);
		}
	}
}
