namespace VixenApplication.FiltersAndPatching
{
	partial class PatchingWizard_3_Destinations
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
			this.listViewDestinations = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(18, 123);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 16);
			this.label1.TabIndex = 16;
			this.label1.Text = "for patching.";
			// 
			// listViewDestinations
			// 
			this.listViewDestinations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewDestinations.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.listViewDestinations.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewDestinations.Location = new System.Drawing.Point(22, 170);
			this.listViewDestinations.Name = "listViewDestinations";
			this.listViewDestinations.Size = new System.Drawing.Size(180, 180);
			this.listViewDestinations.TabIndex = 15;
			this.listViewDestinations.UseCompatibleStateImageBehavior = false;
			this.listViewDestinations.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Width = 160;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(18, 99);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(418, 16);
			this.label4.TabIndex = 14;
			this.label4.Text = "The list below shows the components that will be used, and their order";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(18, 75);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(413, 16);
			this.label3.TabIndex = 13;
			this.label3.Text = "will be the destinations for these patches (e.g. a collection of outputs).";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(18, 51);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(412, 16);
			this.label2.TabIndex = 12;
			this.label2.Text = "Using the Filter && Patching Setup window, select the components that";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(18, 17);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(229, 20);
			this.label5.TabIndex = 11;
			this.label5.Text = "Step 3: Select Destinations";
			// 
			// PatchingWizard_3_Destinations
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listViewDestinations);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label5);
			this.Name = "PatchingWizard_3_Destinations";
			this.Size = new System.Drawing.Size(500, 400);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListView listViewDestinations;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label5;

	}
}
