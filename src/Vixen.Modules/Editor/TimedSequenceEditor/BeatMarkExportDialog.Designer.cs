namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class BeatMarkExportDialog
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			radioAudacityFormat = new RadioButton();
			radioVixen3Format = new RadioButton();
			buttonOK = new Button();
			buttonCancel = new Button();
			radioPangolinBeyondFormat = new RadioButton();
			tableLayoutPanel1 = new TableLayoutPanel();
			flowLayoutPanel1 = new FlowLayoutPanel();
			flowLayoutPanel2 = new FlowLayoutPanel();
			label1 = new Label();
			tableLayoutPanel1.SuspendLayout();
			flowLayoutPanel1.SuspendLayout();
			flowLayoutPanel2.SuspendLayout();
			SuspendLayout();
			// 
			// radioAudacityFormat
			// 
			radioAudacityFormat.AutoSize = true;
			radioAudacityFormat.Location = new Point(3, 3);
			radioAudacityFormat.Name = "radioAudacityFormat";
			radioAudacityFormat.Size = new Size(72, 19);
			radioAudacityFormat.TabIndex = 1;
			radioAudacityFormat.TabStop = true;
			radioAudacityFormat.Text = "Audacity";
			radioAudacityFormat.UseVisualStyleBackColor = true;
			// 
			// radioVixen3Format
			// 
			radioVixen3Format.AutoSize = true;
			radioVixen3Format.Location = new Point(3, 28);
			radioVixen3Format.Name = "radioVixen3Format";
			radioVixen3Format.Size = new Size(62, 19);
			radioVixen3Format.TabIndex = 0;
			radioVixen3Format.TabStop = true;
			radioVixen3Format.Text = "Vixen 3";
			radioVixen3Format.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			buttonOK.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			buttonOK.DialogResult = DialogResult.OK;
			buttonOK.Location = new Point(31, 3);
			buttonOK.Name = "buttonOK";
			buttonOK.Size = new Size(87, 27);
			buttonOK.TabIndex = 1;
			buttonOK.Text = "OK";
			buttonOK.UseVisualStyleBackColor = false;
			// 
			// buttonCancel
			// 
			buttonCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			buttonCancel.DialogResult = DialogResult.Cancel;
			buttonCancel.Location = new Point(124, 3);
			buttonCancel.Name = "buttonCancel";
			buttonCancel.Size = new Size(87, 27);
			buttonCancel.TabIndex = 2;
			buttonCancel.Text = "Cancel";
			buttonCancel.UseVisualStyleBackColor = false;
			// 
			// radioPangolinBeyondFormat
			// 
			radioPangolinBeyondFormat.AutoSize = true;
			radioPangolinBeyondFormat.Location = new Point(3, 53);
			radioPangolinBeyondFormat.Name = "radioPangolinBeyondFormat";
			radioPangolinBeyondFormat.Size = new Size(115, 19);
			radioPangolinBeyondFormat.TabIndex = 3;
			radioPangolinBeyondFormat.TabStop = true;
			radioPangolinBeyondFormat.Text = "Pangolin Beyond";
			radioPangolinBeyondFormat.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.ColumnCount = 1;
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			tableLayoutPanel1.Controls.Add(flowLayoutPanel1, 0, 1);
			tableLayoutPanel1.Controls.Add(flowLayoutPanel2, 0, 2);
			tableLayoutPanel1.Controls.Add(label1, 0, 0);
			tableLayoutPanel1.Dock = DockStyle.Fill;
			tableLayoutPanel1.Location = new Point(0, 0);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 3;
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.Size = new Size(220, 186);
			tableLayoutPanel1.TabIndex = 4;
			// 
			// flowLayoutPanel1
			// 
			flowLayoutPanel1.AutoSize = true;
			flowLayoutPanel1.Controls.Add(radioAudacityFormat);
			flowLayoutPanel1.Controls.Add(radioVixen3Format);
			flowLayoutPanel1.Controls.Add(radioPangolinBeyondFormat);
			flowLayoutPanel1.Dock = DockStyle.Fill;
			flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
			flowLayoutPanel1.Location = new Point(3, 28);
			flowLayoutPanel1.Name = "flowLayoutPanel1";
			flowLayoutPanel1.Size = new Size(214, 116);
			flowLayoutPanel1.TabIndex = 0;
			// 
			// flowLayoutPanel2
			// 
			flowLayoutPanel2.AutoSize = true;
			flowLayoutPanel2.Controls.Add(buttonCancel);
			flowLayoutPanel2.Controls.Add(buttonOK);
			flowLayoutPanel2.Dock = DockStyle.Fill;
			flowLayoutPanel2.FlowDirection = FlowDirection.RightToLeft;
			flowLayoutPanel2.Location = new Point(3, 150);
			flowLayoutPanel2.Name = "flowLayoutPanel2";
			flowLayoutPanel2.Size = new Size(214, 33);
			flowLayoutPanel2.TabIndex = 1;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(3, 0);
			label1.Name = "label1";
			label1.Padding = new Padding(0, 5, 0, 5);
			label1.Size = new Size(88, 25);
			label1.TabIndex = 2;
			label1.Text = "Choose Format";
			// 
			// BeatMarkExportDialog
			// 
			AcceptButton = buttonOK;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = true;
			CancelButton = buttonCancel;
			ClientSize = new Size(220, 186);
			Controls.Add(tableLayoutPanel1);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MaximumSize = new Size(236, 225);
			MinimizeBox = false;
			MinimumSize = new Size(236, 225);
			Name = "BeatMarkExportDialog";
			StartPosition = FormStartPosition.CenterParent;
			Text = "Export Format";
			Load += BeatMarkExportDialog_Load;
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			flowLayoutPanel1.ResumeLayout(false);
			flowLayoutPanel1.PerformLayout();
			flowLayoutPanel2.ResumeLayout(false);
			ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RadioButton radioAudacityFormat;
		private System.Windows.Forms.RadioButton radioVixen3Format;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private RadioButton radioPangolinBeyondFormat;
		private TableLayoutPanel tableLayoutPanel1;
		private FlowLayoutPanel flowLayoutPanel1;
		private FlowLayoutPanel flowLayoutPanel2;
		private Label label1;
	}
}