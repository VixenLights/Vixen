using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using NGit;
using NGit.Api;
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Treewalk;
using Sharpen;
using Vixen.Sys;


namespace VersionControl
{
	public partial class Versioning : BaseForm
    {
        Data _data;

        public Data VersionControlData
        {
            get { return _data; }
            set
            {
                _data = value;
            }
        }


        Repository _repo;

        public Versioning(Data data, Repository repo)
        {
            InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
            VersionControlData = data;
            _repo = repo;
			
			Shown += Versioning_Shown;
            LoadFileStructure();
        }

		private async void Versioning_Shown(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;
			await LoadFileStructure();
			Cursor = Cursors.Arrow;
		}

		private async Task LoadFileStructure()
		{
			Iterable<RevCommit> log = null;
			listViewRestorePoints.Items.Clear();

			await Task.Run(() =>
			{
				Repository repository = new FileRepository(_repo.Directory);
				String treeName = "refs/heads/master"; // tag or branch
				Git git = new Git(_repo);
				log = git.Log().Add(repository.Resolve(treeName)).Call();
			});

			if (log != null)
			{
				foreach (RevCommit revCommit in log)
				{
					listViewRestorePoints.Items.Add(CreateListItem(revCommit));
				}

				listViewRestorePoints.ColumnAutoSize();
				listViewRestorePoints.SetLastColumnWidth();
			}
			
        }

	    private ListViewItem CreateListItem(RevCommit c)
	    {
			var commitTime = c.GetAuthorIdent().GetWhen().ToLocalTime();
		    ListViewItem item = new ListViewItem($"{commitTime.ToShortDateString()} {commitTime.ToShortTimeString()}");
			item.SubItems.Add(c.GetFullMessage());
		    item.Tag = c;
			
		    return item;
	    } 
        
        private void btnRestore_Click(object sender, EventArgs e)
        {
			if(listViewRestorePoints.SelectedItems.Count != 1) return;
			//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
			MessageBoxForm.msgIcon = SystemIcons.Hand; //this is used if you want to add a system icon to the message form.
			var messageBox = new MessageBoxForm("Are you sure you want to restore this version?  \n\rIf you have not backed up the current profile, all changes will be lost!", "Restore?", true, false);
			messageBox.ShowDialog();
			if (messageBox.DialogResult == DialogResult.OK)
			{
				Cursor = Cursors.WaitCursor;
				RevCommit commit = listViewRestorePoints.SelectedItems[0].Tag as RevCommit;
	            Git git = new Git(_repo);
				git.Reset().SetMode(ResetCommand.ResetType.HARD).SetRef(commit.Name).Call();
				VixenSystem.ReloadSystemConfig();
				Cursor = Cursors.Arrow;
				Close();
			}

        }

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}

		private void listViewRestorePoints_SelectedIndexChanged(object sender, EventArgs e)
		{
			btnRestore.Enabled = listViewRestorePoints.SelectedItems.Count == 1;
		}
	}
}
