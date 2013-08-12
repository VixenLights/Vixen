using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using GitSharp;

namespace VersionControl {
	public partial class Versioning : Form {
		Data _data;
		public Data VersionControlData {
			get { return _data; }
			set { _data = value; }
		}

		List<ChangeDetails> _changeDetails;
		GitSharp.Repository _repo;

		public Versioning(Data data, GitSharp.Repository repo, List<ChangeDetails> changeDetails) {

			InitializeComponent();
			VersionControlData = data;
			_changeDetails = changeDetails;
			_repo = repo;
			LoadFileStructure();
		}

		private void LoadFileStructure() {
			//foreach (var item in _changeDetails.OrderBy(a => a.FileName).GroupBy(g => g.FileName).OrderBy(h => h.Key).ToList()) {
			//	foreach (var item2 in item.ToList()) {
			//		AddTreeNode(item2);
			//	}
			//}

			//_rootPaths.ToList().ForEach(dir => {
			//	var rootNode = CreateTreeNodeForDirectory(dir);
			//	rootNode.Expand();
			//	treeViewFiles.Nodes.Add(rootNode);
			//});

		}

		private void AddTreeNode(ChangeDetails detail) {
			var nodes = treeViewFiles.Nodes;
			var details = detail.FileName.Split('/');
			TreeNode node;
			foreach (var item in details) {

				if (nodes.ContainsKey(item)) {
					node = nodes.Find(item, false).First();
				}
				else {
					node = new TreeNode(item);
					node.Tag = detail;
					nodes.Add(node);

				}

				nodes = node.Nodes;
			}
			node = new TreeNode(detail.ChangeDate.ToString());
			node.Tag = detail;
			node.Nodes.Add(node);
		 
		}

		private static TreeNode CreateTreeNodeForDirectory(string dir) {

			TreeNode rootNode = null;
			if (Directory.Exists(dir)) {
				var di = new DirectoryInfo(dir);
				rootNode = new TreeNode(di.Name);

				if (di.Name == ".git" || di.Name == "Logs") return null;

				rootNode.Tag = dir;
				di.GetDirectories().ToList().ForEach(d => {
					var xNode = CreateTreeNodeForDirectory(d.FullName);
					if (xNode != null)
						rootNode.Nodes.Add(xNode);
				});

				foreach (var f in Directory.GetFiles(dir)) {
					var fi = new FileInfo(f);
					var fileNode = new TreeNode(fi.Name);
					fileNode.Tag = dir;
					rootNode.Nodes.Add(fileNode);
				}

			}


			return rootNode;
		}

		private void treeViewFiles_AfterSelect(object sender, TreeViewEventArgs e) {
			Console.WriteLine(e.Node.Tag.ToString());
			GetVersionControlInfo(e.Node.Tag.ToString());
		}

		private void GetVersionControlInfo(string fileName) {

		}
	}
}
