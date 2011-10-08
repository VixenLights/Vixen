using System.Windows.Forms;
namespace VixenModules.App.ColorGradients
{
	partial class GradientCollectionSelector
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GradientCollectionSelector));
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.lstPresets = new CommonElements.ControlsEx.ListControls.VTableDisplayList();
			this.ctxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mnuDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.label7 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.sDialog = new System.Windows.Forms.SaveFileDialog();
			this.oDialog = new System.Windows.Forms.OpenFileDialog();
			this.labelName = new System.Windows.Forms.Label();
			this.groupBox3.SuspendLayout();
			this.ctxMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox3
			// 
			resources.ApplyResources(this.groupBox3, "groupBox3");
			this.groupBox3.Controls.Add(this.lstPresets);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.TabStop = false;
			// 
			// lstPresets
			// 
			resources.ApplyResources(this.lstPresets, "lstPresets");
			this.lstPresets.ContextMenuStrip = this.ctxMenu;
			this.lstPresets.FieldSize = new System.Drawing.Size(48, 64);
			this.lstPresets.Name = "lstPresets";
			this.lstPresets.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;
			this.lstPresets.SelectionChanged += new System.EventHandler(this.lstPresets_SelectionChanged);
			// 
			// ctxMenu
			// 
			this.ctxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDelete});
			this.ctxMenu.Name = "contextMenuStrip1";
			resources.ApplyResources(this.ctxMenu, "ctxMenu");
			// 
			// mnuDelete
			// 
			resources.ApplyResources(this.mnuDelete, "mnuDelete");
			this.mnuDelete.Name = "mnuDelete";
			this.mnuDelete.Click += new System.EventHandler(this.mnuDelete_Click);
			// 
			// label7
			// 
			resources.ApplyResources(this.label7, "label7");
			this.label7.Name = "label7";
			// 
			// btnCancel
			// 
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnOK
			// 
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
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
			// labelName
			// 
			resources.ApplyResources(this.labelName, "labelName");
			this.labelName.Name = "labelName";
			// 
			// GradientCollectionSelector
			// 
			this.AcceptButton = this.btnOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GradientCollectionSelector";
			this.ShowInTaskbar = false;
			this.groupBox3.ResumeLayout(false);
			this.ctxMenu.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private GroupBox groupBox3;
		private CommonElements.ControlsEx.ListControls.VTableDisplayList lstPresets;
		private Label label7;
		private Button btnCancel;
		private Button btnOK;
		private ContextMenuStrip ctxMenu;
		private ToolStripMenuItem mnuDelete;
		private SaveFileDialog sDialog;
		private OpenFileDialog oDialog;
		private Label labelName;


	}
}

