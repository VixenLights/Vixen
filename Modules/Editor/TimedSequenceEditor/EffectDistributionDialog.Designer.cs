namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class EffectDistributionDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.labelElementCount = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.radioSpecifiedDuration = new System.Windows.Forms.RadioButton();
			this.radioDoNotChangeDuration = new System.Windows.Forms.RadioButton();
			this.radioEqualDuration = new System.Windows.Forms.RadioButton();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.radioPlacementSpacedDuration = new System.Windows.Forms.RadioButton();
			this.radioEffectPlacementOverlap = new System.Windows.Forms.RadioButton();
			this.radioStairStep = new System.Windows.Forms.RadioButton();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.radioStartAtLast = new System.Windows.Forms.RadioButton();
			this.radioStartAtFirst = new System.Windows.Forms.RadioButton();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.txtStartTime = new System.Windows.Forms.MaskedTextBox();
			this.txtEndTime = new System.Windows.Forms.MaskedTextBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.txtSpecifiedEffectDuration = new System.Windows.Forms.MaskedTextBox();
			this.txtEffectPlacementOverlap = new System.Windows.Forms.MaskedTextBox();
			this.txtSpacedPlacementDuration = new System.Windows.Forms.MaskedTextBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelElementCount
			// 
			this.labelElementCount.AutoSize = true;
			this.labelElementCount.Location = new System.Drawing.Point(13, 9);
			this.labelElementCount.Name = "labelElementCount";
			this.labelElementCount.Size = new System.Drawing.Size(92, 13);
			this.labelElementCount.TabIndex = 0;
			this.labelElementCount.Text = "8 Effects selected";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.txtEndTime);
			this.groupBox1.Controls.Add(this.txtStartTime);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(16, 40);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 82);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Time Control";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 55);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(52, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "End Time";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 29);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(55, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Start Time";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.txtSpecifiedEffectDuration);
			this.groupBox2.Controls.Add(this.radioSpecifiedDuration);
			this.groupBox2.Controls.Add(this.radioDoNotChangeDuration);
			this.groupBox2.Controls.Add(this.radioEqualDuration);
			this.groupBox2.Location = new System.Drawing.Point(16, 128);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(200, 90);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Duration Control";
			// 
			// radioSpecifiedDuration
			// 
			this.radioSpecifiedDuration.AutoSize = true;
			this.radioSpecifiedDuration.Location = new System.Drawing.Point(9, 66);
			this.radioSpecifiedDuration.Name = "radioSpecifiedDuration";
			this.radioSpecifiedDuration.Size = new System.Drawing.Size(110, 17);
			this.radioSpecifiedDuration.TabIndex = 2;
			this.radioSpecifiedDuration.TabStop = true;
			this.radioSpecifiedDuration.Text = "Specified duration";
			this.radioSpecifiedDuration.UseVisualStyleBackColor = true;
			// 
			// radioDoNotChangeDuration
			// 
			this.radioDoNotChangeDuration.AutoSize = true;
			this.radioDoNotChangeDuration.Location = new System.Drawing.Point(9, 43);
			this.radioDoNotChangeDuration.Name = "radioDoNotChangeDuration";
			this.radioDoNotChangeDuration.Size = new System.Drawing.Size(137, 17);
			this.radioDoNotChangeDuration.TabIndex = 1;
			this.radioDoNotChangeDuration.TabStop = true;
			this.radioDoNotChangeDuration.Text = "Do not change duration";
			this.radioDoNotChangeDuration.UseVisualStyleBackColor = true;
			// 
			// radioEqualDuration
			// 
			this.radioEqualDuration.AutoSize = true;
			this.radioEqualDuration.Location = new System.Drawing.Point(9, 20);
			this.radioEqualDuration.Name = "radioEqualDuration";
			this.radioEqualDuration.Size = new System.Drawing.Size(95, 17);
			this.radioEqualDuration.TabIndex = 0;
			this.radioEqualDuration.TabStop = true;
			this.radioEqualDuration.Text = "Equal Duration";
			this.radioEqualDuration.UseVisualStyleBackColor = true;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.txtSpacedPlacementDuration);
			this.groupBox3.Controls.Add(this.txtEffectPlacementOverlap);
			this.groupBox3.Controls.Add(this.radioPlacementSpacedDuration);
			this.groupBox3.Controls.Add(this.radioEffectPlacementOverlap);
			this.groupBox3.Controls.Add(this.radioStairStep);
			this.groupBox3.Location = new System.Drawing.Point(16, 224);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(200, 93);
			this.groupBox3.TabIndex = 3;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Placement Control";
			// 
			// radioPlacementSpacedDuration
			// 
			this.radioPlacementSpacedDuration.AutoSize = true;
			this.radioPlacementSpacedDuration.Location = new System.Drawing.Point(6, 65);
			this.radioPlacementSpacedDuration.Name = "radioPlacementSpacedDuration";
			this.radioPlacementSpacedDuration.Size = new System.Drawing.Size(103, 17);
			this.radioPlacementSpacedDuration.TabIndex = 2;
			this.radioPlacementSpacedDuration.TabStop = true;
			this.radioPlacementSpacedDuration.Text = "Spaced duration";
			this.radioPlacementSpacedDuration.UseVisualStyleBackColor = true;
			// 
			// radioEffectPlacementOverlap
			// 
			this.radioEffectPlacementOverlap.AutoSize = true;
			this.radioEffectPlacementOverlap.Location = new System.Drawing.Point(6, 42);
			this.radioEffectPlacementOverlap.Name = "radioEffectPlacementOverlap";
			this.radioEffectPlacementOverlap.Size = new System.Drawing.Size(82, 17);
			this.radioEffectPlacementOverlap.TabIndex = 9;
			this.radioEffectPlacementOverlap.TabStop = true;
			this.radioEffectPlacementOverlap.Text = "Overlapping";
			this.radioEffectPlacementOverlap.UseVisualStyleBackColor = true;
			// 
			// radioStairStep
			// 
			this.radioStairStep.AutoSize = true;
			this.radioStairStep.Location = new System.Drawing.Point(6, 19);
			this.radioStairStep.Name = "radioStairStep";
			this.radioStairStep.Size = new System.Drawing.Size(87, 17);
			this.radioStairStep.TabIndex = 1;
			this.radioStairStep.TabStop = true;
			this.radioStairStep.Text = "Stair stepped";
			this.radioStairStep.UseVisualStyleBackColor = true;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.radioStartAtLast);
			this.groupBox4.Controls.Add(this.radioStartAtFirst);
			this.groupBox4.Location = new System.Drawing.Point(16, 323);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(200, 45);
			this.groupBox4.TabIndex = 4;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Starting point";
			// 
			// radioStartAtLast
			// 
			this.radioStartAtLast.AutoSize = true;
			this.radioStartAtLast.Location = new System.Drawing.Point(118, 17);
			this.radioStartAtLast.Name = "radioStartAtLast";
			this.radioStartAtLast.Size = new System.Drawing.Size(76, 17);
			this.radioStartAtLast.TabIndex = 3;
			this.radioStartAtLast.TabStop = true;
			this.radioStartAtLast.Text = "Last Effect";
			this.radioStartAtLast.UseVisualStyleBackColor = true;
			// 
			// radioStartAtFirst
			// 
			this.radioStartAtFirst.AutoSize = true;
			this.radioStartAtFirst.Location = new System.Drawing.Point(6, 17);
			this.radioStartAtFirst.Name = "radioStartAtFirst";
			this.radioStartAtFirst.Size = new System.Drawing.Size(75, 17);
			this.radioStartAtFirst.TabIndex = 2;
			this.radioStartAtFirst.TabStop = true;
			this.radioStartAtFirst.Text = "First Effect";
			this.radioStartAtFirst.UseVisualStyleBackColor = true;
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(16, 374);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 5;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(141, 374);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// txtStartTime
			// 
			this.txtStartTime.Location = new System.Drawing.Point(68, 26);
			this.txtStartTime.Name = "txtStartTime";
			this.txtStartTime.Size = new System.Drawing.Size(126, 20);
			this.txtStartTime.TabIndex = 6;
			// 
			// txtEndTime
			// 
			this.txtEndTime.Location = new System.Drawing.Point(68, 52);
			this.txtEndTime.Name = "txtEndTime";
			this.txtEndTime.Size = new System.Drawing.Size(126, 20);
			this.txtEndTime.TabIndex = 7;
			// 
			// txtSpecifiedEffectDuration
			// 
			this.txtSpecifiedEffectDuration.Location = new System.Drawing.Point(125, 65);
			this.txtSpecifiedEffectDuration.Name = "txtSpecifiedEffectDuration";
			this.txtSpecifiedEffectDuration.Size = new System.Drawing.Size(69, 20);
			this.txtSpecifiedEffectDuration.TabIndex = 8;
			// 
			// txtEffectPlacementOverlap
			// 
			this.txtEffectPlacementOverlap.Location = new System.Drawing.Point(125, 41);
			this.txtEffectPlacementOverlap.Name = "txtEffectPlacementOverlap";
			this.txtEffectPlacementOverlap.Size = new System.Drawing.Size(69, 20);
			this.txtEffectPlacementOverlap.TabIndex = 10;
			// 
			// txtSpacedPlacementDuration
			// 
			this.txtSpacedPlacementDuration.Location = new System.Drawing.Point(125, 64);
			this.txtSpacedPlacementDuration.Name = "txtSpacedPlacementDuration";
			this.txtSpacedPlacementDuration.Size = new System.Drawing.Size(69, 20);
			this.txtSpacedPlacementDuration.TabIndex = 11;
			// 
			// EffectDistributionDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(228, 404);
			this.ControlBox = false;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.labelElementCount);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "EffectDistributionDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Effect Distribution";
			this.Load += new System.EventHandler(this.EffectDistributionDialog_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelElementCount;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioSpecifiedDuration;
		private System.Windows.Forms.RadioButton radioDoNotChangeDuration;
		private System.Windows.Forms.RadioButton radioEqualDuration;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton radioPlacementSpacedDuration;
		private System.Windows.Forms.RadioButton radioStairStep;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.RadioButton radioStartAtLast;
		private System.Windows.Forms.RadioButton radioStartAtFirst;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.RadioButton radioEffectPlacementOverlap;
		private System.Windows.Forms.MaskedTextBox txtEndTime;
		private System.Windows.Forms.MaskedTextBox txtStartTime;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.MaskedTextBox txtSpecifiedEffectDuration;
		private System.Windows.Forms.MaskedTextBox txtSpacedPlacementDuration;
		private System.Windows.Forms.MaskedTextBox txtEffectPlacementOverlap;
	}
}