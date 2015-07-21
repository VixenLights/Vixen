using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Resources.Properties;
using Vixen.Sys;

namespace VixenModules.App.LipSyncApp
{
    public partial class LipSyncNodeSelect : Form
    {
        private bool _userAdd;
        private bool _stringAreRows;

        public LipSyncNodeSelect()
        {
			Location = ActiveForm != null ? new Point(ActiveForm.Location.X + 50, ActiveForm.Location.Y + 50) : new Point(400, 200);
            InitializeComponent();
			buttonAdd.BackgroundImage = Resources.HeadingBackgroundImage;
			buttonCancel.BackgroundImage = Resources.HeadingBackgroundImage;
			buttonOk.BackgroundImage = Resources.HeadingBackgroundImage;
			buttonRemove.BackgroundImage = Resources.HeadingBackgroundImage;
			buttonReset.BackgroundImage = Resources.HeadingBackgroundImage;
			Icon = Resources.Icon_Vixen3;
            Changed = false;
            _userAdd = false;
            _matrixOptsOnly = false;
            
        }
        
        private bool _matrixOptsOnly;

        public int MaxNodes { get; set; }

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
        
        private void BuildNode(TreeNode parentNode, ElementNode node)
        {
            foreach(ElementNode childNode in node.Children)
            {
                TreeNode newNode = new TreeNode(childNode.Name);
                BuildNode(newNode, childNode);
                parentNode.Nodes.Add(newNode);
            }
        }

        private void LipSyncNodeSelect_Load(object sender, EventArgs e)
        {
            foreach (ElementNode node in VixenSystem.Nodes.GetRootNodes())
            {
                TreeNode newNode = new TreeNode(node.Name);
                BuildNode(newNode, node);
                nodeTreeView.Nodes.Add(newNode);

            }
            

        }

        private List<String> _origNodeNames;
        public List<string> NodeNames
        {
            get
            {
                List<string> retVal = new List<string>();
                foreach (ElementNode element in chosenTargets.Items)
                {
                    retVal.Add(element.Name);
                }
                return retVal;
            }

            set
            {
                List<string> names = value;
                _origNodeNames = value;
                if (names != null)
                {
                    names.ForEach(x => findAndAddElements(x, false));
                }
            }
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            _origNodeNames.Clear();
            chosenTargets.Items.Clear();
            Changed = true;
        }

        private void addToChosenTargets(ElementNode node)
        {
            bool found = false;
            foreach (ElementNode chosenNode in chosenTargets.Items)
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

        private void addElementNodes(ElementNode node, bool recurse)
        {
            if ((allowGroupsCheckbox.Checked == true) || (node.IsLeaf == true))
            {
                addToChosenTargets(node);
            }

            if (recurse == true)
            {
                foreach (ElementNode childNode in node.Children)
                {
                    addElementNodes(childNode, recurse);
                }
            }
        }

        private void findAndAddElements(string name, bool recurse)
        {
            foreach (ElementNode node in VixenSystem.Nodes)
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
                _origNodeNames.ForEach(x => findAndAddElements(x, false));
                Changed = true;
            }

        }

        private void LipSyncNodeSelect_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_stringAreRows != StringsAreRows)
            {
                DialogResult dr = 
                    MessageBox.Show("Changing Matrix Orientation will modify existing matrix data!" +
                    Environment.NewLine + "Press Cancel to keep existing matrix orientation" + 
                    Environment.NewLine + "Press OK to continue", 
                    "Warning!",  MessageBoxButtons.OKCancel);

                if (dr == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    StringsAreRows = _stringAreRows;
                }
            }
        }

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.HeadingBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.HeadingBackgroundImage;
		}

		#region Draw lines and GroupBox borders
		//set color for box borders.
		private Color _borderColor = Color.FromArgb(80, 80, 80);

		public Color BorderColor
		{
			get { return _borderColor; }
			set { _borderColor = value; }
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			//used to draw the boards and text for the groupboxes to change the default box color.
			//get the text size in groupbox
			Size tSize = TextRenderer.MeasureText((sender as GroupBox).Text, Font);

			//draw the border
			Rectangle borderRect = e.ClipRectangle;
			borderRect.Y = (borderRect.Y + (tSize.Height / 2));
			borderRect.Height = (borderRect.Height - (tSize.Height / 2));
			ControlPaint.DrawBorder(e.Graphics, borderRect, _borderColor, ButtonBorderStyle.Solid);

			//draw the text
			Rectangle textRect = e.ClipRectangle;
			textRect.X = (textRect.X + 6);
			textRect.Width = tSize.Width + 10;
			textRect.Height = tSize.Height;
			e.Graphics.FillRectangle(new SolidBrush(BackColor), textRect);
			e.Graphics.DrawString((sender as GroupBox).Text, Font, new SolidBrush(Color.FromArgb(221, 221, 221)), textRect);
		}
		#endregion
	}
}
