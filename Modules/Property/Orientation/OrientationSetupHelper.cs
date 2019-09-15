using System.Collections.Generic;
using System.Windows.Forms;
using Vixen.Rule;
using Vixen.Sys;

namespace VixenModules.Property.Orientation
{
	public partial class OrientationSetupHelper : IElementSetupHelper
	{
		#region Implementation of IElementSetupHelper

		/// <inheritdoc />
		public string HelperName => "Orientation";

		/// <inheritdoc />
		public bool Perform(IEnumerable<IElementNode> selectedNodes)
		{
			SetupForm form = new SetupForm(Orientation.Vertical);
			DialogResult dr = form.ShowDialog();
			if (dr != DialogResult.OK)
			{
				return false;
			}

			foreach (var selectedNode in selectedNodes)
			{
				OrientationModule om;
				if (selectedNode.Properties.Contains(OrientationDescriptor.ModuleId))
				{
					om = selectedNode.Properties.Get(OrientationDescriptor.ModuleId) as OrientationModule;
				}
				else
				{
					om = selectedNode.Properties.Add(OrientationDescriptor.ModuleId) as OrientationModule;
				}

				om.Orientation = form.Orientation;
			}
			
			return true;
		}

		#endregion
	}
}
