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
			this.tableLayoutPanelAll = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanelParameterEditors = new System.Windows.Forms.TableLayoutPanel();
			this.panelFormButtons = new System.Windows.Forms.Panel();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.tableLayoutPanelAll.SuspendLayout();
			this.panelFormButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanelAll
			// 
			this.tableLayoutPanelAll.AutoSize = true;
			this.tableLayoutPanelAll.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanelAll.ColumnCount = 1;
			this.tableLayoutPanelAll.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanelAll.Controls.Add(this.panelFormButtons, 0, 1);
			this.tableLayoutPanelAll.Controls.Add(this.tableLayoutPanelParameterEditors, 0, 0);
			this.tableLayoutPanelAll.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanelAll.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanelAll.Name = "tableLayoutPanelAll";
			this.tableLayoutPanelAll.RowCount = 2;
			this.tableLayoutPanelAll.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanelAll.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
			this.tableLayoutPanelAll.Size = new System.Drawing.Size(308, 256);
			this.tableLayoutPanelAll.TabIndex = 7;
			// 
			// tableLayoutPanelParameterEditors
			// 
			this.tableLayoutPanelParameterEditors.AutoSize = true;
			this.tableLayoutPanelParameterEditors.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanelParameterEditors.ColumnCount = 1;
			this.tableLayoutPanelParameterEditors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanelParameterEditors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanelParameterEditors.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanelParameterEditors.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanelParameterEditors.Name = "tableLayoutPanelParameterEditors";
			this.tableLayoutPanelParameterEditors.RowCount = 1;
			this.tableLayoutPanelParameterEditors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanelParameterEditors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanelParameterEditors.Size = new System.Drawing.Size(302, 210);
			this.tableLayoutPanelParameterEditors.TabIndex = 0;
			// 
			// panelFormButtons
			// 
			this.panelFormButtons.AutoSize = true;
			this.panelFormButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panelFormButtons.Controls.Add(this.buttonCancel);
			this.panelFormButtons.Controls.Add(this.buttonOK);
			this.panelFormButtons.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelFormButtons.Location = new System.Drawing.Point(3, 219);
			this.panelFormButtons.Name = "panelFormButtons";
			this.panelFormButtons.Size = new System.Drawing.Size(302, 34);
			this.panelFormButtons.TabIndex = 2;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(143, 8);
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
			this.buttonOK.Location = new System.Drawing.Point(224, 8);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 6;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// TimedSequenceEditorEffectEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClientSize = new System.Drawing.Size(308, 256);
			this.Controls.Add(this.tableLayoutPanelAll);
			this.Name = "TimedSequenceEditorEffectEditor";
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
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelParameterEditors;

	}
}