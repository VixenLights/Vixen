using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DragDropTest1
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			this.treeView1.AllowDrop = true;
			this.listBox1.AllowDrop = true;
			this.listBox1.MouseDown += new MouseEventHandler(listBox1_MouseDown);
			this.listBox1.DragOver += new DragEventHandler(listBox1_DragOver);

			this.treeView1.DragEnter += new DragEventHandler(treeView1_DragEnter);
			this.treeView1.DragDrop += new DragEventHandler(treeView1_DragDrop);

		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.PopulateListBox();
			this.PopulateTreeView();
		}

		private void PopulateListBox()
		{
			for (int i = 0; i <= 10; i++)
			{
				this.listBox1.Items.Add(DateTime.Now.AddDays(i));
			}
		}

		private void PopulateTreeView()
		{
			for (int i = 1; i <= 2; i++)
			{
				TreeNode node = new TreeNode("Node" + i);
				for (int j = 1; j <= 2; j++)
				{
					node.Nodes.Add("SubNode" + j);
				}
				this.treeView1.Nodes.Add(node);
			}
		}

		private void treeView1_DragDrop(object sender, DragEventArgs e)
		{

			TreeNode nodeToDropIn = this.treeView1.GetNodeAt(this.treeView1.PointToClient(new Point(e.X, e.Y)));
			if (nodeToDropIn == null) { return; }
			if (nodeToDropIn.Level > 0)
			{
				nodeToDropIn = nodeToDropIn.Parent;
			}

			object data = e.Data.GetData(typeof(DateTime));
			if (data == null) { return; }
			nodeToDropIn.Nodes.Add(data.ToString());
			this.listBox1.Items.Remove(data);
		}

		private void listBox1_DragOver(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Move;
		}

		private void treeView1_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Move;
		}

		private void listBox1_MouseDown(object sender, MouseEventArgs e)
		{
			this.listBox1.DoDragDrop(this.listBox1.SelectedItem, DragDropEffects.Move);
		}


	}
}
