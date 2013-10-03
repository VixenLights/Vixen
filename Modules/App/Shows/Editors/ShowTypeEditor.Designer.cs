namespace VixenModules.App.Shows
{
	partial class ShowTypeEditor
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
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.toolTip1 = new System.Windows.Forms.ToolTip();
			this.checkBoxStopCurrentShow = new System.Windows.Forms.CheckBox();
			this.comboBoxShow = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(0, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(74, 13);
			this.label1.TabIndex = 10;
			this.label1.Text = "Show to Start:";
			// 
			// checkBoxStopCurrentShow
			// 
			this.checkBoxStopCurrentShow.AutoSize = true;
			this.checkBoxStopCurrentShow.Location = new System.Drawing.Point(80, 28);
			this.checkBoxStopCurrentShow.Name = "checkBoxStopCurrentShow";
			this.checkBoxStopCurrentShow.Size = new System.Drawing.Size(115, 17);
			this.checkBoxStopCurrentShow.TabIndex = 16;
			this.checkBoxStopCurrentShow.Text = "Stop Current Show";
			this.checkBoxStopCurrentShow.UseVisualStyleBackColor = true;
			this.checkBoxStopCurrentShow.CheckedChanged += new System.EventHandler(this.checkBoxStopCurrentShow_CheckedChanged);
			// 
			// comboBoxShow
			// 
			this.comboBoxShow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxShow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxShow.FormattingEnabled = true;
			this.comboBoxShow.Location = new System.Drawing.Point(74, 3);
			this.comboBoxShow.Name = "comboBoxShow";
			this.comboBoxShow.Size = new System.Drawing.Size(213, 21);
			this.comboBoxShow.TabIndex = 17;
			this.comboBoxShow.SelectedIndexChanged += new System.EventHandler(this.comboBoxShow_SelectedIndexChanged);
			// 
			// ShowTypeEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.comboBoxShow);
			this.Controls.Add(this.checkBoxStopCurrentShow);
			this.Controls.Add(this.label1);
			this.Name = "ShowTypeEditor";
			this.Size = new System.Drawing.Size(290, 150);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.CheckBox checkBoxStopCurrentShow;
		private System.Windows.Forms.ComboBox comboBoxShow;
	}
}
