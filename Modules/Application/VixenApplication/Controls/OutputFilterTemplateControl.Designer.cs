namespace VixenApplication.Controls {
	partial class OutputFilterTemplateControl {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.buttonMoveUp = new System.Windows.Forms.Button();
			this.buttonMoveDown = new System.Windows.Forms.Button();
			this.listBoxFilters = new System.Windows.Forms.ListBox();
			this.buttonRemove = new System.Windows.Forms.Button();
			this.comboBoxFilters = new System.Windows.Forms.ComboBox();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.buttonConfigure = new System.Windows.Forms.Button();
			this.labelTitle = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonMoveUp
			// 
			this.buttonMoveUp.Enabled = false;
			this.buttonMoveUp.Location = new System.Drawing.Point(12, 32);
			this.buttonMoveUp.Name = "buttonMoveUp";
			this.buttonMoveUp.Size = new System.Drawing.Size(33, 35);
			this.buttonMoveUp.TabIndex = 0;
			this.buttonMoveUp.UseVisualStyleBackColor = true;
			this.buttonMoveUp.Click += new System.EventHandler(this.buttonMoveUp_Click);
			// 
			// buttonMoveDown
			// 
			this.buttonMoveDown.Enabled = false;
			this.buttonMoveDown.Location = new System.Drawing.Point(12, 73);
			this.buttonMoveDown.Name = "buttonMoveDown";
			this.buttonMoveDown.Size = new System.Drawing.Size(33, 35);
			this.buttonMoveDown.TabIndex = 1;
			this.buttonMoveDown.UseVisualStyleBackColor = true;
			this.buttonMoveDown.Click += new System.EventHandler(this.buttonMoveDown_Click);
			// 
			// listBoxFilters
			// 
			this.listBoxFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxFilters.FormattingEnabled = true;
			this.listBoxFilters.Location = new System.Drawing.Point(62, 32);
			this.listBoxFilters.Name = "listBoxFilters";
			this.listBoxFilters.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxFilters.Size = new System.Drawing.Size(187, 199);
			this.listBoxFilters.TabIndex = 2;
			this.listBoxFilters.SelectedIndexChanged += new System.EventHandler(this.listBoxFilters_SelectedIndexChanged);
			// 
			// buttonRemove
			// 
			this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonRemove.Enabled = false;
			this.buttonRemove.Location = new System.Drawing.Point(62, 240);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(75, 23);
			this.buttonRemove.TabIndex = 3;
			this.buttonRemove.Text = "Remove";
			this.buttonRemove.UseVisualStyleBackColor = true;
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			// 
			// comboBoxFilters
			// 
			this.comboBoxFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxFilters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxFilters.FormattingEnabled = true;
			this.comboBoxFilters.Location = new System.Drawing.Point(267, 32);
			this.comboBoxFilters.Name = "comboBoxFilters";
			this.comboBoxFilters.Size = new System.Drawing.Size(228, 21);
			this.comboBoxFilters.TabIndex = 4;
			this.comboBoxFilters.SelectedIndexChanged += new System.EventHandler(this.comboBoxFilters_SelectedIndexChanged);
			// 
			// buttonAdd
			// 
			this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonAdd.Enabled = false;
			this.buttonAdd.Location = new System.Drawing.Point(267, 59);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(75, 23);
			this.buttonAdd.TabIndex = 5;
			this.buttonAdd.Text = "Add";
			this.buttonAdd.UseVisualStyleBackColor = true;
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// buttonConfigure
			// 
			this.buttonConfigure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonConfigure.Enabled = false;
			this.buttonConfigure.Location = new System.Drawing.Point(267, 127);
			this.buttonConfigure.Name = "buttonConfigure";
			this.buttonConfigure.Size = new System.Drawing.Size(75, 23);
			this.buttonConfigure.TabIndex = 6;
			this.buttonConfigure.Text = "Configure";
			this.buttonConfigure.UseVisualStyleBackColor = true;
			this.buttonConfigure.Click += new System.EventHandler(this.buttonConfigure_Click);
			// 
			// labelTitle
			// 
			this.labelTitle.AutoSize = true;
			this.labelTitle.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelTitle.Location = new System.Drawing.Point(1, 2);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(69, 16);
			this.labelTitle.TabIndex = 7;
			this.labelTitle.Text = "Output X";
			// 
			// PostFilterTemplateControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.labelTitle);
			this.Controls.Add(this.buttonConfigure);
			this.Controls.Add(this.buttonAdd);
			this.Controls.Add(this.comboBoxFilters);
			this.Controls.Add(this.buttonRemove);
			this.Controls.Add(this.listBoxFilters);
			this.Controls.Add(this.buttonMoveDown);
			this.Controls.Add(this.buttonMoveUp);
			this.Name = "PostFilterTemplateControl";
			this.Size = new System.Drawing.Size(507, 277);
			this.Load += new System.EventHandler(this.PostFilterTemplateControl_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonMoveUp;
		private System.Windows.Forms.Button buttonMoveDown;
		private System.Windows.Forms.ListBox listBoxFilters;
		private System.Windows.Forms.Button buttonRemove;
		private System.Windows.Forms.ComboBox comboBoxFilters;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonConfigure;
		private System.Windows.Forms.Label labelTitle;
	}
}
