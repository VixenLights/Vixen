namespace VixenApplication
{
	partial class DisplaySetup
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.label1 = new System.Windows.Forms.Label();
			this.tableLayoutPanelElementSetup = new System.Windows.Forms.TableLayoutPanel();
			this.label2 = new System.Windows.Forms.Label();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.tableLayoutPanelPatchingSetup = new System.Windows.Forms.TableLayoutPanel();
			this.label4 = new System.Windows.Forms.Label();
			this.radioButtonPatchingGraphical = new System.Windows.Forms.RadioButton();
			this.label3 = new System.Windows.Forms.Label();
			this.radioButtonPatchingSimple = new System.Windows.Forms.RadioButton();
			this.label5 = new System.Windows.Forms.Label();
			this.buttonHelp = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.tableLayoutPanelControllerSetup = new System.Windows.Forms.TableLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.label1);
			this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanelElementSetup);
			this.splitContainer1.Panel1.Controls.Add(this.label2);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(1206, 737);
			this.splitContainer1.SplitterDistance = 317;
			this.splitContainer1.SplitterWidth = 5;
			this.splitContainer1.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(97, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(84, 20);
			this.label1.TabIndex = 1;
			this.label1.Text = "Elements";
			// 
			// tableLayoutPanelElementSetup
			// 
			this.tableLayoutPanelElementSetup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanelElementSetup.AutoSize = true;
			this.tableLayoutPanelElementSetup.ColumnCount = 1;
			this.tableLayoutPanelElementSetup.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanelElementSetup.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanelElementSetup.Location = new System.Drawing.Point(0, 40);
			this.tableLayoutPanelElementSetup.Name = "tableLayoutPanelElementSetup";
			this.tableLayoutPanelElementSetup.RowCount = 1;
			this.tableLayoutPanelElementSetup.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanelElementSetup.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanelElementSetup.Size = new System.Drawing.Size(313, 693);
			this.tableLayoutPanelElementSetup.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label2.Location = new System.Drawing.Point(6, 37);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(303, 2);
			this.label2.TabIndex = 3;
			// 
			// splitContainer2
			// 
			this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.tableLayoutPanelPatchingSetup);
			this.splitContainer2.Panel1.Controls.Add(this.label4);
			this.splitContainer2.Panel1.Controls.Add(this.radioButtonPatchingGraphical);
			this.splitContainer2.Panel1.Controls.Add(this.label3);
			this.splitContainer2.Panel1.Controls.Add(this.radioButtonPatchingSimple);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.label5);
			this.splitContainer2.Panel2.Controls.Add(this.buttonHelp);
			this.splitContainer2.Panel2.Controls.Add(this.label7);
			this.splitContainer2.Panel2.Controls.Add(this.buttonCancel);
			this.splitContainer2.Panel2.Controls.Add(this.buttonOk);
			this.splitContainer2.Panel2.Controls.Add(this.label6);
			this.splitContainer2.Panel2.Controls.Add(this.tableLayoutPanelControllerSetup);
			this.splitContainer2.Size = new System.Drawing.Size(884, 737);
			this.splitContainer2.SplitterDistance = 562;
			this.splitContainer2.SplitterWidth = 5;
			this.splitContainer2.TabIndex = 0;
			// 
			// tableLayoutPanelPatchingSetup
			// 
			this.tableLayoutPanelPatchingSetup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanelPatchingSetup.AutoSize = true;
			this.tableLayoutPanelPatchingSetup.ColumnCount = 1;
			this.tableLayoutPanelPatchingSetup.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanelPatchingSetup.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanelPatchingSetup.Location = new System.Drawing.Point(0, 67);
			this.tableLayoutPanelPatchingSetup.Name = "tableLayoutPanelPatchingSetup";
			this.tableLayoutPanelPatchingSetup.RowCount = 1;
			this.tableLayoutPanelPatchingSetup.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanelPatchingSetup.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanelPatchingSetup.Size = new System.Drawing.Size(558, 692);
			this.tableLayoutPanelPatchingSetup.TabIndex = 7;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label4.Location = new System.Drawing.Point(3, 37);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(548, 2);
			this.label4.TabIndex = 6;
			// 
			// radioButtonPatchingGraphical
			// 
			this.radioButtonPatchingGraphical.AutoSize = true;
			this.radioButtonPatchingGraphical.Location = new System.Drawing.Point(149, 40);
			this.radioButtonPatchingGraphical.Name = "radioButtonPatchingGraphical";
			this.radioButtonPatchingGraphical.Size = new System.Drawing.Size(103, 19);
			this.radioButtonPatchingGraphical.TabIndex = 5;
			this.radioButtonPatchingGraphical.TabStop = true;
			this.radioButtonPatchingGraphical.Text = "Graphical View";
			this.radioButtonPatchingGraphical.UseVisualStyleBackColor = true;
			this.radioButtonPatchingGraphical.CheckedChanged += new System.EventHandler(this.radioButtonPatchingGraphical_CheckedChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(218, 3);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(79, 20);
			this.label3.TabIndex = 4;
			this.label3.Text = "Patching";
			// 
			// radioButtonPatchingSimple
			// 
			this.radioButtonPatchingSimple.AutoSize = true;
			this.radioButtonPatchingSimple.Location = new System.Drawing.Point(10, 40);
			this.radioButtonPatchingSimple.Name = "radioButtonPatchingSimple";
			this.radioButtonPatchingSimple.Size = new System.Drawing.Size(111, 19);
			this.radioButtonPatchingSimple.TabIndex = 3;
			this.radioButtonPatchingSimple.TabStop = true;
			this.radioButtonPatchingSimple.Text = "Simple Patching";
			this.radioButtonPatchingSimple.UseVisualStyleBackColor = true;
			this.radioButtonPatchingSimple.CheckedChanged += new System.EventHandler(this.radioButtonPatchingSimple_CheckedChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(96, 3);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(96, 20);
			this.label5.TabIndex = 5;
			this.label5.Text = "Controllers";
			// 
			// buttonHelp
			// 
			this.buttonHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonHelp.Location = new System.Drawing.Point(26, 693);
			this.buttonHelp.Name = "buttonHelp";
			this.buttonHelp.Size = new System.Drawing.Size(86, 29);
			this.buttonHelp.TabIndex = 60;
			this.buttonHelp.Tag = "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/nutcracker-eff" +
    "ects/";
			this.buttonHelp.Text = "Help";
			this.buttonHelp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.buttonHelp.UseVisualStyleBackColor = true;
			this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
			this.buttonHelp.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonHelp.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label7.Location = new System.Drawing.Point(5, 679);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(302, 2);
			this.label7.TabIndex = 13;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(213, 693);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(86, 29);
			this.buttonCancel.TabIndex = 12;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(120, 693);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(86, 29);
			this.buttonOk.TabIndex = 11;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			this.buttonOk.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOk.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label6.Location = new System.Drawing.Point(3, 37);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(302, 2);
			this.label6.TabIndex = 10;
			// 
			// tableLayoutPanelControllerSetup
			// 
			this.tableLayoutPanelControllerSetup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanelControllerSetup.AutoSize = true;
			this.tableLayoutPanelControllerSetup.ColumnCount = 1;
			this.tableLayoutPanelControllerSetup.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanelControllerSetup.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanelControllerSetup.Location = new System.Drawing.Point(0, 40);
			this.tableLayoutPanelControllerSetup.Name = "tableLayoutPanelControllerSetup";
			this.tableLayoutPanelControllerSetup.RowCount = 1;
			this.tableLayoutPanelControllerSetup.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanelControllerSetup.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanelControllerSetup.Size = new System.Drawing.Size(311, 636);
			this.tableLayoutPanelControllerSetup.TabIndex = 8;
			// 
			// DisplaySetup
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(1206, 737);
			this.Controls.Add(this.splitContainer1);
			this.DoubleBuffered = true;
			this.MinimumSize = new System.Drawing.Size(1222, 738);
			this.Name = "DisplaySetup";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Display Setup";
			this.Load += new System.EventHandler(this.DisplaySetup_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel1.PerformLayout();
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelElementSetup;
		private System.Windows.Forms.RadioButton radioButtonPatchingGraphical;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.RadioButton radioButtonPatchingSimple;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelPatchingSetup;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelControllerSetup;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonHelp;
	}
}