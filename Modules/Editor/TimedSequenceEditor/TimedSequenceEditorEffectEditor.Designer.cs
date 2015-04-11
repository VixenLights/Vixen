namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class TimedSequenceEditorEffectEditor
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimedSequenceEditorEffectEditor));
			this.tableLayoutPanelAll = new System.Windows.Forms.TableLayoutPanel();
			this.panelFormButtons = new System.Windows.Forms.Panel();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.tableLayoutPanelEffectEditors = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanelAll.SuspendLayout();
			this.panelFormButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanelAll
			// 
			this.tableLayoutPanelAll.AutoSize = true;
			this.tableLayoutPanelAll.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanelAll.ColumnCount = 1;
			this.tableLayoutPanelAll.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanelAll.Controls.Add(this.panelFormButtons, 0, 1);
			this.tableLayoutPanelAll.Controls.Add(this.tableLayoutPanelEffectEditors, 0, 0);
			this.tableLayoutPanelAll.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanelAll.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanelAll.Name = "tableLayoutPanelAll";
			this.tableLayoutPanelAll.RowCount = 2;
			this.tableLayoutPanelAll.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelAll.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
			this.tableLayoutPanelAll.Size = new System.Drawing.Size(175, 52);
			this.tableLayoutPanelAll.TabIndex = 7;
			// 
			// panelFormButtons
			// 
			this.panelFormButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.panelFormButtons.AutoSize = true;
			this.panelFormButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panelFormButtons.Controls.Add(this.buttonCancel);
			this.panelFormButtons.Controls.Add(this.buttonOK);
			this.panelFormButtons.Location = new System.Drawing.Point(7, 22);
			this.panelFormButtons.Name = "panelFormButtons";
			this.panelFormButtons.Size = new System.Drawing.Size(165, 27);
			this.panelFormButtons.TabIndex = 3;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(87, 2);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(6, 2);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 6;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanelEffectEditors
			// 
			this.tableLayoutPanelEffectEditors.AutoSize = true;
			this.tableLayoutPanelEffectEditors.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanelEffectEditors.ColumnCount = 2;
			this.tableLayoutPanelEffectEditors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanelEffectEditors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanelEffectEditors.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanelEffectEditors.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanelEffectEditors.Name = "tableLayoutPanelEffectEditors";
			this.tableLayoutPanelEffectEditors.RowCount = 1;
			this.tableLayoutPanelEffectEditors.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelEffectEditors.Size = new System.Drawing.Size(169, 1);
			this.tableLayoutPanelEffectEditors.TabIndex = 4;
			// 
			// TimedSequenceEditorEffectEditor
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(175, 52);
			this.Controls.Add(this.tableLayoutPanelAll);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TimedSequenceEditorEffectEditor";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Effect";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TimedSequenceEditorEffectEditor_FormClosed);
			this.tableLayoutPanelAll.ResumeLayout(false);
			this.tableLayoutPanelAll.PerformLayout();
			this.panelFormButtons.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelAll;
		private System.Windows.Forms.Panel panelFormButtons;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelEffectEditors;

	}
}