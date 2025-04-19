namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class BulkEffectMoveForm
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
			this.components = new System.ComponentModel.Container();
			this.txtStartTime = new TimeControl();
			this.txtEndTime = new TimeControl();
			this.txtOffset = new TimeControl();
			this.label1 = new System.Windows.Forms.Label();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioButtonBackward = new System.Windows.Forms.RadioButton();
			this.radioButtonForward = new System.Windows.Forms.RadioButton();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.checkBoxVisibleRows = new System.Windows.Forms.CheckBox();
			this.checkBoxClipEffects = new System.Windows.Forms.CheckBox();
			this.checkBoxMoveMarks = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtStartTime
			// 
			this.txtStartTime.Location = new System.Drawing.Point(121, 14);
			this.txtStartTime.Name = "txtStartTime";
			this.txtStartTime.Size = new System.Drawing.Size(116, 23);
			this.txtStartTime.TabIndex = 0;
			// 
			// txtEndTime
			// 
			this.txtEndTime.Location = new System.Drawing.Point(121, 44);
			this.txtEndTime.Name = "txtEndTime";
			this.txtEndTime.Size = new System.Drawing.Size(116, 23);
			this.txtEndTime.TabIndex = 1;
			// 
			// txtOffset
			// 
			this.txtOffset.Location = new System.Drawing.Point(121, 74);
			this.txtOffset.Name = "txtOffset";
			this.txtOffset.Size = new System.Drawing.Size(116, 23);
			this.txtOffset.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(50, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(61, 15);
			this.label1.TabIndex = 3;
			this.label1.Text = "Start Time";
			// 
			// linkLabel1
			// 
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Location = new System.Drawing.Point(77, 27);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(0, 15);
			this.linkLabel1.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(54, 44);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(57, 15);
			this.label2.TabIndex = 5;
			this.label2.Text = "End Time";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(70, 74);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(39, 15);
			this.label3.TabIndex = 6;
			this.label3.Text = "Offset";
			// 
			// btnOk
			// 
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(114, 266);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(87, 27);
			this.btnOk.TabIndex = 7;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(208, 266);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(87, 27);
			this.btnCancel.TabIndex = 8;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioButtonBackward);
			this.groupBox1.Controls.Add(this.radioButtonForward);
			this.groupBox1.Location = new System.Drawing.Point(54, 103);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(184, 78);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Direction";
			this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// radioButtonBackward
			// 
			this.radioButtonBackward.AutoSize = true;
			this.radioButtonBackward.Location = new System.Drawing.Point(9, 51);
			this.radioButtonBackward.Name = "radioButtonBackward";
			this.radioButtonBackward.Size = new System.Drawing.Size(76, 19);
			this.radioButtonBackward.TabIndex = 1;
			this.radioButtonBackward.Text = "Backward";
			this.radioButtonBackward.UseVisualStyleBackColor = true;
			// 
			// radioButtonForward
			// 
			this.radioButtonForward.AutoSize = true;
			this.radioButtonForward.Checked = true;
			this.radioButtonForward.Location = new System.Drawing.Point(9, 24);
			this.radioButtonForward.Name = "radioButtonForward";
			this.radioButtonForward.Size = new System.Drawing.Size(68, 19);
			this.radioButtonForward.TabIndex = 0;
			this.radioButtonForward.TabStop = true;
			this.radioButtonForward.Text = "Forward";
			this.radioButtonForward.UseVisualStyleBackColor = true;
			// 
			// checkBoxVisibleRows
			// 
			this.checkBoxVisibleRows.AutoSize = true;
			this.checkBoxVisibleRows.Location = new System.Drawing.Point(63, 188);
			this.checkBoxVisibleRows.Name = "checkBoxVisibleRows";
			this.checkBoxVisibleRows.Size = new System.Drawing.Size(156, 19);
			this.checkBoxVisibleRows.TabIndex = 10;
			this.checkBoxVisibleRows.Text = "Process visible rows only";
			this.checkBoxVisibleRows.UseVisualStyleBackColor = true;
			// 
			// checkBoxClipEffects
			// 
			this.checkBoxClipEffects.AutoSize = true;
			this.checkBoxClipEffects.Location = new System.Drawing.Point(63, 210);
			this.checkBoxClipEffects.Name = "checkBoxClipEffects";
			this.checkBoxClipEffects.Size = new System.Drawing.Size(156, 19);
			this.checkBoxClipEffects.TabIndex = 10;
			this.checkBoxClipEffects.Text = "Clip Effects";
			this.checkBoxClipEffects.UseVisualStyleBackColor = true;
			// 
			// checkBoxMoveMarks
			// 
			this.checkBoxMoveMarks.AutoSize = true;
			this.checkBoxMoveMarks.Location = new System.Drawing.Point(63, 232);
			this.checkBoxMoveMarks.Name = "checkBoxMoveMarks";
			this.checkBoxMoveMarks.Size = new System.Drawing.Size(156, 19);
			this.checkBoxMoveMarks.TabIndex = 11;
			this.checkBoxMoveMarks.Text = "Include Marks";
			this.checkBoxMoveMarks.UseVisualStyleBackColor = true;
			// 
			// BulkEffectMoveForm
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(307, 307);
			this.Controls.Add(this.checkBoxMoveMarks);
			this.Controls.Add(this.checkBoxClipEffects);
			this.Controls.Add(this.checkBoxVisibleRows);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtOffset);
			this.Controls.Add(this.txtEndTime);
			this.Controls.Add(this.txtStartTime);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			this.Name = "BulkEffectMoveForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Bulk Effect Move";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private TimeControl txtStartTime;
		private TimeControl txtEndTime;
		private TimeControl txtOffset;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonBackward;
		private System.Windows.Forms.RadioButton radioButtonForward;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.CheckBox checkBoxVisibleRows;
		private System.Windows.Forms.CheckBox checkBoxClipEffects;
		private System.Windows.Forms.CheckBox checkBoxMoveMarks;
	}
}