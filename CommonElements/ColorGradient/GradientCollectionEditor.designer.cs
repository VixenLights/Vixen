using System.Windows.Forms;
namespace CommonElements.ColorManagement.Gradients
{
	partial class GradientCollectionEditor
	{
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GradientCollectionEditor));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.edit = new GradientEditPanel();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.lstPresets = new ControlsEx.ListControls.VTableDisplayList();
			this.ctxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mnuDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.label7 = new System.Windows.Forms.Label();
			this.tbNewPreset = new System.Windows.Forms.TextBox();
			this.btnNewPreset = new System.Windows.Forms.Button();
			this.btnLoad = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.sDialog = new System.Windows.Forms.SaveFileDialog();
			this.oDialog = new System.Windows.Forms.OpenFileDialog();
			this.groupBox1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.ctxMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.AccessibleDescription = null;
			this.groupBox1.AccessibleName = null;
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.BackgroundImage = null;
			this.groupBox1.Controls.Add(this.edit);
			this.groupBox1.Font = null;
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// edit
			// 
			this.edit.AccessibleDescription = null;
			this.edit.AccessibleName = null;
			resources.ApplyResources(this.edit, "edit");
			this.edit.BackgroundImage = null;
			this.edit.Font = null;
			this.edit.MinimumSize = new System.Drawing.Size(357, 154);
			this.edit.Name = "edit";
			this.edit.GradientChanged += new System.EventHandler(this.edit_GradientChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.AccessibleDescription = null;
			this.groupBox3.AccessibleName = null;
			resources.ApplyResources(this.groupBox3, "groupBox3");
			this.groupBox3.BackgroundImage = null;
			this.groupBox3.Controls.Add(this.lstPresets);
			this.groupBox3.Font = null;
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.TabStop = false;
			// 
			// lstPresets
			// 
			this.lstPresets.AccessibleDescription = null;
			this.lstPresets.AccessibleName = null;
			resources.ApplyResources(this.lstPresets, "lstPresets");
			this.lstPresets.BackgroundImage = null;
			this.lstPresets.ContextMenuStrip = this.ctxMenu;
			this.lstPresets.FieldSize = new System.Drawing.Size(48, 64);
			this.lstPresets.Font = null;
			this.lstPresets.Name = "lstPresets";
			this.lstPresets.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;
			this.lstPresets.SelectionChanged += new System.EventHandler(this.lstPresets_SelectionChanged);
			// 
			// ctxMenu
			// 
			this.ctxMenu.AccessibleDescription = null;
			this.ctxMenu.AccessibleName = null;
			resources.ApplyResources(this.ctxMenu, "ctxMenu");
			this.ctxMenu.BackgroundImage = null;
			this.ctxMenu.Font = null;
			this.ctxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDelete});
			this.ctxMenu.Name = "contextMenuStrip1";
			// 
			// mnuDelete
			// 
			this.mnuDelete.AccessibleDescription = null;
			this.mnuDelete.AccessibleName = null;
			resources.ApplyResources(this.mnuDelete, "mnuDelete");
			this.mnuDelete.BackgroundImage = null;
			this.mnuDelete.Name = "mnuDelete";
			this.mnuDelete.ShortcutKeyDisplayString = null;
			this.mnuDelete.Click += new System.EventHandler(this.mnuDelete_Click);
			// 
			// label7
			// 
			this.label7.AccessibleDescription = null;
			this.label7.AccessibleName = null;
			resources.ApplyResources(this.label7, "label7");
			this.label7.Font = null;
			this.label7.Name = "label7";
			// 
			// tbNewPreset
			// 
			this.tbNewPreset.AcceptsReturn = true;
			this.tbNewPreset.AccessibleDescription = null;
			this.tbNewPreset.AccessibleName = null;
			resources.ApplyResources(this.tbNewPreset, "tbNewPreset");
			this.tbNewPreset.BackgroundImage = null;
			this.tbNewPreset.Font = null;
			this.tbNewPreset.Name = "tbNewPreset";
			this.tbNewPreset.TextChanged += new System.EventHandler(this.tbNewPreset_TextChanged);
			// 
			// btnNewPreset
			// 
			this.btnNewPreset.AccessibleDescription = null;
			this.btnNewPreset.AccessibleName = null;
			resources.ApplyResources(this.btnNewPreset, "btnNewPreset");
			this.btnNewPreset.BackgroundImage = null;
			this.btnNewPreset.Font = null;
			this.btnNewPreset.Name = "btnNewPreset";
			this.btnNewPreset.UseVisualStyleBackColor = true;
			this.btnNewPreset.Click += new System.EventHandler(this.btnNewPreset_Click);
			// 
			// btnLoad
			// 
			this.btnLoad.AccessibleDescription = null;
			this.btnLoad.AccessibleName = null;
			resources.ApplyResources(this.btnLoad, "btnLoad");
			this.btnLoad.BackgroundImage = null;
			this.btnLoad.Font = null;
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.UseVisualStyleBackColor = true;
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			// 
			// btnSave
			// 
			this.btnSave.AccessibleDescription = null;
			this.btnSave.AccessibleName = null;
			resources.ApplyResources(this.btnSave, "btnSave");
			this.btnSave.BackgroundImage = null;
			this.btnSave.Font = null;
			this.btnSave.Name = "btnSave";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.AccessibleDescription = null;
			this.btnCancel.AccessibleName = null;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.BackgroundImage = null;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Font = null;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnOK
			// 
			this.btnOK.AccessibleDescription = null;
			this.btnOK.AccessibleName = null;
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.BackgroundImage = null;
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Font = null;
			this.btnOK.Name = "btnOK";
			this.btnOK.UseVisualStyleBackColor = true;
			// 
			// sDialog
			// 
			resources.ApplyResources(this.sDialog, "sDialog");
			// 
			// oDialog
			// 
			resources.ApplyResources(this.oDialog, "oDialog");
			// 
			// GradientCollectionEditor
			// 
			this.AcceptButton = this.btnOK;
			this.AccessibleDescription = null;
			this.AccessibleName = null;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = null;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.tbNewPreset);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnLoad);
			this.Controls.Add(this.btnNewPreset);
			this.Font = null;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = null;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GradientCollectionEditor";
			this.ShowInTaskbar = false;
			this.groupBox1.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.ctxMenu.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private GroupBox groupBox1;
		private GroupBox groupBox3;
		private ControlsEx.ListControls.VTableDisplayList lstPresets;
		private Label label7;
		private TextBox tbNewPreset;
		private Button btnNewPreset;
		private Button btnLoad;
		private Button btnSave;
		private Button btnCancel;
		private Button btnOK;
		private CommonElements.ColorManagement.Gradients.GradientEditPanel edit;
		private ContextMenuStrip ctxMenu;
		private ToolStripMenuItem mnuDelete;
		private SaveFileDialog sDialog;
		private OpenFileDialog oDialog;


	}
}

