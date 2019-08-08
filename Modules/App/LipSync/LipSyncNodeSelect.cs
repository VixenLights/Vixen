using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Sys;

namespace VixenModules.App.LipSyncApp
{
	public partial class LipSyncNodeSelect : BaseForm
	{
		private bool _userAdd;
		private bool _stringAreRows;
		private List<String> _selectedNodeNames;
		private bool _matrixOptsOnly;

		public LipSyncNodeSelect()
		{
			Location = ActiveForm != null ? new Point(ActiveForm.Location.X + 50, ActiveForm.Location.Y + 50) : new Point(400, 200);
			InitializeComponent();
			FormBorderStyle = FormBorderStyle.FixedDialog;
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
			Changed = false;
			_userAdd = false;
			_matrixOptsOnly = false;
			allowGroupsCheckbox.Checked = false;
			recurseCB.Checked = true;
		}

		public int MaxNodes { get; set; }
		public bool AllowGroups
		{
			get
			{
				return allowGroupsCheckbox.Checked;
			}

			set
			{
				allowGroupsCheckbox.Checked = value;
			}
		}

		public bool AllowRecursiveAdd
		{
			get
			{
				return recurseCB.Checked;
			}

			set
			{
				recurseCB.Checked = value;
			}
		}

		public bool StringsAreRows
		{
			get
			{
				return rowsRadioButton.Checked;
			}

			set
			{
				_stringAreRows = value;
				rowsRadioButton.Checked = value;
				colsRadioButton.Checked = !value;
			}
		}

		public bool MatrixOptionsOnly
		{
			get
			{
				return _matrixOptsOnly;
			}

			set
			{
				_matrixOptsOnly = value;
				stringsGroupBox.Visible = _matrixOptsOnly;
				rowsRadioButton.Visible = _matrixOptsOnly;
				colsRadioButton.Visible = _matrixOptsOnly;
				allowGroupsCheckbox.Checked = false;
				allowGroupsCheckbox.Visible = !_matrixOptsOnly;
			}
		}

		public bool Changed { get; set; }
		
		private void BuildNode(TreeNode parentNode, IElementNode node)
		{
			foreach(IElementNode childNode in node.Children)
			{
				TreeNode newNode = new TreeNode(childNode.Name);
				BuildNode(newNode, childNode);
				parentNode.Nodes.Add(newNode);
			}
		}

		private void LipSyncNodeSelect_Load(object sender, EventArgs e)
		{
			foreach (IElementNode node in VixenSystem.Nodes.GetRootNodes())
			{
				TreeNode newNode = new TreeNode(node.Name);
				BuildNode(newNode, node);
				nodeTreeView.Nodes.Add(newNode);

			}
			

		}

		public List<IElementNode> SelectedElementNodes
		{
			get
			{
				List<IElementNode> retVal = new List<IElementNode>();
				foreach (IElementNode element in chosenTargets.Items)
				{
					retVal.Add(element);
				}
				return retVal;
			}
		}

		public List<string> SelectedNodeNames
		{
			get
			{
				List<string> retVal = new List<string>();
				foreach (IElementNode element in chosenTargets.Items)
				{
					retVal.Add(element.Name);
				}
				return retVal;
			}

			set
			{
				List<string> names = value;
				_selectedNodeNames = value;
				if (names != null)
				{
					names.ForEach(x => findAndAddElements(x, false));
				}
			}
		}

		private void buttonReset_Click(object sender, EventArgs e)
		{
			_selectedNodeNames?.Clear();
			chosenTargets.Items.Clear();
			Changed = true;
		}

		private void addToChosenTargets(IElementNode node)
		{
			bool found = false;
			foreach (IElementNode chosenNode in chosenTargets.Items)
			{
				if (chosenNode.ToString().Equals(node.ToString()))
				{
					found = true;
					break;
				}
			}

			if ((found == false) && (chosenTargets.Items.Count < MaxNodes))
			{
				chosenTargets.Items.Add(node);
			}
			Changed = true;
		}

		private void addElementNodes(IElementNode node, bool recurse)
		{
			if ((allowGroupsCheckbox.Checked == true) || (node.IsLeaf == true))
			{
				addToChosenTargets(node);
			}

			if (recurse == true)
			{
				foreach (IElementNode childNode in node.Children)
				{
					addElementNodes(childNode, recurse);
				}
			}
		}

		private void findAndAddElements(string name, bool recurse)
		{
			foreach (IElementNode node in VixenSystem.Nodes)
			{
				if (node.Name.Equals(name))
				{
					addElementNodes(node,recurse);
				}
			}
		}

		private void buttonAdd_Click(object sender, EventArgs e)
		{
			foreach (TreeNode treeNode in nodeTreeView.SelectedNodes)
			{
				findAndAddElements(treeNode.Text, recurseCB.Checked);
			}
			_userAdd = true;
		}

		private void buttonRemove_Click(object sender, EventArgs e)
		{
			for (int i = chosenTargets.SelectedIndices.Count - 1; i >= 0; i--)
			{
				chosenTargets.Items.RemoveAt(chosenTargets.SelectedIndices[i]);
				Changed = true;
			}
			_userAdd = true;
		}

		private void allowGroupsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			if (_userAdd == false)
			{
				chosenTargets.Items.Clear();
				_selectedNodeNames?.ForEach(x => findAndAddElements(x, false));
				Changed = true;
			}

		}

		private void LipSyncNodeSelect_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_stringAreRows != StringsAreRows)
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Changing Matrix Orientation will modify existing matrix data!" +
					Environment.NewLine + "Press Cancel to keep existing matrix orientation" +
					Environment.NewLine + "Press OK to continue",
					"Warning!", false, true);
				messageBox.ShowDialog();

				if (messageBox.DialogResult == DialogResult.Cancel)
				{
					e.Cancel = true;
					StringsAreRows = _stringAreRows;
				}
			}
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
	}
}
