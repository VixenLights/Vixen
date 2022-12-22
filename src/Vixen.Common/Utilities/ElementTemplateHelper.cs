using System.Drawing;
using System.Windows.Forms;
using Common.Controls;
using Vixen.Rule;
using Vixen.Sys;
using VixenModules.OutputFilter.DimmingCurve;
using VixenModules.Property.Color;

namespace Utilities
{
	/// <summary>
	/// Helper class for processing an element template.
	/// </summary>
	public class ElementTemplateHelper
	{
		#region Public Methods

		/// <summary>
		/// Processes an element template to create nodes.
		/// </summary>
		/// <param name="selectedNodes">Selected nodes</param>
		/// <param name="template">Template to apply</param>
		/// <param name="owner">Window owner</param>
		/// <param name="addToTree">Delegate for adding create node to the tree</param>
		/// <param name="treeElements">Element tree to add to</param>
		/// <returns>Collection of created nodes</returns>
		public async Task<IEnumerable<ElementNode>> ProcessElementTemplate(
			IEnumerable<ElementNode> selectedNodes, 
			IElementTemplate template, 
			IWin32Window owner,
			Action<ElementNode> addToTree,
			ElementTree treeElements)
		{
			// Default the created nodes to null
			IEnumerable<ElementNode> createdElements = null;

			// Perform (optional) setup
			bool success = template.SetupTemplate(selectedNodes);

			// If the setup was not cancelled then...
			if (success)
			{
				// Generate the nodes
				createdElements = await template.GenerateElements(selectedNodes);

				// If the user has not cancelled the template then...
				if (!template.Cancelled)
				{
					// If elements were not created then...
					if (createdElements == null || !createdElements.Any())
					{
						//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)						
						var messageBox = new MessageBoxForm("Could not create elements.\nPlease close all dialogs and try again.\n\n" +
						                                    "Please send error logs to Vixen Team.", "Error",
							MessageBoxButtons.OKCancel, SystemIcons.Error);
						messageBox.ShowDialog(owner);
						
						// Indicate to the caller that nodes were NOT created
						return null;
					}

					// If the elements created by the template require configuration of dimming curves then...
					if (template.ConfigureDimming)
					{
						var question = new MessageBoxForm("Would you like to configure a dimming curve for this Prop?",
							"Dimming Curve Setup", MessageBoxButtons.YesNo, SystemIcons.Question);
						var response = question.ShowDialog(owner);

						if (response == DialogResult.OK)
						{
							DimmingCurveHelper dimmingHelper = new DimmingCurveHelper(true);
							dimmingHelper.Perform(createdElements);
						}
					}

					// If the elements created by the template require further configuration of a color property then...
					if (template.ConfigureColor)
					{
						ColorSetupHelper helper = new ColorSetupHelper();
						helper.SetColorType(ElementColorType.FullColor);
						helper.Perform(createdElements);
					}
				}
			}

			// if the template was cancelled then...
			if (template.Cancelled)
			{
				// Indicate to the caller that no nodes were created
				createdElements = null;
			}
			// createdElements being null is another indicator that the element template was cancelled
			else if (createdElements != null)
			{
				// Call the delegate to add the node to the tree
				addToTree(createdElements.First());

				// Cleanup the tree if a group was created
				CleanupTree(treeElements, template, createdElements);				
			}

			// Return the created nodes
			return createdElements;	
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Cleanup the tree if both a group and a set of nodes were created.
		/// This method deletes the individual nodes so that only the group remains.
		/// </summary>
		/// <param name="treeElements">Tree to clean</param>
		/// <param name="template">Element template being processed</param>
		public void CleanupTree(ElementTree treeElements, IElementTemplate template, IEnumerable<ElementNode> createdElements)
		{			
			// Refresh the tree so that the delete logic below will be successful
			treeElements.PopulateNodeTree();

			// If there are elements to delete then...
			if (template.GetElementsToDelete().Count() > 0)
			{
				// Loop over the nodes created by the template that are also
				// part of a group.  Since the nodes are part of a group they
				// don't need to exist as stand alone nodes and should be removed.
				foreach (ElementNode node in template.GetElementsToDelete())
				{
					// Select the node
					treeElements.SelectElementNode(node);

					// Delete the tree node
					foreach (TreeNode tn in treeElements.SelectedTreeNodes)
					{
						treeElements.DeleteNode(tn);
					}
				}

				// Refresh the tree
				treeElements.PopulateNodeTree();

				// Since nodes are being deleted it means the template created them within a group
				// The caller will only associate a node with the shape if a node is selected in the tree
				treeElements.SelectElementNode(createdElements.First().Children.First());
			}
		}

		#endregion
	}
}
