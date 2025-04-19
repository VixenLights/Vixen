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
			this.txtEndTime = new TimeControl();
			this.txtStartTime = new TimeControl();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.txtSpecifiedEffectDuration = new TimeControl();
			this.radioSpecifiedDuration = new System.Windows.Forms.RadioButton();
			this.radioDoNotChangeDuration = new System.Windows.Forms.RadioButton();
			this.radioEqualDuration = new System.Windows.Forms.RadioButton();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.txtSpacedPlacementDuration = new TimeControl();
			this.txtEffectPlacementOverlap = new TimeControl();
			this.radioPlacementSpacedDuration = new System.Windows.Forms.RadioButton();
			this.radioEffectPlacementOverlap = new System.Windows.Forms.RadioButton();
			this.radioStairStep = new System.Windows.Forms.RadioButton();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.radioStartAtLast = new System.Windows.Forms.RadioButton();
			this.radioStartAtFirst = new System.Windows.Forms.RadioButton();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelElementCount
			// 
			this.labelElementCount.AutoSize = true;
			this.labelElementCount.Location = new System.Drawing.Point(3, 0);
			this.labelElementCount.Name = "labelElementCount";
			this.labelElementCount.Size = new System.Drawing.Size(97, 15);
			this.labelElementCount.TabIndex = 0;
			this.labelElementCount.Text = "8 Effects selected";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.txtEndTime);
			this.groupBox1.Controls.Add(this.txtStartTime);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(3, 18);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(233, 95);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Time Control";
			this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// txtEndTime
			// 
			this.txtEndTime.Location = new System.Drawing.Point(79, 60);
			this.txtEndTime.Name = "txtEndTime";
			this.txtEndTime.Size = new System.Drawing.Size(147, 23);
			this.txtEndTime.TabIndex = 7;
			// 
			// txtStartTime
			// 
			this.txtStartTime.Location = new System.Drawing.Point(79, 30);
			this.txtStartTime.Name = "txtStartTime";
			this.txtStartTime.Size = new System.Drawing.Size(147, 23);
			this.txtStartTime.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(7, 63);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(57, 15);
			this.label3.TabIndex = 2;
			this.label3.Text = "End Time";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(7, 33);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(61, 15);
			this.label2.TabIndex = 1;
			this.label2.Text = "Start Time";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.txtSpecifiedEffectDuration);
			this.groupBox2.Controls.Add(this.radioSpecifiedDuration);
			this.groupBox2.Controls.Add(this.radioDoNotChangeDuration);
			this.groupBox2.Controls.Add(this.radioEqualDuration);
			this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.groupBox2.Location = new System.Drawing.Point(3, 119);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(233, 104);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Duration Control";
			this.groupBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// txtSpecifiedEffectDuration
			// 
			this.txtSpecifiedEffectDuration.Location = new System.Drawing.Point(146, 75);
			this.txtSpecifiedEffectDuration.Name = "txtSpecifiedEffectDuration";
			this.txtSpecifiedEffectDuration.Size = new System.Drawing.Size(80, 23);
			this.txtSpecifiedEffectDuration.TabIndex = 8;
			// 
			// radioSpecifiedDuration
			// 
			this.radioSpecifiedDuration.AutoSize = true;
			this.radioSpecifiedDuration.Location = new System.Drawing.Point(10, 76);
			this.radioSpecifiedDuration.Name = "radioSpecifiedDuration";
			this.radioSpecifiedDuration.Size = new System.Drawing.Size(121, 19);
			this.radioSpecifiedDuration.TabIndex = 2;
			this.radioSpecifiedDuration.TabStop = true;
			this.radioSpecifiedDuration.Text = "Specified duration";
			this.radioSpecifiedDuration.UseVisualStyleBackColor = true;
			// 
			// radioDoNotChangeDuration
			// 
			this.radioDoNotChangeDuration.AutoSize = true;
			this.radioDoNotChangeDuration.Location = new System.Drawing.Point(10, 50);
			this.radioDoNotChangeDuration.Name = "radioDoNotChangeDuration";
			this.radioDoNotChangeDuration.Size = new System.Drawing.Size(151, 19);
			this.radioDoNotChangeDuration.TabIndex = 1;
			this.radioDoNotChangeDuration.TabStop = true;
			this.radioDoNotChangeDuration.Text = "Do not change duration";
			this.radioDoNotChangeDuration.UseVisualStyleBackColor = true;
			// 
			// radioEqualDuration
			// 
			this.radioEqualDuration.AutoSize = true;
			this.radioEqualDuration.Location = new System.Drawing.Point(10, 23);
			this.radioEqualDuration.Name = "radioEqualDuration";
			this.radioEqualDuration.Size = new System.Drawing.Size(103, 19);
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
			this.groupBox3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.groupBox3.Location = new System.Drawing.Point(3, 229);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(233, 107);
			this.groupBox3.TabIndex = 3;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Placement Control";
			this.groupBox3.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// txtSpacedPlacementDuration
			// 
			this.txtSpacedPlacementDuration.Location = new System.Drawing.Point(146, 74);
			this.txtSpacedPlacementDuration.Name = "txtSpacedPlacementDuration";
			this.txtSpacedPlacementDuration.Size = new System.Drawing.Size(80, 23);
			this.txtSpacedPlacementDuration.TabIndex = 11;
			// 
			// txtEffectPlacementOverlap
			// 
			this.txtEffectPlacementOverlap.Location = new System.Drawing.Point(146, 47);
			this.txtEffectPlacementOverlap.Name = "txtEffectPlacementOverlap";
			this.txtEffectPlacementOverlap.Size = new System.Drawing.Size(80, 23);
			this.txtEffectPlacementOverlap.TabIndex = 10;
			// 
			// radioPlacementSpacedDuration
			// 
			this.radioPlacementSpacedDuration.AutoSize = true;
			this.radioPlacementSpacedDuration.Location = new System.Drawing.Point(7, 75);
			this.radioPlacementSpacedDuration.Name = "radioPlacementSpacedDuration";
			this.radioPlacementSpacedDuration.Size = new System.Drawing.Size(111, 19);
			this.radioPlacementSpacedDuration.TabIndex = 2;
			this.radioPlacementSpacedDuration.TabStop = true;
			this.radioPlacementSpacedDuration.Text = "Spaced duration";
			this.radioPlacementSpacedDuration.UseVisualStyleBackColor = true;
			// 
			// radioEffectPlacementOverlap
			// 
			this.radioEffectPlacementOverlap.AutoSize = true;
			this.radioEffectPlacementOverlap.Location = new System.Drawing.Point(7, 48);
			this.radioEffectPlacementOverlap.Name = "radioEffectPlacementOverlap";
			this.radioEffectPlacementOverlap.Size = new System.Drawing.Size(90, 19);
			this.radioEffectPlacementOverlap.TabIndex = 9;
			this.radioEffectPlacementOverlap.TabStop = true;
			this.radioEffectPlacementOverlap.Text = "Overlapping";
			this.radioEffectPlacementOverlap.UseVisualStyleBackColor = true;
			// 
			// radioStairStep
			// 
			this.radioStairStep.AutoSize = true;
			this.radioStairStep.Location = new System.Drawing.Point(7, 22);
			this.radioStairStep.Name = "radioStairStep";
			this.radioStairStep.Size = new System.Drawing.Size(93, 19);
			this.radioStairStep.TabIndex = 1;
			this.radioStairStep.TabStop = true;
			this.radioStairStep.Text = "Stair stepped";
			this.radioStairStep.UseVisualStyleBackColor = true;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.radioStartAtLast);
			this.groupBox4.Controls.Add(this.radioStartAtFirst);
			this.groupBox4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.groupBox4.Location = new System.Drawing.Point(3, 342);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(233, 52);
			this.groupBox4.TabIndex = 4;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Starting point";
			this.groupBox4.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// radioStartAtLast
			// 
			this.radioStartAtLast.AutoSize = true;
			this.radioStartAtLast.Location = new System.Drawing.Point(138, 20);
			this.radioStartAtLast.Name = "radioStartAtLast";
			this.radioStartAtLast.Size = new System.Drawing.Size(79, 19);
			this.radioStartAtLast.TabIndex = 3;
			this.radioStartAtLast.TabStop = true;
			this.radioStartAtLast.Text = "Last Effect";
			this.radioStartAtLast.UseVisualStyleBackColor = true;
			// 
			// radioStartAtFirst
			// 
			this.radioStartAtFirst.AutoSize = true;
			this.radioStartAtFirst.Location = new System.Drawing.Point(7, 20);
			this.radioStartAtFirst.Name = "radioStartAtFirst";
			this.radioStartAtFirst.Size = new System.Drawing.Size(80, 19);
			this.radioStartAtFirst.TabIndex = 2;
			this.radioStartAtFirst.TabStop = true;
			this.radioStartAtFirst.Text = "First Effect";
			this.radioStartAtFirst.UseVisualStyleBackColor = true;
			// 
			// btnOK
			// 
			this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(14, 3);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(87, 27);
			this.btnOK.TabIndex = 5;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = false;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(131, 3);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(87, 27);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = false;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.Controls.Add(this.labelElementCount);
			this.flowLayoutPanel1.Controls.Add(this.groupBox1);
			this.flowLayoutPanel1.Controls.Add(this.groupBox2);
			this.flowLayoutPanel1.Controls.Add(this.groupBox3);
			this.flowLayoutPanel1.Controls.Add(this.groupBox4);
			this.flowLayoutPanel1.Controls.Add(this.tableLayoutPanel1);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(5, 5);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(244, 440);
			this.flowLayoutPanel1.TabIndex = 7;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.btnOK, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.btnCancel, 1, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 400);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(233, 33);
			this.tableLayoutPanel1.TabIndex = 7;
			// 
			// EffectDistributionDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(254, 450);
			this.Controls.Add(this.flowLayoutPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			this.Name = "EffectDistributionDialog";
			this.Padding = new System.Windows.Forms.Padding(5);
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
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
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
		private TimeControl txtEndTime;
		private TimeControl txtStartTime;
		private System.Windows.Forms.ToolTip toolTip;
		private TimeControl txtSpecifiedEffectDuration;
		private TimeControl txtSpacedPlacementDuration;
		private TimeControl txtEffectPlacementOverlap;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}