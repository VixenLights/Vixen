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
		private void InitializeComponent() {
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tabControl = new Dotnetrix.Controls.TabControlEX();
			this.tabMouth = new Dotnetrix.Controls.TabPageEX();
			this.dataGridViewMouth = new System.Windows.Forms.DataGridView();
			this.tabOther = new Dotnetrix.Controls.TabPageEX();
			this.dataGridViewOther = new System.Windows.Forms.DataGridView();
			this.tableLayoutPanel1.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.tabMouth.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewMouth)).BeginInit();
			this.tabOther.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewOther)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(774, 364);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(87, 27);
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(681, 364);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(87, 27);
			this.buttonOK.TabIndex = 5;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOK.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.tabControl, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.buttonOK, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.buttonCancel, 1, 2);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(864, 394);
			this.tableLayoutPanel1.TabIndex = 7;
			// 
			// tabControl
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.tabControl, 2);
			this.tabControl.Controls.Add(this.tabMouth);
			this.tabControl.Controls.Add(this.tabOther);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.ItemSize = new System.Drawing.Size(42, 18);
			this.tabControl.Location = new System.Drawing.Point(6, 6);
			this.tabControl.Margin = new System.Windows.Forms.Padding(6);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 1;
			this.tabControl.Size = new System.Drawing.Size(852, 329);
			this.tabControl.TabIndex = 7;
			this.tabControl.UseVisualStyles = false;
			// 
			// tabMouth
			// 
			this.tabMouth.Controls.Add(this.dataGridViewMouth);
			this.tabMouth.Location = new System.Drawing.Point(4, 22);
			this.tabMouth.Margin = new System.Windows.Forms.Padding(6);
			this.tabMouth.Name = "tabMouth";
			this.tabMouth.Size = new System.Drawing.Size(943, 507);
			this.tabMouth.TabIndex = 1;
			this.tabMouth.Text = "Mouth";
			this.tabMouth.UseVisualStyleBackColor = true;
			// 
			// dataGridViewMouth
			// 
			this.dataGridViewMouth.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.dataGridViewMouth.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewMouth.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewMouth.Location = new System.Drawing.Point(0, 0);
			this.dataGridViewMouth.Name = "dataGridViewMouth";
			this.dataGridViewMouth.Size = new System.Drawing.Size(943, 507);
			this.dataGridViewMouth.TabIndex = 5;
			this.dataGridViewMouth.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellContentClick);
			this.dataGridViewMouth.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellDoubleClick);
			this.dataGridViewMouth.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridViewMouth_CellPainting);
			// 
			// tabOther
			// 
			this.tabOther.Controls.Add(this.dataGridViewOther);
			this.tabOther.Location = new System.Drawing.Point(4, 22);
			this.tabOther.Margin = new System.Windows.Forms.Padding(6);
			this.tabOther.Name = "tabOther";
			this.tabOther.Size = new System.Drawing.Size(844, 303);
			this.tabOther.TabIndex = 2;
			this.tabOther.Text = "Outlines/Eyes";
			// 
			// dataGridViewOther
			// 
			this.dataGridViewOther.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.dataGridViewOther.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewOther.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewOther.Location = new System.Drawing.Point(0, 0);
			this.dataGridViewOther.Name = "dataGridViewOther";
			this.dataGridViewOther.Size = new System.Drawing.Size(844, 303);
			this.dataGridViewOther.TabIndex = 22;
			this.dataGridViewOther.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellContentClick);
			this.dataGridViewOther.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellDoubleClick);
			this.dataGridViewOther.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridViewOther_CellPainting);
			// 
			// FaceSetupHelper
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(864, 394);
			this.Controls.Add(this.tableLayoutPanel1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FaceSetupHelper";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Face Setup";
			this.Load += new System.EventHandler(this.FaceSetupHelper_Load);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.tabMouth.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewMouth)).EndInit();
			this.tabOther.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewOther)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private Dotnetrix.Controls.TabControlEX tabControl;
		private Dotnetrix.Controls.TabPageEX tabMouth;
		private System.Windows.Forms.DataGridView dataGridViewMouth;
		private Dotnetrix.Controls.TabPageEX tabOther;
		private System.Windows.Forms.DataGridView dataGridViewOther;
	}
}