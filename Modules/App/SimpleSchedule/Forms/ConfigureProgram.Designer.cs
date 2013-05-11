namespace VixenModules.App.SimpleSchedule.Forms
{
	partial class ConfigureProgram {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.panel1 = new System.Windows.Forms.Panel();
			this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.panel4 = new System.Windows.Forms.Panel();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.buttonMoveDown = new System.Windows.Forms.Button();
			this.buttonMoveUp = new System.Windows.Forms.Button();
			this.listBoxSequences = new System.Windows.Forms.ListBox();
			this.panel3 = new System.Windows.Forms.Panel();
			this.buttonMoveLeft = new System.Windows.Forms.Button();
			this.buttonMoveRight = new System.Windows.Forms.Button();
			this.listBoxProgram = new System.Windows.Forms.ListBox();
			this.panel5 = new System.Windows.Forms.Panel();
			this.textBoxProgramName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.tableLayoutPanel.SuspendLayout();
			this.panel4.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panel5.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.tableLayoutPanel);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(621, 410);
			this.panel1.TabIndex = 0;
			// 
			// tableLayoutPanel
			// 
			this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel.ColumnCount = 4;
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 67F));
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 67F));
			this.tableLayoutPanel.Controls.Add(this.panel4, 3, 0);
			this.tableLayoutPanel.Controls.Add(this.listBoxSequences, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.panel3, 1, 0);
			this.tableLayoutPanel.Controls.Add(this.listBoxProgram, 2, 0);
			this.tableLayoutPanel.Controls.Add(this.panel5, 0, 1);
			this.tableLayoutPanel.Location = new System.Drawing.Point(16, 15);
			this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 2;
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
			this.tableLayoutPanel.Size = new System.Drawing.Size(589, 335);
			this.tableLayoutPanel.TabIndex = 0;
			// 
			// panel4
			// 
			this.panel4.Controls.Add(this.buttonDelete);
			this.panel4.Controls.Add(this.buttonMoveDown);
			this.panel4.Controls.Add(this.buttonMoveUp);
			this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel4.Location = new System.Drawing.Point(525, 4);
			this.panel4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(60, 265);
			this.panel4.TabIndex = 3;
			// 
			// buttonDelete
			// 
			this.buttonDelete.Enabled = false;
			this.buttonDelete.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonDelete.ForeColor = System.Drawing.Color.Red;
			this.buttonDelete.Location = new System.Drawing.Point(9, 114);
			this.buttonDelete.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(40, 37);
			this.buttonDelete.TabIndex = 4;
			this.buttonDelete.Text = "x";
			this.buttonDelete.UseVisualStyleBackColor = true;
			this.buttonDelete.Visible = false;
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// buttonMoveDown
			// 
			this.buttonMoveDown.Enabled = false;
			this.buttonMoveDown.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonMoveDown.Location = new System.Drawing.Point(9, 52);
			this.buttonMoveDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.buttonMoveDown.Name = "buttonMoveDown";
			this.buttonMoveDown.Size = new System.Drawing.Size(40, 37);
			this.buttonMoveDown.TabIndex = 3;
			this.buttonMoveDown.Text = "˅";
			this.buttonMoveDown.UseVisualStyleBackColor = true;
			this.buttonMoveDown.Click += new System.EventHandler(this.buttonMoveDown_Click);
			// 
			// buttonMoveUp
			// 
			this.buttonMoveUp.Enabled = false;
			this.buttonMoveUp.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonMoveUp.Location = new System.Drawing.Point(9, 4);
			this.buttonMoveUp.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.buttonMoveUp.Name = "buttonMoveUp";
			this.buttonMoveUp.Size = new System.Drawing.Size(40, 37);
			this.buttonMoveUp.TabIndex = 2;
			this.buttonMoveUp.Text = "˄";
			this.buttonMoveUp.UseVisualStyleBackColor = true;
			this.buttonMoveUp.Click += new System.EventHandler(this.buttonMoveUp_Click);
			// 
			// listBoxSequences
			// 
			this.listBoxSequences.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBoxSequences.FormattingEnabled = true;
			this.listBoxSequences.ItemHeight = 16;
			this.listBoxSequences.Location = new System.Drawing.Point(4, 4);
			this.listBoxSequences.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.listBoxSequences.Name = "listBoxSequences";
			this.listBoxSequences.Size = new System.Drawing.Size(219, 265);
			this.listBoxSequences.TabIndex = 0;
			this.listBoxSequences.SelectedIndexChanged += new System.EventHandler(this.listBoxSequences_SelectedIndexChanged);
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.buttonMoveLeft);
			this.panel3.Controls.Add(this.buttonMoveRight);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(231, 4);
			this.panel3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(59, 265);
			this.panel3.TabIndex = 1;
			// 
			// buttonMoveLeft
			// 
			this.buttonMoveLeft.Enabled = false;
			this.buttonMoveLeft.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonMoveLeft.Location = new System.Drawing.Point(9, 52);
			this.buttonMoveLeft.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.buttonMoveLeft.Name = "buttonMoveLeft";
			this.buttonMoveLeft.Size = new System.Drawing.Size(40, 37);
			this.buttonMoveLeft.TabIndex = 3;
			this.buttonMoveLeft.Text = "<";
			this.buttonMoveLeft.UseVisualStyleBackColor = true;
			this.buttonMoveLeft.Click += new System.EventHandler(this.buttonMoveLeft_Click);
			// 
			// buttonMoveRight
			// 
			this.buttonMoveRight.Enabled = false;
			this.buttonMoveRight.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonMoveRight.Location = new System.Drawing.Point(9, 4);
			this.buttonMoveRight.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.buttonMoveRight.Name = "buttonMoveRight";
			this.buttonMoveRight.Size = new System.Drawing.Size(40, 37);
			this.buttonMoveRight.TabIndex = 2;
			this.buttonMoveRight.Text = ">";
			this.buttonMoveRight.UseVisualStyleBackColor = true;
			this.buttonMoveRight.Click += new System.EventHandler(this.buttonMoveRight_Click);
			// 
			// listBoxProgram
			// 
			this.listBoxProgram.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBoxProgram.FormattingEnabled = true;
			this.listBoxProgram.ItemHeight = 16;
			this.listBoxProgram.Location = new System.Drawing.Point(298, 4);
			this.listBoxProgram.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.listBoxProgram.Name = "listBoxProgram";
			this.listBoxProgram.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxProgram.Size = new System.Drawing.Size(219, 265);
			this.listBoxProgram.TabIndex = 2;
			this.listBoxProgram.SelectedIndexChanged += new System.EventHandler(this.listBoxProgram_SelectedIndexChanged);
			// 
			// panel5
			// 
			this.tableLayoutPanel.SetColumnSpan(this.panel5, 4);
			this.panel5.Controls.Add(this.textBoxProgramName);
			this.panel5.Controls.Add(this.label1);
			this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel5.Location = new System.Drawing.Point(4, 277);
			this.panel5.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(581, 54);
			this.panel5.TabIndex = 4;
			// 
			// textBoxProgramName
			// 
			this.textBoxProgramName.Location = new System.Drawing.Point(131, 15);
			this.textBoxProgramName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.textBoxProgramName.Name = "textBoxProgramName";
			this.textBoxProgramName.Size = new System.Drawing.Size(236, 20);
			this.textBoxProgramName.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(19, 18);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(78, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Program name:";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.buttonCancel);
			this.panel2.Controls.Add(this.buttonOK);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 357);
			this.panel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(621, 53);
			this.panel2.TabIndex = 1;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(505, 10);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(100, 28);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(397, 10);
			this.buttonOK.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(100, 28);
			this.buttonOK.TabIndex = 0;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// ConfigureProgram
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(621, 410);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "ConfigureProgram";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Program";
			this.panel1.ResumeLayout(false);
			this.tableLayoutPanel.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel5.ResumeLayout(false);
			this.panel5.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Button buttonMoveDown;
		private System.Windows.Forms.Button buttonMoveUp;
		private System.Windows.Forms.ListBox listBoxSequences;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Button buttonMoveLeft;
		private System.Windows.Forms.Button buttonMoveRight;
		private System.Windows.Forms.ListBox listBoxProgram;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.TextBox textBoxProgramName;
		private System.Windows.Forms.Label label1;
	}
}