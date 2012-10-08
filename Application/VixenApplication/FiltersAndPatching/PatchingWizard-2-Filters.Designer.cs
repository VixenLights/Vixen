namespace VixenApplication.FiltersAndPatching
{
	partial class PatchingWizard_2_Filters
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
			this.listViewFilters = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonAddFilter = new System.Windows.Forms.Button();
			this.comboBoxNewFilterTypes = new System.Windows.Forms.ComboBox();
			this.groupBoxSelectedFilter = new System.Windows.Forms.GroupBox();
			this.buttonSetupFilter = new System.Windows.Forms.Button();
			this.buttonMoveDown = new System.Windows.Forms.Button();
			this.buttonMoveUp = new System.Windows.Forms.Button();
			this.buttonDeleteSelected = new System.Windows.Forms.Button();
			this.groupBoxSelectedFilter.SuspendLayout();
			this.SuspendLayout();
			// 
			// listViewFilters
			// 
			this.listViewFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewFilters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.listViewFilters.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewFilters.HideSelection = false;
			this.listViewFilters.Location = new System.Drawing.Point(22, 170);
			this.listViewFilters.MultiSelect = false;
			this.listViewFilters.Name = "listViewFilters";
			this.listViewFilters.Size = new System.Drawing.Size(180, 200);
			this.listViewFilters.TabIndex = 9;
			this.listViewFilters.UseCompatibleStateImageBehavior = false;
			this.listViewFilters.View = System.Windows.Forms.View.Details;
			this.listViewFilters.SelectedIndexChanged += new System.EventHandler(this.listViewFilters_SelectedIndexChanged);
			this.listViewFilters.DoubleClick += new System.EventHandler(this.listViewFilters_DoubleClick);
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
			this.label4.Size = new System.Drawing.Size(426, 16);
			this.label4.TabIndex = 8;
			this.label4.Text = "filters here. Each filter will be duplicated multiple times as a new layer of";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(18, 75);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(467, 16);
			this.label3.TabIndex = 7;
			this.label3.Text = " destination components (e.g. a color filter or dimming filter), add and configur" +
    "e";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(18, 51);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(411, 16);
			this.label2.TabIndex = 6;
			this.label2.Text = "If you would like to automatically add filter(s) between the source and";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(18, 17);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(243, 20);
			this.label5.TabIndex = 5;
			this.label5.Text = "Step 2: Add Filters (Optional)";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(18, 123);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(447, 16);
			this.label1.TabIndex = 10;
			this.label1.Text = "filtering for each source. If you do not want any filters, you can skip this step" +
    ".";
			// 
			// buttonAddFilter
			// 
			this.buttonAddFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonAddFilter.Location = new System.Drawing.Point(364, 177);
			this.buttonAddFilter.Name = "buttonAddFilter";
			this.buttonAddFilter.Size = new System.Drawing.Size(80, 25);
			this.buttonAddFilter.TabIndex = 12;
			this.buttonAddFilter.Text = "Add Filter";
			this.buttonAddFilter.UseVisualStyleBackColor = true;
			this.buttonAddFilter.Click += new System.EventHandler(this.buttonAddFilter_Click);
			// 
			// comboBoxNewFilterTypes
			// 
			this.comboBoxNewFilterTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.comboBoxNewFilterTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxNewFilterTypes.FormattingEnabled = true;
			this.comboBoxNewFilterTypes.Location = new System.Drawing.Point(217, 180);
			this.comboBoxNewFilterTypes.Name = "comboBoxNewFilterTypes";
			this.comboBoxNewFilterTypes.Size = new System.Drawing.Size(140, 21);
			this.comboBoxNewFilterTypes.TabIndex = 11;
			this.comboBoxNewFilterTypes.SelectedIndexChanged += new System.EventHandler(this.comboBoxNewFilterTypes_SelectedIndexChanged);
			// 
			// groupBoxSelectedFilter
			// 
			this.groupBoxSelectedFilter.Controls.Add(this.buttonSetupFilter);
			this.groupBoxSelectedFilter.Controls.Add(this.buttonMoveDown);
			this.groupBoxSelectedFilter.Controls.Add(this.buttonMoveUp);
			this.groupBoxSelectedFilter.Controls.Add(this.buttonDeleteSelected);
			this.groupBoxSelectedFilter.Location = new System.Drawing.Point(217, 223);
			this.groupBoxSelectedFilter.Name = "groupBoxSelectedFilter";
			this.groupBoxSelectedFilter.Size = new System.Drawing.Size(227, 113);
			this.groupBoxSelectedFilter.TabIndex = 17;
			this.groupBoxSelectedFilter.TabStop = false;
			this.groupBoxSelectedFilter.Text = "Selected Filter";
			// 
			// buttonSetupFilter
			// 
			this.buttonSetupFilter.Location = new System.Drawing.Point(72, 29);
			this.buttonSetupFilter.Name = "buttonSetupFilter";
			this.buttonSetupFilter.Size = new System.Drawing.Size(80, 25);
			this.buttonSetupFilter.TabIndex = 20;
			this.buttonSetupFilter.Text = "Setup";
			this.buttonSetupFilter.UseVisualStyleBackColor = true;
			this.buttonSetupFilter.Click += new System.EventHandler(this.buttonSetupFilter_Click);
			// 
			// buttonMoveDown
			// 
			this.buttonMoveDown.Image = global::VixenApplication.Properties.Resources.DownArrowShort_Blue_16x16;
			this.buttonMoveDown.Location = new System.Drawing.Point(21, 64);
			this.buttonMoveDown.Name = "buttonMoveDown";
			this.buttonMoveDown.Size = new System.Drawing.Size(32, 32);
			this.buttonMoveDown.TabIndex = 19;
			this.buttonMoveDown.UseVisualStyleBackColor = true;
			this.buttonMoveDown.Click += new System.EventHandler(this.buttonMoveDown_Click);
			// 
			// buttonMoveUp
			// 
			this.buttonMoveUp.Image = global::VixenApplication.Properties.Resources.UpArrowShort_Blue_16x16;
			this.buttonMoveUp.Location = new System.Drawing.Point(21, 26);
			this.buttonMoveUp.Name = "buttonMoveUp";
			this.buttonMoveUp.Size = new System.Drawing.Size(32, 32);
			this.buttonMoveUp.TabIndex = 18;
			this.buttonMoveUp.UseVisualStyleBackColor = true;
			this.buttonMoveUp.Click += new System.EventHandler(this.buttonMoveUp_Click);
			// 
			// buttonDeleteSelected
			// 
			this.buttonDeleteSelected.Image = global::VixenApplication.Properties.Resources.Delete;
			this.buttonDeleteSelected.Location = new System.Drawing.Point(72, 64);
			this.buttonDeleteSelected.Name = "buttonDeleteSelected";
			this.buttonDeleteSelected.Size = new System.Drawing.Size(32, 32);
			this.buttonDeleteSelected.TabIndex = 17;
			this.buttonDeleteSelected.UseVisualStyleBackColor = true;
			this.buttonDeleteSelected.Click += new System.EventHandler(this.buttonDeleteSelected_Click);
			// 
			// PatchingWizard_2_Filters
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBoxSelectedFilter);
			this.Controls.Add(this.buttonAddFilter);
			this.Controls.Add(this.comboBoxNewFilterTypes);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listViewFilters);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label5);
			this.Name = "PatchingWizard_2_Filters";
			this.Size = new System.Drawing.Size(500, 400);
			this.Load += new System.EventHandler(this.PatchingWizard_2_Filters_Load);
			this.groupBoxSelectedFilter.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListView listViewFilters;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonAddFilter;
		private System.Windows.Forms.ComboBox comboBoxNewFilterTypes;
		private System.Windows.Forms.GroupBox groupBoxSelectedFilter;
		private System.Windows.Forms.Button buttonSetupFilter;
		private System.Windows.Forms.Button buttonMoveDown;
		private System.Windows.Forms.Button buttonMoveUp;
		private System.Windows.Forms.Button buttonDeleteSelected;
	}
}
