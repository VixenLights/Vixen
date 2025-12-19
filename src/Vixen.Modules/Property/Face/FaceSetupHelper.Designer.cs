namespace VixenModules.Property.Face {
	partial class FaceSetupHelper {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
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
			DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
			buttonCancel = new Button();
			buttonOK = new Button();
			tableLayoutPanel1 = new TableLayoutPanel();
			tabControl = new TabControl();
			tabMouth = new TabPage();
			dataGridViewMouth = new DataGridView();
			tabOther = new TabPage();
			dataGridViewOther = new DataGridView();
			tableLayoutPanel1.SuspendLayout();
			tabControl.SuspendLayout();
			tabMouth.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)dataGridViewMouth).BeginInit();
			tabOther.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)dataGridViewOther).BeginInit();
			SuspendLayout();
			// 
			// buttonCancel
			// 
			buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonCancel.DialogResult = DialogResult.Cancel;
			buttonCancel.Location = new Point(774, 364);
			buttonCancel.Name = "buttonCancel";
			buttonCancel.Size = new Size(87, 27);
			buttonCancel.TabIndex = 6;
			buttonCancel.Text = "Cancel";
			buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			buttonOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonOK.DialogResult = DialogResult.OK;
			buttonOK.Location = new Point(681, 364);
			buttonOK.Name = "buttonOK";
			buttonOK.Size = new Size(87, 27);
			buttonOK.TabIndex = 5;
			buttonOK.Text = "OK";
			buttonOK.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.ColumnCount = 2;
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
			tableLayoutPanel1.Controls.Add(tabControl, 0, 0);
			tableLayoutPanel1.Controls.Add(buttonOK, 0, 2);
			tableLayoutPanel1.Controls.Add(buttonCancel, 1, 2);
			tableLayoutPanel1.Dock = DockStyle.Fill;
			tableLayoutPanel1.Location = new Point(0, 0);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 3;
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.Size = new Size(864, 394);
			tableLayoutPanel1.TabIndex = 7;
			// 
			// tabControl
			// 
			tableLayoutPanel1.SetColumnSpan(tabControl, 2);
			tabControl.Controls.Add(tabMouth);
			tabControl.Controls.Add(tabOther);
			tabControl.Dock = DockStyle.Fill;
			tabControl.ItemSize = new Size(42, 18);
			tabControl.Location = new Point(6, 6);
			tabControl.Margin = new Padding(6);
			tabControl.Name = "tabControl";
			tabControl.SelectedIndex = 1;
			tabControl.Size = new Size(852, 329);
			tabControl.TabIndex = 7;
			// 
			// tabMouth
			// 
			tabMouth.Controls.Add(dataGridViewMouth);
			tabMouth.Location = new Point(4, 22);
			tabMouth.Margin = new Padding(6);
			tabMouth.Name = "tabMouth";
			tabMouth.Size = new Size(844, 303);
			tabMouth.TabIndex = 1;
			tabMouth.Text = "Mouth";
			tabMouth.UseVisualStyleBackColor = true;
			// 
			// dataGridViewMouth
			// 
			dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = SystemColors.Control;
			dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F);
			dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
			dataGridViewMouth.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			dataGridViewMouth.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = SystemColors.Window;
			dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
			dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
			dataGridViewMouth.DefaultCellStyle = dataGridViewCellStyle2;
			dataGridViewMouth.Dock = DockStyle.Fill;
			dataGridViewMouth.Location = new Point(0, 0);
			dataGridViewMouth.Name = "dataGridViewMouth";
			dataGridViewMouth.Size = new Size(844, 303);
			dataGridViewMouth.TabIndex = 5;
			dataGridViewMouth.CellContentClick += dataGridView_CellContentClick;
			dataGridViewMouth.CellDoubleClick += dataGridView_CellDoubleClick;
			dataGridViewMouth.CellPainting += dataGridViewMouth_CellPainting;
			// 
			// tabOther
			// 
			tabOther.Controls.Add(dataGridViewOther);
			tabOther.Location = new Point(4, 22);
			tabOther.Margin = new Padding(6);
			tabOther.Name = "tabOther";
			tabOther.Size = new Size(844, 303);
			tabOther.TabIndex = 2;
			tabOther.Text = "Outlines/Eyes";
			// 
			// dataGridViewOther
			// 
			dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = SystemColors.Control;
			dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
			dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
			dataGridViewOther.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
			dataGridViewOther.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle4.BackColor = SystemColors.Window;
			dataGridViewCellStyle4.Font = new Font("Segoe UI", 9F);
			dataGridViewCellStyle4.ForeColor = SystemColors.ControlText;
			dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle4.WrapMode = DataGridViewTriState.False;
			dataGridViewOther.DefaultCellStyle = dataGridViewCellStyle4;
			dataGridViewOther.Dock = DockStyle.Fill;
			dataGridViewOther.Location = new Point(0, 0);
			dataGridViewOther.Name = "dataGridViewOther";
			dataGridViewOther.Size = new Size(844, 303);
			dataGridViewOther.TabIndex = 22;
			dataGridViewOther.CellContentClick += dataGridView_CellContentClick;
			dataGridViewOther.CellDoubleClick += dataGridView_CellDoubleClick;
			dataGridViewOther.CellPainting += dataGridViewOther_CellPainting;
			// 
			// FaceSetupHelper
			// 
			AcceptButton = buttonOK;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			CancelButton = buttonCancel;
			ClientSize = new Size(864, 394);
			Controls.Add(tableLayoutPanel1);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "FaceSetupHelper";
			StartPosition = FormStartPosition.CenterParent;
			Text = "Face Setup";
			Load += FaceSetupHelper_Load;
			tableLayoutPanel1.ResumeLayout(false);
			tabControl.ResumeLayout(false);
			tabMouth.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)dataGridViewMouth).EndInit();
			tabOther.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)dataGridViewOther).EndInit();
			ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private TabControl tabControl;
		private TabPage tabMouth;
		private System.Windows.Forms.DataGridView dataGridViewMouth;
		private TabPage tabOther;
		private System.Windows.Forms.DataGridView dataGridViewOther;
	}
}