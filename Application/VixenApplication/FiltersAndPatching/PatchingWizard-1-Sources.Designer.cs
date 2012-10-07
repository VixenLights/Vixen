namespace VixenApplication.FiltersAndPatching
{
	partial class PatchingWizard_1_Sources
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.listViewSources = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(18, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(194, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Step 1: Select Sources";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(18, 51);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(439, 16);
			this.label2.TabIndex = 1;
			this.label2.Text = "Using the Filter && Patching Setup window, select the source(s) you want to";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(18, 75);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(426, 16);
			this.label3.TabIndex = 2;
			this.label3.Text = "patch (e.g. a collection of channels.) The list below shows what sources";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(18, 99);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(170, 16);
			this.label4.TabIndex = 3;
			this.label4.Text = "will be used and their order.";
			// 
			// listViewSources
			// 
			this.listViewSources.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewSources.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.listViewSources.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewSources.Location = new System.Drawing.Point(22, 170);
			this.listViewSources.Name = "listViewSources";
			this.listViewSources.Size = new System.Drawing.Size(180, 180);
			this.listViewSources.TabIndex = 4;
			this.listViewSources.UseCompatibleStateImageBehavior = false;
			this.listViewSources.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Width = 160;
			// 
			// PatchingWizard_1_Sources
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.listViewSources);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Name = "PatchingWizard_1_Sources";
			this.Size = new System.Drawing.Size(500, 400);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ListView listViewSources;
		private System.Windows.Forms.ColumnHeader columnHeader1;

	}
}
