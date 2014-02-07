namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class EffectDistributionDialog
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
			this.labelElementCount = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textBoxDuration = new System.Windows.Forms.TextBox();
			this.textBoxEndTime = new System.Windows.Forms.TextBox();
			this.textBoxStartTime = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.textBoxSpecifiedEffectDuration = new System.Windows.Forms.TextBox();
			this.radioSpecifiedDuration = new System.Windows.Forms.RadioButton();
			this.radioDoNotChangeDuration = new System.Windows.Forms.RadioButton();
			this.radioEqualDuration = new System.Windows.Forms.RadioButton();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.textBoxSpacedPlacementDuration = new System.Windows.Forms.TextBox();
			this.radioPlacementSpacedDuration = new System.Windows.Forms.RadioButton();
			this.radioStairStep = new System.Windows.Forms.RadioButton();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.radioStartAtLast = new System.Windows.Forms.RadioButton();
			this.radioStartAtFirst = new System.Windows.Forms.RadioButton();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.textBoxEffectPlacementOverlap = new System.Windows.Forms.TextBox();
			this.radioEffectPlacementOverlap = new System.Windows.Forms.RadioButton();
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
			this.groupBox1.Controls.Add(this.textBoxDuration);
			this.groupBox1.Controls.Add(this.textBoxEndTime);
			this.groupBox1.Controls.Add(this.textBoxStartTime);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(16, 40);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 107);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Time Control";
			// 
			// textBoxDuration
			// 
			this.textBoxDuration.Location = new System.Drawing.Point(68, 78);
			this.textBoxDuration.Name = "textBoxDuration";
			this.textBoxDuration.Size = new System.Drawing.Size(126, 20);
			this.textBoxDuration.TabIndex = 6;
			// 
			// textBoxEndTime
			// 
			this.textBoxEndTime.Location = new System.Drawing.Point(68, 52);
			this.textBoxEndTime.Name = "textBoxEndTime";
			this.textBoxEndTime.Size = new System.Drawing.Size(126, 20);
			this.textBoxEndTime.TabIndex = 5;
			// 
			// textBoxStartTime
			// 
			this.textBoxStartTime.Location = new System.Drawing.Point(68, 26);
			this.textBoxStartTime.Name = "textBoxStartTime";
			this.textBoxStartTime.Size = new System.Drawing.Size(126, 20);
			this.textBoxStartTime.TabIndex = 4;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 74);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(47, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "Duration";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 51);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(52, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "End Time";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 26);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(55, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Start Time";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textBoxSpecifiedEffectDuration);
			this.groupBox2.Controls.Add(this.radioSpecifiedDuration);
			this.groupBox2.Controls.Add(this.radioDoNotChangeDuration);
			this.groupBox2.Controls.Add(this.radioEqualDuration);
			this.groupBox2.Location = new System.Drawing.Point(16, 153);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(200, 90);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Duration Control";
			// 
			// textBoxSpecifiedEffectDuration
			// 
			this.textBoxSpecifiedEffectDuration.Location = new System.Drawing.Point(125, 63);
			this.textBoxSpecifiedEffectDuration.Name = "textBoxSpecifiedEffectDuration";
			this.textBoxSpecifiedEffectDuration.Size = new System.Drawing.Size(67, 20);
			this.textBoxSpecifiedEffectDuration.TabIndex = 7;
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
			this.groupBox3.Controls.Add(this.textBoxEffectPlacementOverlap);
			this.groupBox3.Controls.Add(this.textBoxSpacedPlacementDuration);
			this.groupBox3.Controls.Add(this.radioPlacementSpacedDuration);
			this.groupBox3.Controls.Add(this.radioEffectPlacementOverlap);
			this.groupBox3.Controls.Add(this.radioStairStep);
			this.groupBox3.Location = new System.Drawing.Point(16, 249);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(200, 93);
			this.groupBox3.TabIndex = 3;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Placement Control";
			// 
			// textBoxSpacedPlacementDuration
			// 
			this.textBoxSpacedPlacementDuration.Location = new System.Drawing.Point(125, 64);
			this.textBoxSpacedPlacementDuration.Name = "textBoxSpacedPlacementDuration";
			this.textBoxSpacedPlacementDuration.Size = new System.Drawing.Size(67, 20);
			this.textBoxSpacedPlacementDuration.TabIndex = 8;
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
			this.groupBox4.Location = new System.Drawing.Point(16, 348);
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
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(16, 399);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 5;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(141, 399);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// textBoxEffectPlacementOverlap
			// 
			this.textBoxEffectPlacementOverlap.Location = new System.Drawing.Point(125, 41);
			this.textBoxEffectPlacementOverlap.Name = "textBoxEffectPlacementOverlap";
			this.textBoxEffectPlacementOverlap.Size = new System.Drawing.Size(67, 20);
			this.textBoxEffectPlacementOverlap.TabIndex = 10;
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
			// EffectDistributionDialog
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(228, 430);
			this.ControlBox = false;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
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
		private System.Windows.Forms.TextBox textBoxDuration;
		private System.Windows.Forms.TextBox textBoxEndTime;
		private System.Windows.Forms.TextBox textBoxStartTime;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox textBoxSpecifiedEffectDuration;
		private System.Windows.Forms.RadioButton radioSpecifiedDuration;
		private System.Windows.Forms.RadioButton radioDoNotChangeDuration;
		private System.Windows.Forms.RadioButton radioEqualDuration;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox textBoxSpacedPlacementDuration;
		private System.Windows.Forms.RadioButton radioPlacementSpacedDuration;
		private System.Windows.Forms.RadioButton radioStairStep;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.RadioButton radioStartAtLast;
		private System.Windows.Forms.RadioButton radioStartAtFirst;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.TextBox textBoxEffectPlacementOverlap;
		private System.Windows.Forms.RadioButton radioEffectPlacementOverlap;
	}
}